using Application.Services.Interfaces;
using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;
using WebApi.Controllers.AdminControllers.Mapper;

namespace WebApi.Controllers.AdminControllers.Map;

[ApiController]
[Route("api/admin/maps")]
public class   MapController : ControllerBase
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
    
    //TODO: GetMapWithActiveRegions
    
    /// <summary>
    /// Получить все карточки карт
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet("cards")]
    [ProducesResponseType(typeof(MapsCardsResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllMapsCards(CancellationToken ct)
    {
        var cards = await _mapService.GetAllCardsASync(ct);
        
        var response = MapMapper.MapsDtosToMapsCardsResponse(cards);
        
        return response == null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Создать карту
    /// </summary>
    /// <param name="request">Dto для создания карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MapLayersFeatureCollectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMap([FromForm] CreateMapRequest request, CancellationToken ct)
    {
        var mapDto = MapMapper.CreateMapRequestToDto(request);
        var id = await _mapService.CreateMapAsync(mapDto, ct);
        
        var map = await _mapService.GetMapAsync(id, ct);

        var response = MapMapper.MapDtoToResponse(map);
        
        return RedirectToAction(nameof(GetAllMapsCards));
    }

    /// <summary>
    /// Обновление карточки карты
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="request">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPatch("{mapId:Guid}/card")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status303SeeOther)]  
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMapCard([FromRoute] Guid mapId, [FromForm] UpdateMapCardRequest request,
        CancellationToken ct)
    {
        var mapDto = MapMapper.UpdateMapCardRequestToDto(request, mapId);
        
        var response = await _mapService.UpdateMapAsync(mapDto, ct);
        
        return RedirectToAction(nameof(GetAllMapsCards));
    }

    /// <summary>
    /// Полностью обновить карту с новыми слоями регионов
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="request">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPut("{mapId:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status303SeeOther)]  
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMap([FromRoute] Guid mapId, [FromForm] UpdateMapRequest request,
        CancellationToken ct)
    {
        var mapDto = MapMapper.UpdateMapRequestToDto(request, mapId);
        
        var response = await _mapService.UpdateMapAsync(mapDto, ct);
        
        return RedirectToAction(nameof(GetMap), new { mapId = mapId });
    }

    /// <summary>
    /// Удалить карту
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpDelete("{mapId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMap([FromRoute] Guid mapId, CancellationToken ct)
    {
        var res = await _mapService.DeleteMapAsync(mapId, ct);
        
        if (res) return NoContent();
        
        return NotFound();
    }
}