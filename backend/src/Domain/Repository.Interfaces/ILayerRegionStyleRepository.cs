using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface ILayerRegionStyleRepository
{
    Task AddAsync(LayerRegionStyle style, CancellationToken ct);
    Task<LayerRegionStyle?> GetByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct);
    Task UpdateAsync(LayerRegionStyle style, CancellationToken ct);
    Task DeleteByLayerRegionIdAsync(Guid layerRegionId, CancellationToken ct);
}