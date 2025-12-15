using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.HistoricalObject.Request;
using WebApi.Controllers.AdminControllers.HistoricalObject.Response;
using WebApi.Controllers.AdminControllers.Mapper;

namespace WebApi.Controllers.AdminControllers.HistoricalObject;

[ApiController]
[Route("api/admin/maps/{mapId:guid}/layers")]
public class HistoricalObjectController : ControllerBase
{
    private readonly ILayerRegionService _layerRegionService;
    private readonly IHistoricalObjectService _historicalObjectService;
    
    public HistoricalObjectController(ILayerRegionService layerRegionService,
        IHistoricalObjectService historicalObjectService)
    {
        _historicalObjectService = historicalObjectService;
        _layerRegionService = layerRegionService;
    }

    /// <summary>
    /// Добавить множество объектов, вне зависимости от региона
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("points")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateManyHistoricalObjects([FromRoute] Guid mapId, 
        [FromForm] CreateHistoricalObjectsRequest request, CancellationToken ct)
    {
        var dtos = HistoricalObjectMapper.CreateHistoricalObjectsRequestToDtosList(request);

        await _layerRegionService.AddNewHistoricalObjectsAsync(dtos, ct);
        
        return Ok();
    }

    /// <summary>
    /// Добавить один объект к определённому региону
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="layerId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("{layerId:guid}/points")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHistoricalObject([FromRoute] Guid mapId, [FromRoute] Guid layerId, 
        [FromForm] CreateHistoricalObjectRequest request, CancellationToken ct)
    {
        var dto = HistoricalObjectMapper.CreateHistoricalObjectRequestToDto(request, layerId);
        
        var id = await _historicalObjectService.CreateHistoricalObjectAsync(layerId, dto, ct);
        
        return Ok(id);
    }
    
    /// <summary>
    /// Получить все объекты одного региона
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="layerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{layerId:guid}/points")]
    [ProducesResponseType(typeof(List<HistoricalObjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllHistoricalObjectsByLayerId([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        CancellationToken ct)
    {
        var dtos = await _historicalObjectService.GetAllByLayerRegionIdAsync(layerId, ct);

        var response = HistoricalObjectMapper.HistoricalObjectsListDtoToResponse(dtos);
        
        return Ok(response);
    }

    /// <summary>
    /// Удалить объект по его Id
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="layerId"></param>
    /// <param name="pointId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpDelete("{layerId:guid}/points")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHistoricalObjectById([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        [FromQuery] Guid pointId, CancellationToken ct)
    {
        var res = await _historicalObjectService.DeleteHistoricalObjectAsync(pointId, ct);
        
        return res ? Ok() : NotFound();
    }

    /// <summary>
    /// Обновить объект
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="layerId"></param>
    /// <param name="pointId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPatch("{layerId:guid}/points")]
    public async Task<IActionResult> UpdateHistoricalObject([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        [FromQuery] Guid pointId, [FromForm] UpdateHistoricalObjectRequest request, CancellationToken ct)
    {
        var dto = HistoricalObjectMapper.UpdateHistoricalObjectRequestToDto(request, pointId);
        
        var updated = await _historicalObjectService.UpdateHistoricalObjectAsync(pointId, dto, ct);
        
        return updated == Guid.Empty ? NotFound() : Ok();
    }
}