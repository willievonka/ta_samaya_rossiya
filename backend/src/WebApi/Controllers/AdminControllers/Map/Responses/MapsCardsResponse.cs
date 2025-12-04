
namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapsCardsResponse(
    IEnumerable<MapCardResponse>? Cards
    );