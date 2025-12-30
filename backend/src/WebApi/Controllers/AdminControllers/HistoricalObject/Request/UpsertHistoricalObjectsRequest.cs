namespace WebApi.Controllers.AdminControllers.HistoricalObject.Request;

public record UpsertHistoricalObjectsRequest(
    List<UpsertHistoricalObjectRequest> Points
    );