using System.Security.Claims;

namespace Application.Services.Auth.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(Guid id, string email);
    string GenerateRefreshToken();
    
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}