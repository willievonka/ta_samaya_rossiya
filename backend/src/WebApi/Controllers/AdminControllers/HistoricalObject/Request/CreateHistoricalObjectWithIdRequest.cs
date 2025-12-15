using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.HistoricalObject.Request;

public record CreateHistoricalObjectWithIdRequest(
    Guid LayerId,
    string Title,
    double[] Coordinates,
    int Year,
    [FromForm] IFormFile Image,
    string Description,
    string? ExcursionUrl
    ) : CreateHistoricalObjectRequest(
    Title,
    Coordinates,
    Year,
    Image,
    Description,
    ExcursionUrl
    );