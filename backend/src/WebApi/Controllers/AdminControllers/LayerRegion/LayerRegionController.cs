using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.LayerRegion.Request;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;
using WebApi.Controllers.AdminControllers.Mapper;

namespace WebApi.Controllers.AdminControllers.LayerRegion;

[ApiController]
[Route("api/admin/maps/{mapId:guid}/layers")]
public class LayerRegionController : ControllerBase
{
    private readonly IMapService _mapService;
    private readonly ILayerRegionService _layerRegionService;

    public LayerRegionController(IMapService mapService,  ILayerRegionService layerRegionService)
    {
        _mapService = mapService;
        _layerRegionService = layerRegionService;
    }
    
    /// <summary>
    /// Добавить новый регион в карту
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="request">Dto для создания региона</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MapLayersFeatureCollectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNewLayerRegion([FromRoute] Guid mapId,
        [FromForm] CreateLayerRegionRequest request, CancellationToken ct)
    {
        var layerRegionDto = LayerRegionMapper.CreateLayerRegionRequestToDto(request);

        var id = await _mapService.AddNewLayerRegionAsync(mapId, layerRegionDto, ct);

        if (id == Guid.Empty)
            return BadRequest();
        
        return Ok(id);
    }

    /// <summary>
    /// Удалить регион 
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="layerId">Id региона</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpDelete("{layerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLayerRegion([FromRoute] Guid mapId,
        [FromRoute] Guid layerId, CancellationToken ct)
    {
        var res = await _layerRegionService.DeleteLayerRegionAsync(layerId, ct);
        
        if (res) return NoContent();
        
        return NotFound();
    }

    /// <summary>
    /// Полностью обновить слой региона
    /// </summary>
    /// <param name="mapId">Id карты </param>
    /// <param name="layerId">Id региона</param>
    /// <param name="request">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPut("{layerId:guid}")]
    [ProducesResponseType(StatusCodes.Status303SeeOther)]  
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLayerRegion([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        [FromForm] UpdateLayerRegionRequest request, CancellationToken ct)
    {
        var dto = LayerRegionMapper.UpdateLayerRegionRequestToDto(request);
        
        var id = await _layerRegionService.UpdateLayerRegionAsync(layerId, dto, ct);

        if (id == Guid.Empty)
            return NotFound();
        
        return Ok(id);
    }
}