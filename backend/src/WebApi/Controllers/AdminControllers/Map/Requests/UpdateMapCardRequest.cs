using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record UpdateMapCardRequest(
    bool? IsAnalytics, 
    string? Title,
    string? Description,
    IFormFile? BackgroundImage
    );