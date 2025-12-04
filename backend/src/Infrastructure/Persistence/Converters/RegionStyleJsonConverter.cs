using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.Converters;

public static class RegionStyleJsonConverter
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static readonly ValueComparer<LayerRegionStyle> Comparer = new ValueComparer<LayerRegionStyle>(
            (a, b) => JsonSerializer.Serialize(a, Options) == JsonSerializer.Serialize(b, Options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, Options).GetHashCode(),
            v => v == null
                ? null
                : JsonSerializer.Deserialize<LayerRegionStyle>(JsonSerializer.Serialize(v, Options), Options)!);
}