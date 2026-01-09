using Application.Services.Dtos;

namespace Application.Services.Auth.Interfaces;

public interface IAuthService
{
    Guid? GetCurrentUserId();
    Task<AuthTokensDto?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<Guid?> ValidateAdminCredentialsAsync(string email, string password);
    Task<AuthTokensDto?> RefreshAsync(string accessToken, string refreshToken, CancellationToken ct);
    Task<bool> LogoutAsync(Guid adminId, CancellationToken ct);
}