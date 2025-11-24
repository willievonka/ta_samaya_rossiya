using System.Diagnostics;
using System.Text;
using Application.Services.Interfaces.OpenStreetMap;

namespace Infrastructure.Services.Implementations.OpenStreetMap;

public class OsmToGeoJsonConverter : IOsmToGeoJsonConverter
{
    private readonly ILogger<OsmToGeoJsonConverter> _logger;
    
    private const string WorkingDirectory = "OsmToGeoJson";
    private const string ScriptName = "OsmToGeoJsonConvert.js";

    public OsmToGeoJsonConverter(ILogger<OsmToGeoJsonConverter> logger)
    {
        _logger = logger;
    }

    public async Task<string> OsmToGeoJsonAsync(string osmData, CancellationToken ct)
    {
        _logger.LogInformation("Starting the OSM → GeoJSON conversion. Input data size: {Size} bytes", osmData.Length);

        if (string.IsNullOrWhiteSpace(osmData))
        {
            _logger.LogError("OSM data cannot be empty");
            throw new ArgumentException("OSM data cannot be empty", nameof(osmData));
        }

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WorkingDirectory, ScriptName);

        if (!File.Exists(path))
        {
            _logger.LogError("No JS script found on the path: {ScriptPath}", path);
            throw new FileNotFoundException("OsmToGeoJsonConvert.js not found", path);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"\"{path}\"",
            WorkingDirectory = WorkingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };
        
        _logger.LogDebug("Starting the process: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);

        try
        {
            process.Start();

            await using (var sw = process.StandardInput)
            {
                await sw.WriteAsync(osmData.AsMemory(), ct);
                await sw.FlushAsync(ct);
            }
            
            var outputTask = process.StandardOutput.ReadToEndAsync(ct);
            var errorTask = process.StandardError.ReadToEndAsync(ct);
            
            var allReadingTask = Task.WhenAll(outputTask, errorTask);
            
            var completedTask = await Task.WhenAny(
                allReadingTask,
                Task.Delay(TimeSpan.FromSeconds(30), ct));
            
            if (completedTask != allReadingTask)
            {
                if (!process.HasExited) 
                    process.Kill(true);
                
                _logger.LogWarning("OSM → GeoJSON conversion timeout (30 seconds). The process is killed.");
                throw new TimeoutException("Conversion timeout exceeded (30 seconds)");
            }

            var errorOutput = await errorTask;
            await process.WaitForExitAsync(ct);
            
            if (process.ExitCode != 0)
            {
                _logger.LogError("Node.The js failed with an error. ExitCode: {ExitCode}. Message: {ErrorMessage}", 
                    process.ExitCode, errorOutput);

                throw new InvalidOperationException($"Node.the js script crashed with the code {process.ExitCode}");
            }

            var result = (await outputTask).Trim();
            
            _logger.LogInformation("The conversion was successful. GeoJSON size: {GeoJsonBytes} bytes", result.Length);

            return result;
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            _logger.LogError(e, "Critical error in OsmToGeoJsonConverter");
            throw;
        }
    }
}