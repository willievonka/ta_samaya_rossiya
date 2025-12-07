namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapCardResponse(
    Guid Id,
    bool? IsAnalytics,
    string Title, 
    string Description, 
    string? BackgroundImagePath
    );