using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.AdminControllers.Requests;

public record CreateMapRequest(
    [Required] bool IsAnalitics,
    [Required] string Title,
    [Required] string Description,
    [Required] IFormFile? BackgroundImage
    );