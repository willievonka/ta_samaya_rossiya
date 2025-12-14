using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IHistoricalObjectRepository
{
    Task AddAsync(HistoricalObject histObject, CancellationToken ct);
    Task<HistoricalObject?> GetByIdAsync(Guid histObjectId, CancellationToken ct);
    Task<List<HistoricalObject>?> GetAllByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct);    
    Task UpdateAsync(HistoricalObject histObject, CancellationToken ct);
    Task DeleteByIdAsync(Guid histObjectId, CancellationToken ct);
}