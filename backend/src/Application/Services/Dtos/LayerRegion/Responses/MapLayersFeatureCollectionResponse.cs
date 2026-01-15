using System.Text.Json.Serialization;

namespace Application.Services.Dtos.LayerRegion.Responses;

public record MapLayersFeatureCollectionResponse(
    IReadOnlyList<MapLayerResponse> Features
    )
{
    [JsonPropertyOrder(-1)]
    public string Type => "FeatureCollection";
}