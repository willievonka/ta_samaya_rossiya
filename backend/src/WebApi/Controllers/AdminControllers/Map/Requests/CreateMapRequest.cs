using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record CreateMapRequest(
    [Required] bool IsAnalitics,
    [Required] string Title,
    [Required] string Description,
    [FromForm] IFormFile? BackgroundImage
    );