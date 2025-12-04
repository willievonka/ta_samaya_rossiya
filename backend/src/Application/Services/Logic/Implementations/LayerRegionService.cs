using Application.Services.Dtos;
using Application.Services.Interfaces;
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

    public LayerRegionService(ILayerRegionRepository layerRegionRepository,
        IRegionRepository regionRepository, ILogger<ILayerRegionService> logger, 
        IIndicatorsService indicatorsService, ILayerRegionStyleService layerRegionStyleService)
    {
        _layerRegionRepository = layerRegionRepository;
        _regionRepository = regionRepository;
        _logger = logger;
        _indicatorsService = indicatorsService;
        _layerRegionStyleService = layerRegionStyleService;
    }
    
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
            IsActive = layerRegionDto.IsActive,
            RegionId = region.Id,
            MapId = mapId,
        };
        await _layerRegionRepository.AddAsync(newLayerRegion, ct);

        if (layerRegionDto.Indicators != null)
        {
            var indicatorsId = await _indicatorsService.CreateIndicatorsAsync(newLayerRegion.Id, layerRegionDto.Indicators, ct);
        }

        if (layerRegionDto.Style != null)
        {
            await _layerRegionStyleService.AddAsync(newLayerRegion.Id, layerRegionDto.Style, ct);
        }
        
        return newLayerRegion.Id;
    }

    public async Task<List<LayerRegionDto>?> GetAllByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        var regions = await _layerRegionRepository.GetAllByMapAllIncludesAsync(mapId, ct);
        
        if (regions == null)
        {
            _logger.LogError("Invalid map id {mapId}", mapId);
            return new List<LayerRegionDto>();
        }

        var regionsDtos = await GetLayerRegionsDtos(regions, ct);

        return regionsDtos;
    }

    public async Task<List<LayerRegionDto>?> GetAllActiveByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        var regions = await _layerRegionRepository.GetAllActiveByMapAllInlcudesAsync(mapId, ct);
        
        if (regions == null)
        {
            _logger.LogError("Invalid map id {mapId}", mapId);
            return new List<LayerRegionDto>();
        }

        var regionsDtos = await GetLayerRegionsDtos(regions, ct);

        return regionsDtos;
    }

    public async Task<Guid> UpdateLayerRegionAsync(Guid layerRegionId, LayerRegionDto layerRegionDto, CancellationToken ct)
    {
        var layerRegion = await _layerRegionRepository.GetHeaderByIdAsync(layerRegionId, ct);

        if (layerRegion == null)
        {
            _logger.LogError("Invalid region id {regionId}", layerRegionDto.Id);
            return Guid.Empty;
        }
        
        layerRegion.IsActive = layerRegionDto.IsActive;

        if (layerRegionDto.Indicators != null)
        {
            var indicators = await _indicatorsService.GetIndicatorsByLayerRegionAsync(layerRegionId, ct);

            var indicatorsId = indicators is null 
                ? await _indicatorsService.CreateIndicatorsAsync(layerRegionId, layerRegionDto.Indicators, ct) 
                : await _indicatorsService.UpdateIndicatorsAsync(layerRegionId, layerRegionDto.Indicators, ct);
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

    public async Task<bool> DeleteLayerRegionAsync(Guid layerRegionId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting layer region {layerRegionId}", layerRegionId);

        var layerRegion = await _layerRegionRepository.GetHeaderByIdAsync(layerRegionId, ct);
        if (layerRegion == null)
        {
            _logger.LogInformation("layer {layerRegionId} could not be deleted", layerRegionId);
            return false;
        }
        
        await _layerRegionRepository.DeleteByIdAsync(layerRegionId, ct);
       
        _logger.LogInformation("layer {layerRegionId} deleted", layerRegionId);
        return true;
    }

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
            
            regionDto.Style = await _layerRegionStyleService.GetStyleByLayerIdAsync(region.Id, ct);
            
            var indicatorsDto = await _indicatorsService.GetIndicatorsByLayerRegionAsync(region.Id, ct);
            regionDto.Indicators = indicatorsDto;
            
            regionsDtos.Add(regionDto);
        }
        
        return regionsDtos;
    }
}