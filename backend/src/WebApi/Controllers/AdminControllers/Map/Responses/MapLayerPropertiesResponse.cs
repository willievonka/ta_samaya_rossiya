namespace WebApi.Controllers.AdminControllers.Map.Responses;

public sealed record MapLayerPropertiesResponse(
    string RegionName,
    Guid? Id = null,
    bool? IsActive = null,
    PathOptionsResponse? Style = null,
    AnalyticsMapLayerPropertiesResponse? AnalyticsData = null
);