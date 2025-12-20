using Application.Services.Auth.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services.Implementations.Auth;

public class BlacklistService : IBlacklistService
{
    private readonly IMemoryCache _cache;
    
    public BlacklistService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public void AddTokenToBlackList(string jti, TimeSpan lifetime)
    {
        _cache.Set($"bl_{jti}", true, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime,
            Priority = CacheItemPriority.NeverRemove
        });
    }

    public bool IsTokenBlacklisted(string jti)
    {
        return _cache.TryGetValue(jti, out _);
    }
}