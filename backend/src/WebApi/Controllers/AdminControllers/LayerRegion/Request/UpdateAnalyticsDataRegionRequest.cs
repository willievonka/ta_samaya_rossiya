using System.ComponentModel.DataAnnotations;
using WebApi.Controllers.AdminControllers.Map.Requests;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Request;

public record UpdateAnalyticsDataRegionRequest(
    CreateIndicatorsRequest? AnalyticsData = null
    );