using Application.Services.Dtos;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapLayersFeatureCollectionResponse(
    IReadOnlyList<MapLayerResponse> Features
)
{
    public string Type => "FeatureCollection";
}