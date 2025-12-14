namespace WebApi.Controllers.AdminControllers.HistoricalObject.Response;

public record HistoricalObjectResponse(
    Guid Id,
    string Title,
    double[] Coordinates,
    int Year,
    string ImagePath,
    string Description,
    string? ExcursionUrl
    );