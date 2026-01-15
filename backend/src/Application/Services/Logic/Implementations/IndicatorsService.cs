using Application.Services.Dtos.Indicators;
using Application.Services.Logic.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class IndicatorsService : IIndicatorsService
{
    private readonly IIndicatorsRepository _indicatorsRepository;
    private readonly ILogger<IIndicatorsService> _logger;
    private readonly IImageService _imageService;

    private const string FilePath = "indicators"; 
    
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
            _logger.LogInformation("Starting save Indicators image");
            var fileUri = await _imageService.SaveImageAsync(layerRegionId, FilePath, indicatorsRegionDto.Image);
            indicators.ImagePath = fileUri;
        }
        else _logger.LogError("Image is null");
        
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
            _logger.LogWarning("IndicatorsRegion could not be found by layer {indicatorsRegionDto.Id}", layerRegionId);
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
            _logger.LogWarning("IndicatorsRegion {indicatorsRegionDto.Id} could not be found", indicatorsRegionDto.Id);
            return Guid.Empty;
        }
        
        if (indicatorsRegionDto.Image != null)
        { 
            _logger.LogInformation("Starting update Indicators image");
            var fileUri = await _imageService.UpdateImageAsync(indicators.Id, indicators.ImagePath, FilePath,
                indicatorsRegionDto.Image);
            
            indicators.ImagePath = fileUri;
        }
        else if (indicators.ImagePath != null)
        {
            await _imageService.DeleteImageAsync(indicators.ImagePath);
            indicators.ImagePath = null;
            _logger.LogInformation("Image {path} deleted", indicators.ImagePath);
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
    /// <param name="layerRegionId">Id показателей региона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> DeleteIndicatorsAsync(Guid layerRegionId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting Indicators {indicatorsId}", layerRegionId);
        
        var indicators = await _indicatorsRepository.GetByLayerRegionAsync(layerRegionId, ct);
        if (indicators == null)
        {
            _logger.LogInformation("Indicators {indicatorsId} could not be deleted", layerRegionId);
            return false;
        }
        
        if (indicators.ImagePath != null)
        { 
            _logger.LogInformation("Starting delete Indicators image");
            await _imageService.DeleteImageAsync(indicators.ImagePath);
        }
        
        await _indicatorsRepository.DeleteByLayerIdAsync(layerRegionId, ct);
       
        _logger.LogInformation("Indicators {indicatorsId} deleted", layerRegionId);
        return true;
    }
}