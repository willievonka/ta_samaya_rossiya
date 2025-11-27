namespace Application.Services.Interfaces;

public interface IRegionSeederService
{
    Task SeedIfEmptyAsync(CancellationToken ct = default); 
}