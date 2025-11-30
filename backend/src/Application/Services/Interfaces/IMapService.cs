using Application.Services.Dtos;

namespace Application.Services.Interfaces;

public interface IMapService
{
    //Task<IEnumerable<MapDto>> GetMaps();
    Task<Guid> CreateMapAsync(MapDto mapDto, CancellationToken ct);
    Task<bool> DeleteMapAsync(Guid mapId, CancellationToken ct);
    Task<MapDto?> GetMapAsync(Guid mapId, CancellationToken ct);
    Task<Guid> UpdateMapAsync(MapDto mapDto, CancellationToken ct);
    Task<Guid> AddNewLayerRegionAsync(Guid mapId, LayerRegionDto layerRegionDto, CancellationToken ct);
}