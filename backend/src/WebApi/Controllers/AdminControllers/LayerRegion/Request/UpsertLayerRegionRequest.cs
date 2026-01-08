using System.ComponentModel.DataAnnotations;
using WebApi.Controllers.AdminControllers.HistoricalObject.Request;
using WebApi.Controllers.AdminControllers.LayerRegionStyle.Request;
using WebApi.Controllers.AdminControllers.Map.Requests;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Request;

public record UpsertLayerRegionRequest(
    Guid? Id,
    string? RegionName,
    bool? IsActive,
    UpsertLayerRegionStyleRequest? Style = null,
    UpsertIndicatorsRequest? AnalyticsData = null,
    List<UpsertHistoricalObjectRequest>? Points = null
    );