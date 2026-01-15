using Application.Services.Dtos.HistoricalObject.Requests;
using Application.Services.Dtos.LayerRegionStyle.Requests;

namespace Application.Services.Dtos.LayerRegion.Requests;

public record UpsertLayerRegionRequest(
    Guid? Id,
    string? RegionName,
    bool? IsActive,
    UpsertLayerRegionStyleRequest? Style = null,
    UpsertIndicatorsRequest? AnalyticsData = null,
    List<UpsertHistoricalObjectRequest>? Points = null
    );