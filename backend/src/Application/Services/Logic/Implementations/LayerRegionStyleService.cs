using System.Data;
using Application.Services.Dtos;
using Application.Services.Logic.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;

namespace Application.Services.Logic.Implementations;

public class LayerRegionStyleService : ILayerRegionStyleService
{
    private readonly ILayerRegionStyleRepository  _layerRegionStyleRepository;
    private readonly ILogger<ILayerRegionStyleService> _logger;

    public LayerRegionStyleService(ILayerRegionStyleRepository layerRegionStyleRepository, 
        ILogger<ILayerRegionStyleService> logger)
    {
        _layerRegionStyleRepository = layerRegionStyleRepository;
        _logger = logger;
    }
    
    public async Task<Guid> AddAsync(Guid layerRegionId, LayerRegionStyleDto? styleDto, CancellationToken ct)
    {
        if (styleDto == null)
        {
            _logger.LogError("StyleDto is null");
            return Guid.Empty;
        }
        
        _logger.LogInformation("Creating Layer Region Style");

        var style = new LayerRegionStyle();

        style.RegionId = layerRegionId;
        style.Stroke = styleDto.Stroke;
        style.Color = styleDto.Color;
        style.Weight = styleDto.Weight;
        style.Opacity = styleDto.Opacity;
        style.LineCap = styleDto.LineCap;
        style.LineJoin = styleDto.LineJoin;
        style.DashArray = styleDto.DashArray;
        style.DashOffset = styleDto.DashOffset;
        style.Fill = styleDto.Fill;
        style.FillColor = styleDto.FillColor;
        style.FillOpacity = styleDto.FillOpacity;
        style.FillRule = styleDto.FillRule;
        style.ClassName = styleDto.ClassName;
        
        await _layerRegionStyleRepository.AddAsync(style, ct);
        
        return style.Id;
    }
    
    public async Task<LayerRegionStyleDto?> GetStyleByLayerIdAsync(Guid layerRegionId, CancellationToken ct)
    {
        var style = await _layerRegionStyleRepository.GetByLayerRegionIdAsync(layerRegionId, ct);
        
        if (style == null)
        {
            _logger.LogError("Style could not be found by layer {layerRegionId}", layerRegionId);
            return null;
        }

        return new LayerRegionStyleDto
        {
            Stroke = style.Stroke,
            Color = style.Color,
            ClassName = style.ClassName,
            FillColor = style.FillColor,
            FillOpacity = style.FillOpacity,
            FillRule = style.FillRule,
            LineCap = style.LineCap,
            LineJoin = style.LineJoin,
            Opacity = style.Opacity,
            Weight = style.Weight,
            DashArray = style.DashArray,
            DashOffset = style.DashOffset,
            Fill = style.Fill,
        };
    }

    public async Task<LayerRegionStyleDto?> UpdateAsync(Guid layerRegionId, LayerRegionStyleDto? styleDto, CancellationToken ct)
    {
        if (styleDto == null)
        {
            _logger.LogError("StyleDto is null");
            return null;
        }
        
        var style = await _layerRegionStyleRepository.GetByLayerRegionIdAsync(layerRegionId, ct);
        if (style == null)
        {
            _logger.LogError("Style could not be found by layer {layerRegionId}", layerRegionId);
            return null;
        }

        if (styleDto.Stroke != null) style.Stroke = styleDto.Stroke;
        if (styleDto.Color != null) style.Color = styleDto.Color;
        if (styleDto.ClassName != null) style.ClassName = styleDto.ClassName;
        if (styleDto.FillColor != null) style.FillColor = styleDto.FillColor;
        if (styleDto.FillOpacity != null) style.FillOpacity = styleDto.FillOpacity;
        if (styleDto.FillRule != null) style.FillRule = styleDto.FillRule;
        if (styleDto.LineCap != null) style.LineCap = styleDto.LineCap;
        if (styleDto.LineJoin != null) style.LineJoin = styleDto.LineJoin;
        if (styleDto.Opacity != null) style.Opacity = styleDto.Opacity;
        if (styleDto.Weight != null) style.Weight = styleDto.Weight;
        if (styleDto.DashArray != null) style.DashArray = styleDto.DashArray;
        if (styleDto.DashOffset != null) style.DashOffset = styleDto.DashOffset;
        if (styleDto.Fill != null) style.Fill = styleDto.Fill;
        
        await _layerRegionStyleRepository.UpdateAsync(style, ct);
        
        return styleDto;
    }

    public async Task DeleteByLayerIdAsync(Guid layerRegionId, CancellationToken ct)
    {
        var style = await _layerRegionStyleRepository.GetByLayerRegionIdAsync(layerRegionId, ct);
        if (style == null)
        {
            _logger.LogError("Style {styleId} could not be found", layerRegionId);
            return;
        }
        
        await _layerRegionStyleRepository.DeleteByLayerRegionIdAsync(layerRegionId, ct);
        
        _logger.LogInformation("Style {styleId} deleted", layerRegionId);
    }
}