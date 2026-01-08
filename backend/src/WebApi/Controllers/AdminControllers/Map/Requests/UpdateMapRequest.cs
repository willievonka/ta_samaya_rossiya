using WebApi.Controllers.AdminControllers.LayerRegion.Request;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

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