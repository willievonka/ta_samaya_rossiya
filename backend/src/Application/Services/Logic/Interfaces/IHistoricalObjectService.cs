using Application.Services.Dtos;

namespace Application.Services.Logic.Interfaces;

public interface IHistoricalObjectService
{
    Task<Guid> CreateHistoricalObjectAsync(Guid layerRegionId, HistoricalObjectDto? histObjectDto, CancellationToken ct);
    Task<Guid> UpdateHistoricalObjectAsync(Guid histObjectId, HistoricalObjectDto? histObjectDto, CancellationToken ct);
    Task<List<HistoricalObjectDto>?> GetAllByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct);
    Task<bool> DeleteHistoricalObjectAsync(Guid histObjectId, CancellationToken ct);
}