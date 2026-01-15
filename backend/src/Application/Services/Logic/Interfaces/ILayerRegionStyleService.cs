using Application.Services.Dtos.LayerRegionStyle;

namespace Application.Services.Logic.Interfaces;

public interface ILayerRegionStyleService
{
    Task<Guid> AddAsync(Guid layerRegionId, LayerRegionStyleDto? styleDto, CancellationToken ct);
    Task<LayerRegionStyleDto?> GetStyleByLayerIdAsync(Guid layerRegionId, CancellationToken ct);
    Task<LayerRegionStyleDto?> UpdateAsync(Guid layerRegionId, LayerRegionStyleDto? styleDto, CancellationToken ct);
    Task DeleteByLayerIdAsync(Guid layerRegionId, CancellationToken ct);
}