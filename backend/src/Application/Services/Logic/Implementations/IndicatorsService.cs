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
    
    /// <summary>
    /// Создаёт показатели слоя региона
    /// </summary>
    /// <param name="layerRegionId">id слоя региона</param>
    /// <param name="indicatorsRegionDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Guid> CreateIndicatorsAsync(Guid layerRegionId, IndicatorsRegionDto? indicatorsRegionDto, CancellationToken ct)
    {
        if (indicatorsRegionDto == null)
        {
            _logger.LogError("IndicatorsRegionDto is null");
            return Guid.Empty;
        }
        
        _logger.LogInformation("Creating IndicatorsRegion");
        
        var indicators = new IndicatorsRegion
        {
            IsActive = indicatorsRegionDto.IsActive!.Value,
            Excursions = indicatorsRegionDto.Excursions!.Value,
            Participants = indicatorsRegionDto.Participants!.Value,
            Partners = indicatorsRegionDto.Partners!.Value,
            RegionId = layerRegionId
        };
            
        if (indicatorsRegionDto.Image != null)
        {
            var fileUri = await _imageService.SaveImageAsync(layerRegionId, FilePath, indicatorsRegionDto.Image);
            indicators.ImagePath = fileUri;
        }
        
        await _indicatorsRepository.AddAsync(indicators, ct);
        
        return indicators.Id;
    }

    /// <summary>
    /// Получает показатели слоя региона по Id слоя региона
    /// </summary>
    /// <param name="layerRegionId">id слоя региона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Обновляет показатели слоя региона
    /// </summary>
    /// <param name="layerId">id слоя региона</param>
    /// <param name="indicatorsRegionDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Guid> UpdateIndicatorsAsync(Guid layerId, IndicatorsRegionDto indicatorsRegionDto, CancellationToken ct)
    {
        var indicators = await _indicatorsRepository.GetByLayerRegionAsync(layerId, ct);

        if (indicators == null)
        {
            _logger.LogError("IndicatorsRegion {indicatorsRegionDto.Id} could not be found", indicatorsRegionDto.Id);
            return Guid.Empty;
        }
        
        if (indicatorsRegionDto.Image != null)
        { 
            var fileUri = await _imageService.UpdateImageAsync(indicators.Id, indicators.ImagePath, FilePath,
                indicatorsRegionDto.Image);
            
            indicators.ImagePath = fileUri;
        }
        
        if (indicatorsRegionDto.IsActive != null) indicators.IsActive = indicatorsRegionDto.IsActive.Value;
        if (indicatorsRegionDto.Excursions != null) indicators.Excursions = indicatorsRegionDto.Excursions.Value;
        if (indicatorsRegionDto.Participants != null) indicators.Participants = indicatorsRegionDto.Participants.Value;
        if (indicatorsRegionDto.Partners != null) indicators.Partners = indicatorsRegionDto.Partners.Value;
        
        await _indicatorsRepository.UpdateAsync(indicators, ct);
        
        return indicators.Id;
    }

    /// <summary>
    /// Удаляет показатели слоя региона
    /// </summary>
    /// <param name="indicatorsId">Id показателей региона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
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