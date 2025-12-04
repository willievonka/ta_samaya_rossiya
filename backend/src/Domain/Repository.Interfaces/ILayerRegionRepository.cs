using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface ILayerRegionRepository
{
    Task AddAsync(LayerRegion layerRegion, CancellationToken ct);
    Task<LayerRegion?> GetHeaderByIdAsync(Guid regionId, CancellationToken ct);
    Task<List<LayerRegion>?> GetAllByMapAllIncludesAsync(Guid mapId, CancellationToken ct); 
    Task<List<LayerRegion>?> GetAllActiveByMapAllInlcudesAsync(Guid mapId, CancellationToken ct); 
    Task UpdateAsync(LayerRegion layerRegion, CancellationToken ct);
    Task DeleteByIdAsync(Guid regionId, CancellationToken ct);
}