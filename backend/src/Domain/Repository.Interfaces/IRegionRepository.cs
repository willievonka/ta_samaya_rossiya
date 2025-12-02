using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IRegionRepository
{
    Task AddAsync(Region region, CancellationToken ct);
    Task<Region?> GetByIdAsync(Guid regionId, CancellationToken ct);
    Task<Region?> GetByNameAsync(string name, CancellationToken ct);  
    Task<List<Region>?> GetAllAsync(CancellationToken ct);
    Task UpdateAsync(Region region, CancellationToken ct);
    Task DeleteByIdAsync(Guid regionId, CancellationToken ct);
}