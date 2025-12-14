using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.HistoricalObject.Request;

public record UpdateHistoricalObjectRequest(
    string? Title,
    int? Year,
    [FromForm] IFormFile? Image,
    string? Description,
    string? ExcursionUrl
    );