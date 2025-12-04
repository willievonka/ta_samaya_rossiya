using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IMapRepository
{
    Task AddAsync(Map map, CancellationToken ct);
    Task<Map?> GetHeaderByIdAsync (Guid mapId, CancellationToken ct);
    Task<List<Map>?> GetAllHeadersAsync(CancellationToken ct);
    Task UpdateAsync(Map map, CancellationToken ct);
    Task DeleteByIdAsync(Guid mapId, CancellationToken ct);
}