namespace Application.Services.Logic.Interfaces;

public interface IMapQueryCachingService
{
    Task<string?> GetMapResponseAsync(Guid mapId, CancellationToken ct);
    Task<string?> GetEmptyMapResponseAsync(CancellationToken ct);
    Task<string> GetAllMapsCardsResponseAsync(CancellationToken ct);
    Task RemoveMapResponseCacheAsync(Guid mapId, CancellationToken ct);
    Task RemoveEmptyMapResponseCacheAsync(CancellationToken ct);
    Task RemoveAllMapsCardsResponseCacheAsync(CancellationToken ct);
}