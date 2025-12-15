using System.ComponentModel.DataAnnotations;
using WebApi.Controllers.AdminControllers.LayerRegionStyle.Request;
using WebApi.Controllers.AdminControllers.Map.Requests;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Request;

public record UpdateLayerRegionRequest(
    bool? IsActive,
    UpsertLayerRegionStyleRequest? Style = null,
    UpsertIndicatorsRequest? AnalyticsData = null
    );