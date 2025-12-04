using Application.Services.Dtos;
using Application.Services.Logic.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class IndicatorsService : IIndicatorsService
{
    private readonly IIndicatorsRepository _indicatorsRepository;
    private readonly ILogger<IIndicatorsService> _logger;
    private readonly IImageService _imageService;

    private const string FilePath = "map-cards"; 
    
    public IndicatorsService(IIndicatorsRepository indicatorsRepository, ILogger<IIndicatorsService> logger,
        IImageService imageService)
    {
        _indicatorsRepository = indicatorsRepository;
        _logger = logger;
        _imageService = imageService;
    }
    
    public async Task<Guid> CreateIndicatorsAsync(Guid layerRegionId, IndicatorsRegionDto? indicatorsRegionDto, CancellationToken ct)
    {
        if (indicatorsRegionDto == null)
        {
            _logger.LogError("IndicatorsRegionDto is null");
            return Guid.Empty;
        }
        
        _logger.LogInformation("Creating IndicatorsRegion");
        
        var indicators = new IndicatorsRegion();
            
        if (indicatorsRegionDto.Image != null)
        {
            string? fileUri = null;
            fileUri = await _imageService.SaveImageAsync(layerRegionId, FilePath, indicatorsRegionDto.Image);
            indicators.ImagePath = fileUri;
        }

        indicators.IsActive = indicatorsRegionDto.IsActive;
        indicators.Excursions = indicatorsRegionDto.Excursions;
        indicators.Participants = indicatorsRegionDto.Participants;
        indicators.Partners = indicatorsRegionDto.Partners;
        indicators.RegionId = layerRegionId;
        
        await _indicatorsRepository.AddAsync(indicators, ct);
        
        return indicators.Id;
    }

    public async Task<IndicatorsRegionDto?> GetIndicatorsByLayerRegionAsync(Guid layerRegionId, CancellationToken ct)
    {
        var indicators = await _indicatorsRepository.GetByLayerRegionAsync(layerRegionId, ct);
        
        if (indicators == null)
        {
            _logger.LogError("IndicatorsRegion could not be found by layer {indicatorsRegionDto.Id}", layerRegionId);
            return null;
        }

        return new IndicatorsRegionDto
        {
            Excursions = indicators.Excursions,
            Participants = indicators.Participants,
            ImagePath = indicators.ImagePath,
            IsActive = indicators.IsActive,
            Partners = indicators.Partners,
            Id = indicators.Id,
        };
    }

    public async Task<Guid> UpdateIndicatorsAsync(Guid layerId, IndicatorsRegionDto indicatorsRegionDto, CancellationToken ct)
    {
        var indicators = await _indicatorsRepository.GetByIdAsync(layerId, ct);

        if (indicators == null)
        {
            _logger.LogError("IndicatorsRegion {indicatorsRegionDto.Id} could not be found", indicatorsRegionDto.Id);
            return Guid.Empty;
        }
        
        string? fileUri = null;
        if (indicatorsRegionDto.Image != null)
        { 
            fileUri = await _imageService.UpdateImageAsync(indicators.Id, indicators.ImagePath, FilePath,
                indicatorsRegionDto.Image);
        }
        
        indicators.IsActive = indicatorsRegionDto.IsActive;
        indicators.Excursions = indicatorsRegionDto.Excursions;
        indicators.Participants = indicatorsRegionDto.Participants;
        indicators.Partners = indicatorsRegionDto.Partners;
        indicators.ImagePath = fileUri;
        
        await _indicatorsRepository.UpdateAsync(indicators, ct);
        
        return indicators.Id;
    }

    public async Task<bool> DeleteIndicatorsAsync(Guid indicatorsId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting Indicators {indicatorsId}", indicatorsId);
        
        var indicators = await _indicatorsRepository.GetByIdAsync(indicatorsId, ct);
        if (indicators == null)
        {
            _logger.LogInformation("Indicators {indicatorsId} could not be deleted", indicatorsId);
            return false;
        }
        
        if (indicators.ImagePath != null)
        { 
            await _imageService.DeleteImageAsync(indicators.ImagePath);
        }
        
        await _indicatorsRepository.DeleteByIdAsync(indicatorsId, ct);
       
        _logger.LogInformation("Indicators {indicatorsId} deleted", indicatorsId);
        return true;
    }
}