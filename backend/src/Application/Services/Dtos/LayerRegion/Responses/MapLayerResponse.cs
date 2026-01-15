using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace Application.Services.Dtos.LayerRegion.Responses;

public record MapLayerResponse(
    Geometry Geometry,
    MapLayerPropertiesResponse Properties
)
{
    [JsonPropertyOrder(-1)]
    public string Type => "Feature";
};