using Application.Services.Interfaces;
using Infrastructure.Services.Implementations;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrasctructureServices(this IServiceCollection services)
    {
        services.AddScoped<IQueryService, QueryService>();
        services.AddScoped<ICrudService, CrudService>();
        services.AddScoped<IRegionSeederService, RegionSeederService>();
        services.AddScoped<ISaveImageService, SaveImageService>();
        return services;
    }
}