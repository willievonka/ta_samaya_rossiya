using System.ComponentModel.DataAnnotations;
using Application.Services.Dtos.HistoricalObject.Requests;
using Application.Services.Dtos.LayerRegionStyle.Requests;

namespace Application.Services.Dtos.LayerRegion.Requests;

public record CreateLayerRegionRequest(
    [Required] string RegionName,
    bool? IsActive,
    UpsertLayerRegionStyleRequest? Style = null,
    UpsertIndicatorsRequest? AnalyticsData = null,
    List<UpsertHistoricalObjectRequest>? Points = null
);