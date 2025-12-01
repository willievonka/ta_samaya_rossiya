using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IRegionGeometryRepository
{
    Task AddAsync(RegionGeometry regionGeometry, CancellationToken ct);
    Task<RegionGeometry?> GetByIdAsync(Guid geometryId, CancellationToken ct);
    Task<RegionGeometry?> GetByRegionAsync(Guid regionId, CancellationToken ct);
    Task UpdateAsync(RegionGeometry regionGeometry, CancellationToken ct);
    Task DeleteByIdAsync(Guid geometryId, CancellationToken ct);
}