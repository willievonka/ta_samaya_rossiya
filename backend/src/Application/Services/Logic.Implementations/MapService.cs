using Application.Queries;
using Application.Services.Dtos;
using Application.Services.Interfaces;
using Domain.Entities;

namespace Application.Services.Logic.Implementations;

public class MapService : IMapService
{
    private readonly ILogger<IMapService> _logger;
    private readonly ICrudService _crudService;
    private readonly IQueryService _queryService;
    private readonly IImageService _imageService;

    public MapService(ILogger<IMapService> logger, ICrudService crudService, IQueryService queryService,
        IImageService imageService)
    {
        _logger = logger;
        _crudService = crudService;
        _queryService = queryService;
        _imageService = imageService;
    }
    
    public async Task<Guid> CreateMapAsync(MapDto mapDto, CancellationToken ct)
    {
        _logger.LogInformation("Creating map");
        
        var map = new Map
        {
            Title = mapDto.Title,
            Description = mapDto.Description,
            IsAnalitics = mapDto.IsAnalitics,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        var id = await _crudService.CreateAsync(map, ct);

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
            await _crudService.UpdateAsync(map, ct);
        }
        
        _logger.LogInformation("Map {id} created", id);
        
        return id;
    }

    public async Task<bool> DeleteMapAsync(Guid mapId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting map {mapId}", mapId);
        
        var res = await _crudService.TryRemoveAsync<Map>(mapId, ct);
        if (res)
        {
            _logger.LogInformation("Map {mapId} deleted", mapId);
            return true;
        }
        
        _logger.LogInformation("Map {mapId} could not be deleted", mapId);
        return false;
    }

    public async Task<MapDto?> GetMapAsync(Guid mapId, CancellationToken ct)
    {
        var map = await _crudService.GetByIdOrDefaultAsync(mapId, new IncludeParams<Map>
        {
            IncludeProperties = [m => m.Regions, m => m.Regions.Select(r => r.Indicators)]
        }, ct);

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
        var map = await _crudService.GetByIdOrDefaultAsync<Map>(mapDto.Id!.Value, ct);

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

        map.BackgroundImage = fileUri;
        map.IsAnalitics = mapDto.IsAnalitics;
        map.Title = mapDto.Title;
        map.Description = mapDto.Description;
        map.UpdatedAt = DateTime.Now;
        
        var mapId = await _crudService.UpdateAsync(map, ct);
        
        return mapId;
    }

    public async Task<Guid> AddNewLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct)
    {
        _logger.LogInformation("Adding layer region {name}", layerRegionDto.Name);
        
        var map = await _crudService.GetByIdOrDefaultAsync<Map>(mapId, ct);

        if (map == null)
        {
            _logger.LogError("Map {mapId} could not be found", mapId);
            return Guid.Empty;
        }

        var region = (await _queryService.GetAsync(new DataQueryParams<Region>
        {
            Expression = r => r.Name.Equals(layerRegionDto.Name)
        }, ct)).FirstOrDefault();

        if (region == null)
        {
            _logger.LogError("Invalid name region {name}", layerRegionDto.Name);
            return Guid.Empty;
        }
        
        var newLayerRegion = new LayerRegion
        {
            FillColor = layerRegionDto.FillColor,
            IsActive = layerRegionDto.IsActive,
            Region = region,
            Map = map
        };
        var id = await _crudService.CreateAsync(newLayerRegion, ct);
        
        if (id == Guid.Empty)
        {
            _logger.LogError("Unable to create layer region {name}", layerRegionDto.Name);
            return Guid.Empty;
        }
        
        var indicatorsDto = layerRegionDto.Indicators;

        if (indicatorsDto != null)
        {
            var indicators = new IndicatorsRegion();
            
            if (indicatorsDto.Image != null)
            {
                string? fileUri = null;
                fileUri = await _imageService.SaveImageAsync(id, "Map", indicatorsDto.Image);
                indicators.ImagePath = fileUri;
            }

            indicators.IsActive = indicatorsDto.IsActive;
            indicators.Excursions = indicatorsDto.Excursions;
            indicators.Participants = indicatorsDto.Participants;
            indicators.Partners = indicatorsDto.Partners;
            
            newLayerRegion.Indicators = indicators;
            
            await _crudService.UpdateAsync(newLayerRegion, ct);
        }
        
        _logger.LogInformation("Layer region {name} created", layerRegionDto.Name);
        return id;
    }
}