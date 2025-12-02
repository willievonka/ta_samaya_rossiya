using Application.Services.Interfaces;
using Application.Services.Logic.Implementations;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IMapService, MapService>();
        return services;
    }
}