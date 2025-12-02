namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record AnalyticsMapLayerPropertiesResponse(
    string ImagePath,
    int PartnersCount,
    int ExcursionsCount,
    int MembersCount
    );