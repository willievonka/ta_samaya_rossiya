namespace Application.Services.Interfaces;

public interface ICacheService
{
    Task<string?> GetCachedResponseAsync(string key, CancellationToken ct = default);
    Task SetCachedResponseAsync(string key, string json, TimeSpan? expiration = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}