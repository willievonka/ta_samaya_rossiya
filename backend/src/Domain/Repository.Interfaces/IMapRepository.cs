using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IMapRepository
{
    Task AddAsync(Map map, CancellationToken ct);
    Task<Map?> GetByIdAsync(Guid mapId, CancellationToken ct);
    Task<Map?> GetByIdWithActiveRegionsAsync(Guid mapId, CancellationToken ct);
    Task<List<Map>?> GetAllAsync(Guid mapId, CancellationToken ct);
    Task UpdateAsync(Map map, CancellationToken ct);
    Task DeleteByIdAsync(Guid mapId, CancellationToken ct);
}