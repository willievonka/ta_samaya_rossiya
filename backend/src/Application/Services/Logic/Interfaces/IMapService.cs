using Application.Services.Dtos;

namespace Application.Services.Logic.Interfaces;

public interface IMapService
{
    Task<List<MapDto>> GetAllCardsASync(CancellationToken ct);
    Task<Guid> CreateMapAsync(MapDto? mapDto, CancellationToken ct);
    Task<bool> DeleteMapAsync(Guid mapId, CancellationToken ct);
    Task<MapDto?> GetMapAsync(Guid mapId, CancellationToken ct);
    Task<MapDto> GetEmptyMapAsync(CancellationToken ct);
    Task<Guid> UpdateMapAsync(MapDto? mapDto, CancellationToken ct);
    Task<Guid> AddNewLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct);
}