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

    /// <summary>
    /// Получает только свойства карт, без остальных includes
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<MapDto>> GetAllCardsASync(CancellationToken ct)
    {
        var cards = await _mapRepository.GetAllHeadersAsync(ct);

        var list = new List<MapDto>();
        
        if (cards == null)
        {
            _logger.LogError("Cards of maps is null, bug in MapRepository");
            return list;
        }

        var sortedCards = cards.OrderByDescending(c => c.IsAnalytics)
                                                    .ThenBy(c => c.CreatedAt);
        
        foreach (var card in sortedCards)
        {
            list.Add(new MapDto
            {
                Id = card.Id,
                Title = card.Title,
                Description = card.Description,
                Info = card.Info,
                IsAnalytics = card.IsAnalytics,
                BackgroundImagePath = card.BackgroundImage
            });
        }
        
        return list;
    }

    /// <summary>
    /// Создаёт карту, также заполняет её пустыми слоями регионов. (нужно для отображения неактивных на карте)
    /// </summary>
    /// <param name="mapDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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
            Title = mapDto.Title!,
            Description = mapDto.Description ?? "",
            Info = mapDto.Info ?? "",
            IsAnalytics = mapDto.IsAnalytics,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            ActiveLayerRegionsColor = mapDto.ActiveLayerRegionsColor,
            HistoricalObjectPointColor = mapDto.HistoricalObjectPointColor,
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
            _logger.LogInformation("Starting save Map image");
            var fileUri = await _imageService.SaveImageAsync(id, FilePath, mapDto.BackgroundImage);
            map.BackgroundImage = fileUri;
            await _mapRepository.UpdateAsync(map, ct);
        }
        else _logger.LogError("Image is null");
        
        _logger.LogInformation("Map {id} created", id);
        
        await _layerRegionService.CreateAllEmptyRegionsForMap(id, ct);
        
        return id;
    }

    /// <summary>
    /// Удаляет карту по её Id
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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
            _logger.LogInformation("Starting delete Map image");
            await _imageService.DeleteImageAsync(map.BackgroundImage);
        }
        
        var regionsIds = await _layerRegionService.GetAllIdsByMapIdAsync(mapId, ct);
        
        foreach (var id in regionsIds)
        {
            await _layerRegionService.DeleteLayerRegionAsync(id, map.Id, ct);
        }
        
        await _mapRepository.DeleteByIdAsync(mapId, ct);
       
        _logger.LogInformation("Map {mapId} deleted", mapId);
        return true;
    }

    /// <summary>
    /// Получает полную модель карты со всеми вытекающими(регионами, стилями, показателями), полный include
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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
            Info = map.Info,
            IsAnalytics = map.IsAnalytics,
            BackgroundImagePath = map.BackgroundImage,
            ActiveLayerRegionsColor = map.ActiveLayerRegionsColor,
            HistoricalObjectPointColor = map.HistoricalObjectPointColor,
        };
        
        mapDto.Regions = await _layerRegionService.GetAllByMapIdAsync(mapId, ct);
        
        return mapDto;
    }

    /// <summary>
    /// Получает MapDto с ещё не созданными LayerRegions, которые формируются из базовых Regions
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<MapDto> GetEmptyMapAsync(CancellationToken ct)
    {
        var regions = await _layerRegionService.GetAllBasicRegionsAsync(ct);

        var mapDto = new MapDto
        {
            Title = string.Empty,
            Info = string.Empty,
            Regions = regions
        };
        
        return mapDto;
    }

    /// <summary>
    /// Обновляет карту, обновятся только не null значения. Остальные свойства сохраняться прежними.
    /// Принимается полный Snapshot карты, недостающие элементы удаляются, новые добавляются, старые обновляются
    /// </summary>
    /// <param name="mapDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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
        
        if (mapDto.BackgroundImage != null)
        { 
            _logger.LogInformation("Starting update Map image");
            var fileUri = await _imageService.UpdateImageAsync(map.Id, map.BackgroundImage, FilePath,
                mapDto.BackgroundImage);
            
            map.BackgroundImage = fileUri;
        }
        else if (map.BackgroundImage != null)
        { 
            await _imageService.DeleteImageAsync(map.BackgroundImage);
            map.BackgroundImage = null;
            _logger.LogInformation("Image {path} deleted", map.BackgroundImage);
        }
        
        if (mapDto.Title != null) map.Title = mapDto.Title;
        if (mapDto.Description != null) map.Description = mapDto.Description;
        if (mapDto.IsAnalytics != null) map.IsAnalytics = mapDto.IsAnalytics;
        if (mapDto.ActiveLayerRegionsColor != null) map.ActiveLayerRegionsColor = mapDto.ActiveLayerRegionsColor;
        if (mapDto.HistoricalObjectPointColor != null) map.HistoricalObjectPointColor = mapDto.HistoricalObjectPointColor;
        if (mapDto.Info != null) map.Info = mapDto.Info;

        map.UpdatedAt = DateTime.Now;
        
        var idsForDeleting = await _layerRegionService.GetAllIdsByMapIdAsync(map.Id, ct);
        
        if (mapDto.Regions != null)
        {
            _logger.LogInformation("Starting update Map LayerRegions.");
            
            var idsForUpdating = mapDto.Regions.Select(x => x.Id!.Value).ToList();
            idsForDeleting = idsForDeleting.Except(idsForUpdating).ToList();
            
            foreach (var regionDto in mapDto.Regions)
            {
                var layerRegionId = await _layerRegionService.UpdateLayerRegionAsync(regionDto.Id!.Value, regionDto, ct);

                if (layerRegionId == Guid.Empty)
                {
                    await _layerRegionService.CreateLayerRegionAsync(map.Id, regionDto, ct);
                }
            }
        }
        
        foreach (var id in idsForDeleting)
        {
            await _layerRegionService.DeleteLayerRegionAsync(id, map.Id, ct);
        }
        
        await _mapRepository.UpdateAsync(map, ct);
        
        return map.Id;
    }

    /// <summary>
    /// Добавляет новый слой региона со стилями и показателями.
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="layerRegionDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
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