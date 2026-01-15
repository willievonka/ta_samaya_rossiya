namespace Application.Services.Auth.Interfaces;

public interface IBlacklistService
{
    Task AddTokenToBlackListAsync(string jti, TimeSpan lifetime, CancellationToken ct = default);
    Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken ct = default);
}