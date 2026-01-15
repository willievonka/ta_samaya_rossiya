using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Dtos.HistoricalObject.Requests;

public record UpdateHistoricalObjectRequest(
    string? Title,
    int? Year,
    [FromForm] IFormFile? Image,
    string? Description,
    string? ExcursionUrl
    );