using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.HistoricalObject.Request;

public record CreateHistoricalObjectRequest(
    string Title,
    double[] Coordinates,
    int Year,
    [FromForm] IFormFile Image,
    string Description,
    string? ExcursionUrl
    );