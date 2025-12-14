using System.Text.Json.Serialization;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Response;

public record MapLayersFeatureCollectionResponse(
    IReadOnlyList<MapLayerResponse> Features
    )
{
    [JsonPropertyOrder(-1)]
    public string Type => "FeatureCollection";
}