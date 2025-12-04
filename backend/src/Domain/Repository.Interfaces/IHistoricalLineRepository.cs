using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IHistoricalLineRepository
{
    Task AddAsync(HistoricalLine histLine, CancellationToken ct);
    Task<HistoricalLine?> GetHeaderByIdAsync(Guid histLineId, CancellationToken ct);
    Task<HistoricalLine?> GetByIdWithObjectsAsync(Guid histLineId, CancellationToken ct);
    Task<HistoricalLine?> GetByMapWithObjectsAsync(Guid mapId, CancellationToken ct);    
    Task UpdateAsync(HistoricalLine histLine, CancellationToken ct);
    Task DeleteByIdAsync(Guid histLineId, CancellationToken ct);
}