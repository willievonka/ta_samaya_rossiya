using Application.Services.Dtos;
using Application.Services.Logic.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class LayerRegionService : ILayerRegionService
{
    private readonly ILogger<ILayerRegionService> _logger;
    private readonly ILayerRegionRepository _layerRegionRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IIndicatorsService _indicatorsService; 
    private readonly ILayerRegionStyleService _layerRegionStyleService;
    private readonly IHistoricalObjectService _historicalObjectService;

    public LayerRegionService(ILayerRegionRepository layerRegionRepository,
        IRegionRepository regionRepository, ILogger<ILayerRegionService> logger, 
        IIndicatorsService indicatorsService, ILayerRegionStyleService layerRegionStyleService,
        IHistoricalObjectService historicalObjectService)
    {
        _layerRegionRepository = layerRegionRepository;
        _regionRepository = regionRepository;
        _logger = logger;
        _indicatorsService = indicatorsService;
        _layerRegionStyleService = layerRegionStyleService;
        _historicalObjectService = historicalObjectService;
    }

    /// <summary>
    /// Активирует/дезактивирует слой региона по его Id
    /// </summary>
    /// <param name="layerRegionId">Id слоя региона</param>
    /// <param name="ct"></param>
    public async Task SwitchRegionActivation(Guid layerRegionId, CancellationToken ct)
    {
        var layerRegion = await _layerRegionRepository.GetHeaderByIdAsync(layerRegionId, ct);

        if (layerRegion == null)
        {
            _logger.LogError("Invalid region id {regionId}", layerRegionId);
            return;
        }
        
        if (layerRegion.IsActive) 
            layerRegion.IsActive = false;
        else 
            layerRegion.IsActive = true;

        await _layerRegionRepository.UpdateAsync(layerRegion, ct);
    }
    
    /// <summary>
    /// Заполнить только что созданную карту неактивными базовыми регионами
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="ct"></param>
    public async Task CreateAllEmptyRegionsForMap(Guid mapId, CancellationToken ct)
    {
        _logger.LogInformation("Filling map {id} with inactive all regions from DataBase", mapId);
        
        var regions = await _regionRepository.GetAllAsync(ct);

        if (regions == null)
        {
            _logger.LogError("There are no base regions in the database");
            return;
        }
        
        foreach (var region in regions)
        {
            await _layerRegionRepository.AddAsync(new LayerRegion
            {
                IsActive = false,
                RegionId = region.Id,
                MapId = mapId
            }, ct);
        }
        
        _logger.LogInformation("Finish filling map {id}", mapId);
    }
    
    /// <summary>
    /// Создаёт слой региона со стилями и показателями (без historicalObject)
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="layerRegionDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Guid> CreateLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct)
    {
        var region = await _regionRepository.GetByNameAsync(layerRegionDto.Name, ct);

        if (region == null)
        {
            _logger.LogError("Invalid name region {name}", layerRegionDto.Name);
            return Guid.Empty;
        }
        
        var newLayerRegion = new LayerRegion
        {
            RegionId = region.Id,
            MapId = mapId,
        };

        if (layerRegionDto.IsActive != null)
        {
            newLayerRegion.IsActive = layerRegionDto.IsActive.Value;
        }
        
        await _layerRegionRepository.AddAsync(newLayerRegion, ct);

        if (layerRegionDto.Indicators != null)
        {
            await _indicatorsService.CreateIndicatorsAsync(newLayerRegion.Id, layerRegionDto.Indicators, ct);
        }

        if (layerRegionDto.Style != null)
        {
            await _layerRegionStyleService.AddAsync(newLayerRegion.Id, layerRegionDto.Style, ct);
        }
        
        return newLayerRegion.Id;
    }

    /// <summary>
    /// Получает все слои регионов по Id карты 
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<LayerRegionDto>?> GetAllByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        var regions = await _layerRegionRepository.GetAllWithRegionAndGeometryByMapIdAsync(mapId, ct);
        
        if (regions == null)
        {
            _logger.LogError("Invalid map id {mapId}", mapId);
            return new List<LayerRegionDto>();
        }

        var regionsDtos = await GetLayerRegionsDtos(regions, ct);

        return regionsDtos;
    }

    /// <summary>
    /// Получает список активных слоёв региона по id карты
    /// </summary>
    /// <param name="mapId">id карты</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<LayerRegionDto>?> GetAllActiveByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        var regions = await _layerRegionRepository.GetAllActiveWithRegionAndGeometryByMapAsync(mapId, ct);
        
        if (regions == null)
        {
            _logger.LogError("Invalid map id {mapId}", mapId);
            return new List<LayerRegionDto>();
        }

        var regionsDtos = await GetLayerRegionsDtos(regions, ct);

        return regionsDtos;
    }

    /// <summary>
    /// Обновляет слой региона полностью со всеми зависимыми сущностями.
    /// Обновятся только не null значения. Остальные свойства сохраняться прежними.
    /// </summary>
    /// <param name="layerRegionId"></param>
    /// <param name="layerRegionDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Guid> UpdateLayerRegionAsync(Guid layerRegionId, LayerRegionDto layerRegionDto, CancellationToken ct)
    {
        var layerRegion = await _layerRegionRepository.GetHeaderByIdAsync(layerRegionId, ct);

        if (layerRegion == null)
        {
            _logger.LogError("Invalid region id {regionId}", layerRegionDto.Id);
            return Guid.Empty;
        }

        if (layerRegionDto.IsActive != null)
        {
            layerRegion.IsActive = layerRegionDto.IsActive.Value;
        }

        if (layerRegionDto.Indicators != null)
        {
            var indicators = await _indicatorsService.GetIndicatorsByLayerRegionAsync(layerRegionId, ct);

            var indicatorsId = indicators is null 
                ? await _indicatorsService.CreateIndicatorsAsync(layerRegionId, layerRegionDto.Indicators, ct) 
                : await _indicatorsService.UpdateIndicatorsAsync(layerRegionId, layerRegionDto.Indicators, ct);
        }

        if (layerRegionDto.HistoricalObjects != null)
        {
            foreach (var historicalObjectDto in layerRegionDto.HistoricalObjects)
            { 
                await _historicalObjectService.UpdateHistoricalObjectAsync(historicalObjectDto.Id!.Value,
                    historicalObjectDto, ct);
            }
        }
        
        if (layerRegionDto.Style != null)
        {
            var styleDto = await _layerRegionStyleService.GetStyleByLayerIdAsync(layerRegionId, ct);

            if (styleDto is null)
            {
                await _layerRegionStyleService.AddAsync(layerRegionId, layerRegionDto.Style, ct);
            }
            else
            {
                await _layerRegionStyleService.UpdateAsync(layerRegionId, layerRegionDto.Style, ct);
            }
        }
        
        await _layerRegionRepository.UpdateAsync(layerRegion, ct);
        
        return layerRegion.Id;
    }

    /// <summary>
    /// Удаляет слой ррегиона по его Id
    /// </summary>
    /// <param name="layerRegionId">Id слоя региона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> DeleteLayerRegionAsync(Guid layerRegionId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting layer region {layerRegionId}", layerRegionId);

        var layerRegion = await _layerRegionRepository.GetHeaderByIdAsync(layerRegionId, ct);
        if (layerRegion == null)
        {
            _logger.LogInformation("layer {layerRegionId} could not be deleted", layerRegionId);
            return false;
        }
        
        //TODO добавить логику добавления пустого региона перед удалением
        
        await _layerRegionRepository.DeleteByIdAsync(layerRegionId, ct);
       
        _logger.LogInformation("layer {layerRegionId} deleted", layerRegionId);
        return true;
    }

    /// <summary>
    /// Преобразует доменные сущности региона в DTO. Добавляет все зависящие от слоя сущности.
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task<List<LayerRegionDto>> GetLayerRegionsDtos(List<LayerRegion> regions, CancellationToken ct)
    {
        var regionsDtos = new List<LayerRegionDto>();
        
        foreach (var region in regions)
        {
            var regionDto = new LayerRegionDto
            {
                Id = region.Id,
                IsActive = region.IsActive,
                Name = region.Region.Name,
                Geometry = region.Region.Geometry.Geometry,
            };

            regionDto.HistoricalObjects = await _historicalObjectService.GetAllByLayerRegionIdAsync(region.Id, ct);
                
            var style = await _layerRegionStyleService.GetStyleByLayerIdAsync(region.Id, ct);
            regionDto.Style = style;
            
            var indicatorsDto = await _indicatorsService.GetIndicatorsByLayerRegionAsync(region.Id, ct);
            regionDto.Indicators = indicatorsDto;
            
            regionsDtos.Add(regionDto);
        }
        
        return regionsDtos;
    }

    public async Task AddNewHistoricalObjectsAsync(List<HistoricalObjectDto> objectDtos, CancellationToken ct)
    {
        if (objectDtos.Count == 0)
        {
            _logger.LogError("No historical objects to add");
            return;
        }
        
        foreach (var objectDto in objectDtos)
        {
            _logger.LogInformation("Adding historical objects to region {regionId}", objectDto.LayerRegionId);
            var id = await _historicalObjectService.CreateHistoricalObjectAsync(objectDto.LayerRegionId!.Value, objectDto, ct);

            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid historical objectDto");
            }
        }
    }
}