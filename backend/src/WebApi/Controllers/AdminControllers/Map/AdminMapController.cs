using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;
using WebApi.Controllers.AdminControllers.Mapper;

namespace WebApi.Controllers.AdminControllers.Map;

[ApiController]
[Authorize]
[Route("api/admin/maps")]
public class AdminMapController : ControllerBase
{
    private readonly IMapService _mapService;
    
    public AdminMapController(IMapService mapService)
    {
        _mapService = mapService;
    }

    /// <summary>
    /// Получить карту
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MapPageResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMap([FromQuery] Guid mapId, CancellationToken ct)
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
    [ProducesResponseType(typeof(List<MapCardResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllMapsCards(CancellationToken ct)
    {
        var cards = await _mapService.GetAllCardsASync(ct);
        
        var response = MapMapper.MapsDtosToMapsCardsResponse(cards);
        
        return Ok(response);
    }

    /// <summary>
    /// Создать карту
    /// </summary>
    /// <param name="request">Dto для создания карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status303SeeOther)]  
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMap([FromForm] CreateMapRequest request, CancellationToken ct)
    {
        var mapDto = MapMapper.CreateMapRequestToDto(request);
        
        await _mapService.CreateMapAsync(mapDto, ct);
        
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
        
        await _mapService.UpdateMapAsync(mapDto, ct);
        
        return RedirectToAction(nameof(GetAllMapsCards));
    }

    /// <summary>
    /// Полностью обновить карту с новыми слоями регионов
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="request">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPut]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMap([FromQuery] Guid mapId, [FromForm] UpdateMapRequest request,
        CancellationToken ct)
    {
        var mapDto = MapMapper.UpdateMapRequestToDto(request, mapId);
        
        var id = await _mapService.UpdateMapAsync(mapDto, ct);
        
        if (id == Guid.Empty)
            return BadRequest();
        
        return Ok(id);
    }

    /// <summary>
    /// Удалить карту
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMap([FromQuery] Guid mapId, CancellationToken ct)
    {
        var res = await _mapService.DeleteMapAsync(mapId, ct);
        
        if (res) return NoContent();
        
        return NotFound();
    }
}