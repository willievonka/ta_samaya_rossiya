namespace Application.Services.Dtos.LayerRegion.Responses;

public record AnalyticsMapLayerPropertiesResponse(
    string ImagePath,
    int PartnersCount,
    int ExcursionsCount,
    int MembersCount
    );