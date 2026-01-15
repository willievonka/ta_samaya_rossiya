using Application.Services.Auth.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services.Implementations.Auth;

public class BlacklistService : IBlacklistService
{
    private readonly IDistributedCache _cache;
    private const string Prefix = "bl_";
    
    public BlacklistService(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    public async Task AddTokenToBlackListAsync(string jti, TimeSpan lifetime, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime
        };
        
        await _cache.SetStringAsync($"{Prefix}{jti}", "1", options, ct);
    }

    public async Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken ct = default)
    {
        var result = await _cache.GetStringAsync($"{Prefix}{jti}", ct);
        return result != null;
    }
}