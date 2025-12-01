using Application.Services.Interfaces;
using Domain.Repository.Interfaces;
using Infrastructure.Repository.Implementations;
using Infrastructure.Services.Implementations;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrasctructureServices(this IServiceCollection services)
    {
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IRegionGeometryRepository, RegionGeometryRepository>();
        services.AddScoped<IMapRepository, MapRepository>();
        services.AddScoped<ILayerRegionRepository, LayerRegionRepository>();
        services.AddScoped<IIndicatorsRepository, IndicatorsRepository>();
        services.AddScoped<IHistoricalObjectRepository, HistoricalObjectRepository>();
        services.AddScoped<IHistoricalLineRepository, HistoricalLineRepository>();
        
        services.AddScoped<IRegionSeederService, RegionSeederService>();
        services.AddScoped<ISaveImageService, SaveImageService>();
        return services;
    }
}