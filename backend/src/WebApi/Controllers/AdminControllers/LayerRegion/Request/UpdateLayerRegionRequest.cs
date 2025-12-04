using System.ComponentModel.DataAnnotations;
using WebApi.Controllers.AdminControllers.Map.Requests;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Request;

public record UpdateLayerRegionRequest(
    [Required] bool IsActive,
    CreateLayerRegionStyleRequest? Style = null,
    CreateIndicatorsRequest? AnalyticsData = null
    );