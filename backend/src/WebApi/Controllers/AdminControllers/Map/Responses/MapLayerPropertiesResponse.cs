namespace WebApi.Controllers.AdminControllers.Map.Responses;

public sealed record MapLayerPropertiesResponse(
    Guid Id,
    string RegionName,
    bool? IsActive = null,
    LayerRegionStyleResponse? Style = null,
    AnalyticsMapLayerPropertiesResponse? AnalyticsData = null
);