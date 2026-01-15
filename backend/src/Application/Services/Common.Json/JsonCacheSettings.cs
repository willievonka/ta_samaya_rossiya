using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Services.Common.Json;

public static class JsonCacheSettings
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        
        Converters = { new NetTopologySuite.IO.Converters.GeoJsonConverterFactory() }
    };
}