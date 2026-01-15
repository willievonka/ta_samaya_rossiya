using Application.Services.Dtos.Map.Requests;
using Application.Services.Dtos.Map.Responses;
using Application.Services.Logic.Interfaces;
using Application.Services.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.Map;

[ApiController]
[Authorize]
[Route("api/admin/maps")]
public class AdminMapController : ControllerBase
{
    private readonly IMapService _mapService;
    private readonly IMapQueryCachingService _mapQueryCachingService;
    
    public AdminMapController(IMapService mapService, IMapQueryCachingService mapQueryCachingService)
    {
        _mapService = mapService;
        _mapQueryCachingService = mapQueryCachingService;
    }

    /// <summary>
    /// Получить карту. При mapId = null отправиться шаблон карты для её дальнейшего создания
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MapPageResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMap([FromQuery] Guid? mapId, CancellationToken ct)
    {
        if (mapId == null)
        {
            var emptyMapResponse = await _mapQueryCachingService.GetEmptyMapResponseAsync(ct);
        
            return emptyMapResponse == null 
                ? NotFound() 
                : Content(emptyMapResponse, "application/json");
        }
        
        var mapResponse = await _mapQueryCachingService.GetMapResponseAsync(mapId.Value, ct);
            
        return mapResponse == null 
            ? NotFound() 
            : Content(mapResponse, "application/json");
    }
    
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
        var response = await _mapQueryCachingService.GetAllMapsCardsResponseAsync(ct);
        
        return Content(response, "application/json");
    }

    /// <summary>
    /// Создать карту
    /// </summary>
    /// <param name="request">Dto для создания карты</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]  
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMap([FromForm] CreateMapRequest request, CancellationToken ct)
    {
        var mapDto = MapMapper.CreateMapRequestToDto(request);
        
        var mapId = await _mapService.CreateMapAsync(mapDto, ct);
        
        if (mapId == Guid.Empty)
            return BadRequest();
        
        await _mapQueryCachingService.RemoveAllMapsCardsResponseCacheAsync(ct);
        
        return Ok(new { mapId = mapId });
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
        
        await _mapQueryCachingService.RemoveAllMapsCardsResponseCacheAsync(ct);
        
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

        await _mapQueryCachingService.RemoveAllMapsCardsResponseCacheAsync(ct);
        await _mapQueryCachingService.RemoveMapResponseCacheAsync(mapId, ct);
        
        return Ok();
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
        
        await _mapQueryCachingService.RemoveAllMapsCardsResponseCacheAsync(ct);
        await _mapQueryCachingService.RemoveMapResponseCacheAsync(mapId, ct);
        
        if (res) return NoContent();
        
        return NotFound();
    }
}