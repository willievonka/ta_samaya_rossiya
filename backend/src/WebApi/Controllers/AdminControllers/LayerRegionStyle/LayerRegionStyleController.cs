using Application.Services.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;
using WebApi.Controllers.AdminControllers.Mapper;

namespace WebApi.Controllers.AdminControllers.LayerRegionStyle;

[ApiController]
[Route("api/admin/maps/{mapId:guid}/layers/{layerId:guid}/style")]
public class LayerRegionStyleController : ControllerBase
{
    private readonly ILayerRegionStyleService _layerRegionStyleService;

    public LayerRegionStyleController(ILayerRegionStyleService layerRegionStyleService)
    {
        _layerRegionStyleService = layerRegionStyleService;
    }

    /// <summary>
    /// Создание стиля leaflet для слоя региона
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="layerId">Id региона</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="request">Данные стиля</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status303SeeOther)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddStyleRegion([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        [FromBody] CreateLayerRegionStyleRequest request, CancellationToken ct)
    {
        var dto = LayerRegionStyleMapper.StyleRequestToDto(request);

        var styleId = await _layerRegionStyleService.AddAsync(layerId, dto, ct);

        if (styleId == Guid.Empty)
            BadRequest();
        
        return RedirectToAction(nameof(GetStyleByLayerId), new { mapId, layerId, styleId });
    }
    
    /// <summary>
    /// Получить стиль по layer Id
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="layerId">Id региона</param>
    /// <param name="ct">Токен отмены</param>
    [HttpGet]
    [ProducesResponseType(typeof(LayerRegionStyleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStyleByLayerId([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        CancellationToken ct)
    {
        var style = await _layerRegionStyleService.GetStyleByLayerIdAsync(layerId, ct);
        
        var response = LayerRegionStyleMapper.StyleDtoToResponse(style);

        return response == null ? NotFound() : Ok(response);
    }
    
    /// <summary>
    /// Обновить стиль региона
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="layerId">Id региона</param>
    /// <param name="request">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPatch]
    [ProducesResponseType(typeof(LayerRegionStyleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateByLayerIdStyle([FromRoute] Guid mapId, [FromRoute] Guid layerId,
        [FromBody] CreateLayerRegionStyleRequest request, CancellationToken ct)
    {
        var dto = LayerRegionStyleMapper.StyleRequestToDto(request);

        var updatingDto = await _layerRegionStyleService.UpdateAsync(layerId, dto, ct);

        var response = LayerRegionStyleMapper.StyleDtoToResponse(updatingDto);
        
        return response == null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Удаление стиля региона
    /// </summary>
    /// <param name="mapId">Id карты</param>
    /// <param name="layerId">Id региона</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStyle([FromRoute] Guid mapId, [FromRoute] Guid layerId, CancellationToken ct)
    {
        await _layerRegionStyleService.DeleteByLayerIdAsync(layerId, ct);
        
        return NoContent();
    }
}