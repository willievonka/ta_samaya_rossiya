using Application.Services.Dtos;
using Application.Services.Logic.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class HistoricalObjectService : IHistoricalObjectService
{
    private readonly ILogger<IHistoricalObjectService> _logger;
    private readonly IImageService _imageService;
    private readonly IHistoricalObjectRepository _historicalObjectRepository;

    private const string FilePath = "historical-objects"; 
    
    public HistoricalObjectService(ILogger<IHistoricalObjectService> logger, IImageService imageService,
        IHistoricalObjectRepository historicalObjectRepository)
    {
        _logger = logger;
        _imageService = imageService;
        _historicalObjectRepository = historicalObjectRepository;
    }
    
    /// <summary>
    /// Создаёт исторический объект
    /// </summary>
    /// <param name="layerRegionId">Id слоя региона</param>
    /// <param name="histObjectDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Guid> CreateHistoricalObjectAsync(Guid layerRegionId, HistoricalObjectDto? histObjectDto, CancellationToken ct)
    {
        if (histObjectDto == null)
        {
            _logger.LogError("HistObjectDto is null");
            return Guid.Empty;
        }
        
        _logger.LogInformation("Creating HistoricalObject");

        var historicalObject = new HistoricalObject
        {
            Coordinates = histObjectDto.Coordinates,
            Title = histObjectDto.Title!,
            Description = histObjectDto.Description!,
            LayerRegionId = layerRegionId,
            ExcursionUrl = histObjectDto.ExcursionUrl,
            Year = histObjectDto.Year!.Value,
        };

        if (histObjectDto.Image != null)
        {
            var fileUri = await _imageService.SaveImageAsync(layerRegionId, FilePath, histObjectDto.Image);
            historicalObject.ImagePath = fileUri;
        }
        
        await _historicalObjectRepository.AddAsync(historicalObject, ct);
        
        return historicalObject.Id;
    }

    /// <summary>
    /// Обновляет исторический объект
    /// </summary>
    /// <param name="histObjectId">Id исторического объекта</param>
    /// <param name="histObjectDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Guid> UpdateHistoricalObjectAsync(Guid histObjectId, HistoricalObjectDto? histObjectDto, CancellationToken ct)
    {
        if (histObjectDto == null)
        {
            _logger.LogError("HistoricalObjectDto is null");
            return Guid.Empty;
        }
        
        var histObject = await _historicalObjectRepository.GetByIdAsync(histObjectId, ct);

        if (histObject == null)
        {
            _logger.LogError("HistoricalObject {histObjectId} could not be found", histObjectId);
            return Guid.Empty;
        }
        
        if (histObjectDto.Image != null)
        { 
            var fileUri = await _imageService.UpdateImageAsync(histObject.Id, histObject.ImagePath, FilePath,
                histObjectDto.Image);
            
            histObject.ImagePath = fileUri;
        }
        
        if (histObjectDto.ExcursionUrl != null) histObject.ExcursionUrl = histObjectDto.ExcursionUrl;
        if (histObjectDto.Year != null) histObject.Year = histObjectDto.Year.Value;
        if (histObjectDto.Title != null) histObject.Title = histObjectDto.Title;
        if (histObjectDto.Description != null) histObject.Description = histObjectDto.Description;
        
        await _historicalObjectRepository.UpdateAsync(histObject, ct);
        
        return histObject.Id;
    }

    /// <summary>
    /// Получает все исторические объекты по Id Слоя региона
    /// </summary>
    /// <param name="layerRegionId">Id Слоя региона</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<HistoricalObjectDto>?> GetAllByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct)
    {
        var histObjects = await _historicalObjectRepository.GetAllByLayerRegionIdAsync(layerRegionId, ct);

        if (histObjects == null || histObjects.Count == 0)
        {
            _logger.LogError("HistoricalObjects could not be found by layer {layerRegionId} region", layerRegionId);
            return null;
        }
        
        var histObjectsDtos = new List<HistoricalObjectDto>();
        foreach (var histObject in histObjects)
        {
            var histObjectDto = new HistoricalObjectDto
            {
                Coordinates = histObject.Coordinates,
                Title = histObject.Title,
                Description = histObject.Description,
                ExcursionUrl = histObject.ExcursionUrl,
                Year = histObject.Year,
                ImagePath = histObject.ImagePath,
                Id = histObject.Id,
            };
            
            histObjectsDtos.Add(histObjectDto);
        }
        
        return histObjectsDtos;
    }

    /// <summary>
    /// Удаляет исторический объект
    /// </summary>
    /// <param name="histObjectId">id исторического объекта</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> DeleteHistoricalObjectAsync(Guid histObjectId, CancellationToken ct)
    {
        _logger.LogInformation("Deleting HistoricalObject {histObjectId}", histObjectId);

        var histObject = await _historicalObjectRepository.GetByIdAsync(histObjectId, ct);
        if (histObject == null)
        {
            _logger.LogError("HistoricalObject {histObjectId} could not be found", histObjectId);
            return false;
        }
        
        await _historicalObjectRepository.DeleteByIdAsync(histObjectId, ct);
        _logger.LogInformation("HistoricalObject {histObjectId} deleted", histObjectId);
        
        return true;
    }
}