using Application.Services.Dtos.Map;
using Application.Services.Interfaces;
using Application.Services.Logic.Interfaces;
using Domain.Repository.Interfaces;

namespace Infrastructure.Services.Implementations;

public class MapSeederService : IMapSeederService
{
    private readonly ILogger<IMapSeederService> _logger;
    private readonly IMapService _mapService;
    private readonly IMapRepository _mapRepository;
    
    public MapSeederService(ILogger<IMapSeederService> logger, IMapService mapService, IMapRepository mapRepository)
    {
        _logger = logger;
        _mapService = mapService;
        _mapRepository = mapRepository;
    }

    public async Task SeedAnalyticsMapIfEmptyAsync()
    {
        if (await _mapRepository.AnalyticsMapExistsAsync())
        {
            _logger.LogInformation("Analytics map is already exists");
            return;
        }

        var mapDto = new MapDto
        {
            Title = "Аналитическая карта России",
            IsAnalytics = true
        };
        
        var result = await _mapService.CreateMapAsync(mapDto, CancellationToken.None);

        if (result == Guid.Empty)
        {
            _logger.LogError("Analytics map creation failed.");
            throw new InvalidOperationException("Analytics map creation failed.");
        }
    }
}