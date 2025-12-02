using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IIndicatorsRepository
{
    Task AddAsync(IndicatorsRegion indicators, CancellationToken ct);
    Task<IndicatorsRegion?> GetByIdAsync(Guid indicatorsId, CancellationToken ct);
    Task<IndicatorsRegion?> GetByLayerRegionAsync(Guid layerId, CancellationToken ct);
    Task UpdateAsync(IndicatorsRegion indicators, CancellationToken ct);
    Task DeleteByIdAsync(Guid indicatorsId, CancellationToken ct);
}