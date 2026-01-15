namespace Application.Services.Dtos.LayerRegion.Requests;

public record UpdateAnalyticsDataRegionRequest(
    UpsertIndicatorsRequest? AnalyticsData = null
    );