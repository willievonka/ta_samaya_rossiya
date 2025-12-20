using Application.Services.Auth.Interfaces;
using Application.Services.Interfaces;
using Domain.Repository.Interfaces;
using Infrastructure.Repository.Implementations;
using Infrastructure.Services.Implementations;
using Infrastructure.Services.Implementations.Auth;

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
        services.AddScoped<ILayerRegionStyleRepository, LayerRegionStyleRepository>();
        services.AddScoped<IHistoricalObjectRepository, HistoricalObjectRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        
        services.AddScoped<IRegionSeederService, RegionSeederService>();
        services.AddScoped<ISaveImageService, SaveImageService>();
        services.AddScoped<IAdminSeederService, AdminSeederService>();
        
        services.AddScoped<IAdminManager, AdminManager>();
        
        return services;
    }
}