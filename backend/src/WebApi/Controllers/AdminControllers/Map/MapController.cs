using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.Map.Mapper;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Map;

[ApiController]
[Route("/api/admin/maps")]
public class MapController : ControllerBase
{
    private readonly IMapService _mapService;

    public MapController(IMapService mapService)
    {
        _mapService = mapService;
    }

    /// <summary>
    /// Получить карту
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet("{mapId:guid}")]
    [ProducesResponseType(typeof(MapLayersFeatureCollectionResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMap([FromRoute] Guid mapId, CancellationToken ct)
    {
        var map = await _mapService.GetMapAsync(mapId, ct);

        var response = MapMapper.MapDtoToResponse(map);
        
        return response == null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Создать карту
    /// </summary>
    /// <param name="request">Dto для создания карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(MapLayersFeatureCollectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMap([FromForm] CreateMapRequest request, CancellationToken ct)
    {
        var mapDto = MapMapper.MapRequestToDto(request);
        var id = await _mapService.CreateMapAsync(mapDto, ct);
        
        var map = await _mapService.GetMapAsync(id, ct);

        var response = MapMapper.MapDtoToResponse(map);
        
        return CreatedAtAction(nameof(GetMap), new { mapId = id }, response);
    }

    /// <summary>
    /// Добавить новый регион в карту
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="request">Dto для создания региона</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost("{mapId:guid}/regions")]
    [ProducesResponseType(typeof(MapLayersFeatureCollectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNewLayerRegion([FromRoute] Guid mapId,
        [FromForm] CreateLayerRegionRequest request, CancellationToken ct)
    {
        var map = await _mapService.GetMapAsync(mapId, ct);

        var layerRegionDto = LayerRegionMapper.LayerRegionRequestToDto(request);

        var id = await _mapService.AddNewLayerRegionAsync(map.Id ?? Guid.Empty, layerRegionDto, ct);
        
        return Ok(id);
    }
}