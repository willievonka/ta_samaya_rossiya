using Domain.Entities;

namespace Application.Services.Auth.Interfaces;

public interface IAdminManager
{
    Task CreateAsync(string email, string password);
    Task<Guid?> ValidateAdminCredentialsAsync(string email, string password);
}