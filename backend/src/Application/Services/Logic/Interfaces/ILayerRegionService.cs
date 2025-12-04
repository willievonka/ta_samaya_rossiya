using Application.Services.Dtos;

namespace Application.Services.Logic.Interfaces;

public interface ILayerRegionService
{
    Task<Guid> CreateLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct);
    Task<List<LayerRegionDto>?> GetAllByMapIdAsync(Guid mapId, CancellationToken ct);
    Task<List<LayerRegionDto>?> GetAllActiveByMapIdAsync(Guid mapId, CancellationToken ct);
    Task<Guid> UpdateLayerRegionAsync(Guid layerRegionId, LayerRegionDto layerRegionDto, CancellationToken ct);
    Task<bool> DeleteLayerRegionAsync(Guid layerRegionId, CancellationToken ct);
}