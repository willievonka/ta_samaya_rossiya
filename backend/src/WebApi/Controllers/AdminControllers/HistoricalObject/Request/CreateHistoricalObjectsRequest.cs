namespace WebApi.Controllers.AdminControllers.HistoricalObject.Request;

public record CreateHistoricalObjectsRequest(
    List<CreateHistoricalObjectWithIdRequest> Points
    );