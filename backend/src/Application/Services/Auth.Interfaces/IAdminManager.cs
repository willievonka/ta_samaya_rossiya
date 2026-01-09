namespace Application.Services.Auth.Interfaces;

public interface IAdminManager
{
    Task CreateAsync(string email, string password);
}