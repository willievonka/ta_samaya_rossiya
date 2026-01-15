namespace Application.Services.Dtos.Map.Responses;

public record MapCardResponse(
    Guid Id,
    bool? IsAnalytics,
    string Title, 
    string Description, 
    string? BackgroundImagePath
    );