namespace Application.Services.Interfaces;

public interface IRegionSeederService
{
    Task SeedNewRegionAsync(CancellationToken ct = default); 
}