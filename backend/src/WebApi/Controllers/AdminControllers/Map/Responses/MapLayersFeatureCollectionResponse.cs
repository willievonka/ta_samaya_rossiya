using System.Text.Json.Serialization;
using Application.Services.Dtos;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapLayersFeatureCollectionResponse(
    IReadOnlyList<MapLayerResponse> Features
)
{
    [JsonPropertyOrder(-1)]
    public string Type => "FeatureCollection";
}