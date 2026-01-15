using Application.Services.Dtos.HistoricalObject.Responses;
using Application.Services.Dtos.LayerRegionStyle.Responses;

namespace Application.Services.Dtos.LayerRegion.Responses;

public sealed record MapLayerPropertiesResponse(
    Guid Id,
    string RegionName,
    bool? IsActive = null,
    LayerRegionStyleResponse? Style = null,
    AnalyticsMapLayerPropertiesResponse? AnalyticsData = null,
    IReadOnlyList<HistoricalObjectResponse>? Points = null
);