namespace WebApi.Controllers.AdminControllers.LayerRegion.Response;

public record AnalyticsMapLayerPropertiesResponse(
    string ImagePath,
    int PartnersCount,
    int ExcursionsCount,
    int MembersCount
    );