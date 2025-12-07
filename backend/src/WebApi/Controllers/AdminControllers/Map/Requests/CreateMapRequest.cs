using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.Map.Requests;

public record CreateMapRequest(
    bool? IsAnalytics,
    [Required] string Title,
    [Required] string Description,
    [FromForm] IFormFile? BackgroundImage
    );