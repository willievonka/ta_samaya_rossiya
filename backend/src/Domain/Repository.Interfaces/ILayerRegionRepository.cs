using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface ILayerRegionRepository
{
    Task AddAsync(LayerRegion layerRegion, CancellationToken ct);
    Task<LayerRegion?> GetNoActiveEmptyByNameAndMapIdAsync(string name,  Guid mapId, CancellationToken ct);  
    Task<LayerRegion?> GetHeaderByIdAsync(Guid regionId, CancellationToken ct);
    Task<LayerRegion?> GetWithRegionByIdAsync(Guid regionId, CancellationToken ct);
    Task<List<LayerRegion>?> GetAllWithRegionAndGeometryByMapIdAsync(Guid mapId, CancellationToken ct); 
    Task<List<LayerRegion>?> GetAllActiveWithRegionAndGeometryByMapAsync(Guid mapId, CancellationToken ct); 
    Task UpdateAsync(LayerRegion layerRegion, CancellationToken ct);
    Task DeleteByIdAsync(Guid regionId, CancellationToken ct);
}