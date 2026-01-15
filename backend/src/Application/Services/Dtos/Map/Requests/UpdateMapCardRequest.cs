namespace Application.Services.Dtos.Map.Requests;

public record UpdateMapCardRequest(
    bool? IsAnalytics, 
    string? Title,
    string? Description,
    IFormFile? BackgroundImage
    );