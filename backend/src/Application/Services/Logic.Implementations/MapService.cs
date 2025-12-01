using Application.Queries;
using Application.Services.Dtos;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class MapService : IMapService
{
    private readonly ILogger<IMapService> _logger;
    private readonly IMapRepository _mapRepository;
    private readonly ILayerRegionRepository _layerRegionRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IImageService _imageService;

    public MapService(ILogger<IMapService> logger, IMapRepository mapRepository,
        ILayerRegionRepository layerRegionRepository, IImageService imageService,
        IRegionRepository regionRepository)
    {
        _logger = logger;
        _regionRepository = regionRepository;
        _mapRepository = mapRepository;
        _layerRegionRepository = layerRegionRepository;
        _imageService = imageService;
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
            IsAnalitics = mapDto.IsAnalitics,
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
            fileUri = await _imageService.SaveImageAsync(id, "Map", mapDto.BackgroundImage);
            map.BackgroundImage = fileUri;
            await _mapRepository.UpdateAsync(map, ct);
        }
        
        _logger.LogInformation("Map {id} created", id);
        
        return id;
    }

    public async Task<bool> DeleteMapAsync(Guid mapId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting map {mapId}", mapId);
        
        var map = await _mapRepository.GetByIdAsync(mapId, ct);
        if (map == null)
        {
            _logger.LogInformation("Map {mapId} could not be deleted", mapId);
            return false;
        }
        
        await _mapRepository.DeleteByIdAsync(mapId, ct);
       
        _logger.LogInformation("Map {mapId} deleted", mapId);
        return true;
    }

    public async Task<MapDto?> GetMapAsync(Guid mapId, CancellationToken ct)
    {
        var map = await _mapRepository.GetByIdAsync(mapId, ct);

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
            IsAnalitics = map.IsAnalitics,
            BackgroundImagePath = map.BackgroundImage,
        };

        var regions = new List<LayerRegionDto>();
        foreach (var region in map.Regions)
        {
            var indicators = region.Indicators;

            var regionDto = new LayerRegionDto
            {
                Id = region.Id,
                IsActive = region.IsActive,
                FillColor = region.FillColor,
                Name = region.Region.Name,
                IsRussia = region.Region.IsRussia,
                Geometry = region.Region.Geometry.Geometry,
            };

            if (indicators != null)
            {
                regionDto.Indicators = new IndicatorsRegionDto
                {
                    IsActive = indicators.IsActive,
                    Participants = indicators.Participants,
                    Excursions = indicators.Excursions,
                    Partners = indicators.Partners,
                    ImagePath = indicators.ImagePath,
                };
            }

            regions.Add(regionDto);
        }
        
        mapDto.Regions = regions;
        
        return mapDto;
    }

    public async Task<Guid> UpdateMapAsync(MapDto mapDto, CancellationToken ct)
    {
        var map = await _mapRepository.GetByIdAsync(mapDto.Id!.Value, ct);

        if (map == null)
        {
            _logger.LogError("Map {mapId} could not be found", mapDto.Id);
            return Guid.Empty;
        }

        string? fileUri = null;
        if (mapDto.BackgroundImage != null)
        { 
            fileUri = await _imageService.UpdateImageAsync(map.Id, map.BackgroundImage, "Map",
                mapDto.BackgroundImage);
        }
        
        //TODO IndicatorService, LayerRegionService

        map.BackgroundImage = fileUri;
        map.IsAnalitics = mapDto.IsAnalitics;
        map.Title = mapDto.Title;
        map.Description = mapDto.Description;
        map.UpdatedAt = DateTime.Now;
        
        await _mapRepository.UpdateAsync(map, ct);
        
        return map.Id;
    }

    public async Task<Guid> AddNewLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct)
    {
        _logger.LogInformation("Adding layer region {name}", layerRegionDto.Name);
        
        var map = await _mapRepository.GetByIdAsync(mapId, ct);

        if (map == null)
        {
            _logger.LogError("Map {mapId} could not be found", mapId);
            return Guid.Empty;
        }

        var region = await _regionRepository.GetByNameAsync(layerRegionDto.Name, ct);

        if (region == null)
        {
            _logger.LogError("Invalid name region {name}", layerRegionDto.Name);
            return Guid.Empty;
        }
        
        var newLayerRegion = new LayerRegion
        {
            FillColor = layerRegionDto.FillColor,
            IsActive = layerRegionDto.IsActive,
            RegionId = region.Id,
            MapId = mapId
        };
        await _layerRegionRepository.AddAsync(newLayerRegion, ct);
        
        var indicatorsDto = layerRegionDto.Indicators;

        if (indicatorsDto != null)
        {
            var indicators = new IndicatorsRegion();
            
            if (indicatorsDto.Image != null)
            {
                string? fileUri = null;
                fileUri = await _imageService.SaveImageAsync(newLayerRegion.Id, "Map", indicatorsDto.Image);
                indicators.ImagePath = fileUri;
            }

            indicators.IsActive = indicatorsDto.IsActive;
            indicators.Excursions = indicatorsDto.Excursions;
            indicators.Participants = indicatorsDto.Participants;
            indicators.Partners = indicatorsDto.Partners;
            indicators.RegionId = newLayerRegion.Id;
            
            newLayerRegion.Indicators = indicators;
            
            await _layerRegionRepository.UpdateAsync(newLayerRegion, ct);
        }
        
        _logger.LogInformation("Layer region {name} created", layerRegionDto.Name);
        return newLayerRegion.Id;
    }
}