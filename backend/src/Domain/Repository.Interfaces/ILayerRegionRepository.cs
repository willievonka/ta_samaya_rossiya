using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface ILayerRegionRepository
{
    Task AddAsync(LayerRegion layerRegion, CancellationToken ct);
    Task<LayerRegion?> GetByIdAsync(Guid regionId, CancellationToken ct);
    Task<List<LayerRegion>?> GetAllByMapAsync(Guid mapId, CancellationToken ct); 
    Task<List<LayerRegion>?> GetAllActiveByMapAsync(Guid mapId, CancellationToken ct); 
    Task UpdateAsync(LayerRegion layerRegion, CancellationToken ct);
    Task DeleteByIdAsync(Guid regionId, CancellationToken ct);
}