using Infrastructure.Services.Implementations.OpenStreetMap;
using Moq;
using Xunit;

namespace Tests;

[Collection("OsmToGeoJson tests")]
public class OsmToGeoJsonConverterTest
{
    private readonly string _tempBaseDirectory;
    private readonly OsmToGeoJsonConverter _converter;
    private readonly Mock<ILogger<OsmToGeoJsonConverter>> _loggerMock = new();

    private static readonly string ScriptPath = Path.GetFullPath(Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "..", "..", "..", "..",
        "Backend", "WebApi", "OsmToGeoJson", "OsmToGeoJsonConvert.js"));

    private static readonly string ValidJsonData = "D:\\RiderProjects\\TaSamayaRossiya\\Tests\\Geo.txt";
    private static readonly string SimplyGeoData = "D:\\RiderProjects\\TaSamayaRossiya\\Tests\\SimplyGeo.txt";
    
    public OsmToGeoJsonConverterTest()
    {
        if (!File.Exists(ScriptPath))
            throw new FileNotFoundException($"Не найден JS-скрипт: {ScriptPath}");

        if (!File.Exists(ValidJsonData))
            throw new FileNotFoundException("Не найдены исходные данные");

        if (!File.Exists(SimplyGeoData))
            throw new FileNotFoundException("Не найдены конечные данные");
        
        // Создаём временную папку, которая будет BaseDirectory
        _tempBaseDirectory = Path.Combine(Path.GetTempPath(), "OsmTest_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempBaseDirectory);

        var workingDir = Path.Combine(_tempBaseDirectory, "OsmToGeoJson");
        Directory.CreateDirectory(workingDir);

        // Копируем настоящий скрипт во временную папку
        File.Copy(ScriptPath, Path.Combine(workingDir, "OsmToGeoJsonConvert.js"), true);

        // Подменяем BaseDirectory — это ключевой момент!
        AppDomain.CurrentDomain.SetData("BaseDirectory", _tempBaseDirectory);

        _converter = new OsmToGeoJsonConverter(_loggerMock.Object);
    }

    [Fact]
    public async Task OsmToGeoJsonAsync_WithRealValidOsmFile_ReturnsValidGeoJson()
    {
        var osmJson = File.ReadAllText(ValidJsonData);
        
        var act = await _converter.OsmToGeoJsonAsync(osmJson, CancellationToken.None);
        
        var simplyData = await File.ReadAllTextAsync(SimplyGeoData);
        
        Assert.Equal(act, simplyData);
    }
}