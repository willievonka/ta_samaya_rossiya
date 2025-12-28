using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.Map.Responses;
using WebApi.Controllers.AdminControllers.Mapper;

namespace WebApi.Controllers.UserControllers;

[ApiController]
[Route("api/client/maps")]
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
    [HttpGet]
    [ProducesResponseType(typeof(MapPageResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMap([FromQuery] Guid mapId, CancellationToken ct)
    {
        var map = await _mapService.GetMapAsync(mapId, ct);

        var response = MapMapper.MapDtoToResponse(map);
        
        return response == null ? NotFound() : Ok(response);
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
        var cards = await _mapService.GetAllCardsASync(ct);
        
        var response = MapMapper.MapsDtosToMapsCardsResponse(cards);
        
        return Ok(response);
    }
}