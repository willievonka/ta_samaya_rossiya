namespace Application.Services.Interfaces;

public interface IAdminSeederService
{
    Task SeedIfNotExistAsync();
}