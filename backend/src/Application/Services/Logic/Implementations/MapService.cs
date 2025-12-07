using Application.Services.Dtos;
using Application.Services.Logic.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class MapService : IMapService
{
    private readonly ILogger<IMapService> _logger;
    private readonly IMapRepository _mapRepository;
    private readonly IImageService _imageService;
    private readonly ILayerRegionService _layerRegionService;


    private const string FilePath = "map-cards"; 
    
    public MapService(ILogger<IMapService> logger, IMapRepository mapRepository, 
        IImageService imageService, ILayerRegionService layerRegionService)
    {
        _logger = logger;
        _mapRepository = mapRepository;
        _imageService = imageService;
        _layerRegionService = layerRegionService;
    }

    public async Task<List<MapDto>> GetAllCardsASync(CancellationToken ct)
    {
        var cards = await _mapRepository.GetAllHeadersAsync(ct);

        var list = new List<MapDto>();
        
        if (cards == null)
        {
            _logger.LogError("Cards of maps is null, bug in MapRepository");
            return list;
        }

        foreach (var card in cards)
        {
            list.Add(new MapDto
            {
                Id = card.Id,
                Title = card.Title,
                Description = card.Description,
                IsAnalytics = card.IsAnalytics,
                BackgroundImagePath = card.BackgroundImage
            });
        }
        
        return list.OrderByDescending(m => m.IsAnalytics).ToList();
    }

    public async Task<Guid> CreateMapAsync(MapDto? mapDto, CancellationToken ct)
    {
        if (mapDto is null)
        {
            _logger.LogError("MapDto is null");
            return Guid.Empty;
        }
        
        _logger.LogInformation("Creating map");
        
        var map = new Map
        {
            Title = mapDto.Title,
            Description = mapDto.Description,
            IsAnalytics = mapDto.IsAnalytics,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        await _mapRepository.AddAsync(map, ct);
        
        var id = map.Id;
        if (id == Guid.Empty)
        {
            _logger.LogError("Unable to create map");
            return Guid.Empty;
        }
        
        if (mapDto.BackgroundImage != null)
        { 
            string? fileUri = null;
            fileUri = await _imageService.SaveImageAsync(id, FilePath, mapDto.BackgroundImage);
            map.BackgroundImage = fileUri;
            await _mapRepository.UpdateAsync(map, ct);
        }
        
        _logger.LogInformation("Map {id} created", id);
        
        await _layerRegionService.CreateAllEmptyRegionsForMap(id, ct);
        
        return id;
    }

    public async Task<bool> DeleteMapAsync(Guid mapId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting map {mapId}", mapId);
        
        var map = await _mapRepository.GetHeaderByIdAsync(mapId, ct);
        if (map == null)
        {
            _logger.LogInformation("Map {mapId} could not be deleted", mapId);
            return false;
        }
        
        if (map.BackgroundImage != null)
        { 
            await _imageService.DeleteImageAsync(map.BackgroundImage);
        }
        
        await _mapRepository.DeleteByIdAsync(mapId, ct);
       
        _logger.LogInformation("Map {mapId} deleted", mapId);
        return true;
    }

    public async Task<MapDto?> GetMapAsync(Guid mapId, CancellationToken ct)
    {
        var map = await _mapRepository.GetHeaderByIdAsync(mapId, ct);

        if (map == null)
        {
            _logger.LogError("Map {mapId} could not be found", mapId);
            return null;
        }

        var mapDto = new MapDto
        {
            Id = map.Id,
            Title = map.Title,
            Description = map.Description,
            IsAnalytics = map.IsAnalytics,
            BackgroundImagePath = map.BackgroundImage,
        };
        
        mapDto.Regions = await _layerRegionService.GetAllByMapIdAsync(mapId, ct);;
        
        return mapDto;
    }

    public async Task<Guid> UpdateMapAsync(MapDto? mapDto, CancellationToken ct)
    {
        if (mapDto == null)
        {
            _logger.LogError("MapDto is null");
            return Guid.Empty;
        }
        
        var map = await _mapRepository.GetHeaderByIdAsync(mapDto.Id!.Value, ct);

        if (map == null)
        {
            _logger.LogError("Map {mapId} could not be found", mapDto.Id);
            return Guid.Empty;
        }

        string? fileUri = null;
        if (mapDto.BackgroundImage != null)
        { 
            fileUri = await _imageService.UpdateImageAsync(map.Id, map.BackgroundImage, FilePath,
                mapDto.BackgroundImage);
        }
        
        map.BackgroundImage = fileUri;
        map.IsAnalytics = mapDto.IsAnalytics;
        map.Title = mapDto.Title;
        map.Description = mapDto.Description;
        map.UpdatedAt = DateTime.Now;

        if (mapDto.Regions != null)
        {
            foreach (var regionDto in mapDto.Regions)
            {
                await _layerRegionService.UpdateLayerRegionAsync(regionDto.Id!.Value, regionDto, ct);
            }
        }
        
        await _mapRepository.UpdateAsync(map, ct);
        
        return map.Id;
    }

    public async Task<Guid> AddNewLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct)
    {
        _logger.LogInformation("Adding layer region {name}", layerRegionDto.Name);
        
        var map = await _mapRepository.GetHeaderByIdAsync(mapId, ct);

        if (map == null)
        {
            _logger.LogError("Map {mapId} could not be found", mapId);
            return Guid.Empty;
        }
        
        var layerRegionId = await _layerRegionService.CreateLayerRegionAsync(map.Id, layerRegionDto, ct);
        
        _logger.LogInformation("Layer region {name} created", layerRegionDto.Name);
        return layerRegionId;
    }
}