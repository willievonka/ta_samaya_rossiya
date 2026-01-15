namespace Application.Services.Dtos.HistoricalObject.Responses;

public record HistoricalObjectResponse(
    Guid Id,
    string Title,
    double[] Coordinates,
    int Year,
    string ImagePath,
    string Description,
    string? ExcursionUrl
    );