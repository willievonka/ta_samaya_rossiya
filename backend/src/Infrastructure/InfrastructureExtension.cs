using Application.Services.Interfaces;
using Infrastructure.Services.Implementations;

namespace Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IQueryService, QueryService>();
        services.AddScoped<ICrudService, CrudService>();
        services.AddScoped<IRegionSeederService, RegionSeederService>();
        return services;
    }
}