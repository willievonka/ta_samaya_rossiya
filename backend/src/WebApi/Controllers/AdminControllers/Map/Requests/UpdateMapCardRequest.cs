using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record UpdateMapCardRequest(
    bool? IsAnalytics, 
    [Required] string Title,
    [Required] string Description,
    IFormFile? BackgroundImage
    );