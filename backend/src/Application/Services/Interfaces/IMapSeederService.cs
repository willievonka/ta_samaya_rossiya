namespace Application.Services.Interfaces;

public interface IMapSeederService
{
    Task SeedAnalyticsMapIfEmptyAsync();
}