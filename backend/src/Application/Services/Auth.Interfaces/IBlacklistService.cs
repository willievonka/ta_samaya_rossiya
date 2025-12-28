namespace Application.Services.Auth.Interfaces;

public interface IBlacklistService
{
    void AddTokenToBlackList(string jti, TimeSpan lifetime);
    bool IsTokenBlacklisted(string jti);
}