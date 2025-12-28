using Domain.Entities;

namespace Application.Services.Auth.Interfaces;

public interface IAuthService
{
    Guid? GetCurrentUserId();
    string GenerateJwtToken(Guid id, string email);
    void Logout();
}