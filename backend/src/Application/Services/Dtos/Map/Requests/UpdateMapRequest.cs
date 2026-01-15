using Application.Services.Dtos.LayerRegion.Requests;

namespace Application.Services.Dtos.Map.Requests;

public record UpdateMapRequest(
    bool? IsAnalytics, 
    string? Title,
    string? Description,
    string? InfoText,
    string? ActiveLayerColor,
    string? PointColor,
    IFormFile? BackgroundImage,
    IEnumerable<UpsertLayerRegionRequest>? Layers
    );