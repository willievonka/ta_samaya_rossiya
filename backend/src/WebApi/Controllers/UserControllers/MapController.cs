using Application.Services.Dtos.Map.Responses;
using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.UserControllers;

[ApiController]
[Route("api/client/maps")]
public class MapController : ControllerBase
{
    private readonly IMapQueryCachingService _mapQueryCachingService;
    
    public MapController(IMapQueryCachingService mapQueryCachingService)
    {
        _mapQueryCachingService = mapQueryCachingService;
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
        var mapResponse = await _mapQueryCachingService.GetMapResponseAsync(mapId, ct);
            
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
}