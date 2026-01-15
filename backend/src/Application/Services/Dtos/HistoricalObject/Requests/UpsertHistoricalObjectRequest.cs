using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Dtos.HistoricalObject.Requests;

public record UpsertHistoricalObjectRequest(
    Guid? Id,
    string Title,
    double[]? Coordinates,
    int Year,
    [FromForm] IFormFile? Image,
    string Description,
    string? ExcursionUrl
    );