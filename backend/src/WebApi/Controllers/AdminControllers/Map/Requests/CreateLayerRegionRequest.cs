using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record CreateLayerRegionRequest(
    [Required] string RegionName,
    [Required] bool IsActive,
    CreateLayerRegionStyleRequest? Style = null,
    CreateIndicatorsRequest? AnalyticsData = null
);