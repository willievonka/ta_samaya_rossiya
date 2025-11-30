using Application.Services.Interfaces;
using Application.Services.Logic.Implementations;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImageService, ImageService>();
        
        return services;
    }
}