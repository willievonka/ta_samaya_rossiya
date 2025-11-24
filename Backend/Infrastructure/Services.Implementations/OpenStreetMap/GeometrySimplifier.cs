using System.Diagnostics;
using System.Text;
using Application.Services.Interfaces.OpenStreetMap;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Infrastructure.Services.Implementations.OpenStreetMap;

public class GeometrySimplifier : IGeometrySimplifier
{
    private readonly ILogger<GeometrySimplifier> _logger;
    private const string ScriptsFolder = "OsmToGeoJson";
    private const string ScriptName = "Simplify.js";
    
    public GeometrySimplifier(ILogger<GeometrySimplifier> logger)
    {
        _logger = logger;
    }
    
    public async Task<Geometry> SimplifyToPercentageAsync(string geoJson, CancellationToken ct = default)
    {
        if (geoJson is not { Length: > 0 })
            throw new ArgumentException("GeoJSON cannot be null or empty", nameof(geoJson));

        _logger.LogInformation("Starting geometry simplification. Input size: {Size} characters", geoJson.Length);

        var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ScriptsFolder, ScriptName);

        if (!File.Exists(scriptPath))
        {
            _logger.LogCritical("Simplification script not found: {ScriptPath}", scriptPath);
            throw new FileNotFoundException("Simplify.js not found", scriptPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"\"{scriptPath}\"",
            WorkingDirectory = Path.GetDirectoryName(scriptPath)!,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };

        _logger.LogDebug("Launch: node {Script}", ScriptName);

        try
        {
            process.Start();

            await using (var writer = process.StandardInput)
            {
                await writer.WriteAsync(geoJson.AsMemory(), ct);
                await writer.FlushAsync(ct);
            }

            var outputTask = process.StandardOutput.ReadToEndAsync(ct);
            var errorTask  = process.StandardError.ReadToEndAsync(ct);

            var allReadingTask = Task.WhenAll(outputTask, errorTask);
            
            var completedTask = await Task.WhenAny(
                allReadingTask,
                Task.Delay(TimeSpan.FromSeconds(45), ct)
            );

            if (completedTask != allReadingTask)
            {
                if (!process.HasExited) 
                    process.Kill(true);
                
                _logger.LogWarning("Geometry simplification timeout (45 seconds). The process is killed.");
                throw new TimeoutException($"Geometry simplification was not completed in (45 seconds)");
            }

            var errorOutput = await errorTask;
            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
            {
                _logger.LogError(
                    "Simplify.js completed with an error. ExitCode: {ExitCode}\nError: {Error}",
                    process.ExitCode, errorOutput.Trim());

                throw new InvalidOperationException(
                    $"Simplify.js completed with an error. ExitCode: crashed with the {process.ExitCode}.");
            }

            var simplifiedJson = (await outputTask).Trim();

            if (string.IsNullOrWhiteSpace(simplifiedJson))
            {
                _logger.LogError("Simplify.js returned an empty result");
                throw new InvalidOperationException("Simplified GeoJSON is empty");
            }

            var reader = new GeoJsonReader();
            var featureCollection = reader.Read<FeatureCollection>(simplifiedJson);

            if (featureCollection is not { Count: >= 1 })
            {
                _logger.LogWarning("FeatureCollection is empty after simplification");
                throw new InvalidOperationException("After simplification, there are no geometries left");
            }

            var geometry = featureCollection.First().Geometry;

            if (geometry is null)
            {
                _logger.LogWarning("The first feature does not contain Geometry");
                throw new InvalidOperationException("Geometry is missing in the result");
            }

            _logger.LogInformation(
                "Geometry simplification is successful. The original size: {InputSize} → Result: {OutputSize} characters",
                geoJson.Length, simplifiedJson.Length);

            return geometry;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Critical error in GeometrySimplifier");
            throw;
        }
    }
}