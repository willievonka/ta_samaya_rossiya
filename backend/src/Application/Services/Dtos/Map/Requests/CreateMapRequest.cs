using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Dtos.Map.Requests;

public record CreateMapRequest(
    bool? IsAnalytics,
    [Required] string Title,
    [Required] string Description,
    [Required] string InfoText,
    string? ActiveLayerColor,
    string? PointColor,
    [FromForm] IFormFile? BackgroundImage
    );