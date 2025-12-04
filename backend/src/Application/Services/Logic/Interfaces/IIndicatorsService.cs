using Application.Services.Dtos;

namespace Application.Services.Logic.Interfaces;

public interface IIndicatorsService
{
    Task<Guid> CreateIndicatorsAsync(Guid layerRegionId, IndicatorsRegionDto? indicatorsRegionDto, CancellationToken ct);
    Task<IndicatorsRegionDto?> GetIndicatorsByLayerRegionAsync(Guid layerRegionId, CancellationToken ct);
    Task<Guid> UpdateIndicatorsAsync(Guid layerId, IndicatorsRegionDto indicatorsRegionDto, CancellationToken ct);
    Task<bool> DeleteIndicatorsAsync(Guid indicatorsId, CancellationToken ct);
}