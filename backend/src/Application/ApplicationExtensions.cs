using Application.Services.Logic.Implementations;
using Application.Services.Logic.Interfaces;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IIndicatorsService, IndicatorsService>();
        services.AddScoped<ILayerRegionStyleService, LayerRegionStyleService>();
        services.AddScoped<ILayerRegionService, LayerRegionService>();
        services.AddScoped<IMapService, MapService>();
        services.AddScoped<IHistoricalObjectService, HistoricalObjectService>();
        return services;
    }
}