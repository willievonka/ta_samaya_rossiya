using WebApi.Controllers.AdminControllers.HistoricalObject.Response;
using WebApi.Controllers.AdminControllers.LayerRegionStyle.Response;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Response;

public sealed record MapLayerPropertiesResponse(
    Guid Id,
    string RegionName,
    bool? IsActive = null,
    LayerRegionStyleResponse? Style = null,
    AnalyticsMapLayerPropertiesResponse? AnalyticsData = null,
    IReadOnlyList<HistoricalObjectResponse>? Points = null
);