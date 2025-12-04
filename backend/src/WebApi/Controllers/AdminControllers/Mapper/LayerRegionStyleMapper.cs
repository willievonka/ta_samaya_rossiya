using Application.Services.Dtos;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Mapper;

public static class LayerRegionStyleMapper
{
    public static LayerRegionStyleDto? StyleRequestToDto(CreateLayerRegionStyleRequest? request)
    {
        if (request == null)
            return null;
        
        if (request.Equals(new CreateLayerRegionStyleRequest(null, null, null, null, null, 
                null, null, null, null, null, null, null, null)))
        {
            return null;
        }
        
        return new LayerRegionStyleDto
        {
            Stroke = request.Stroke,
            Fill = request.Fill,
            ClassName = request.ClassName,
            Color = request.Color,
            DashArray = request.DashArray,
            DashOffset = request.DashOffset,
            FillOpacity = request.FillOpacity,
            FillColor = request.FillColor,
            FillRule = request.FillRule,
            Opacity = request.Opacity,
            Weight = request.Weight,
            LineCap = request.LineCap,
            LineJoin = request.LineJoin,
        };
    }

    public static LayerRegionStyleResponse? StyleDtoToResponse(LayerRegionStyleDto? styleDto)
    {
        if (styleDto == null)
            return null;
        
        return new LayerRegionStyleResponse(
            Stroke: styleDto.Stroke,
            Color: styleDto.Color,
            Weight: styleDto.Weight,
            Opacity: styleDto.Opacity,
            LineCap: styleDto.LineCap,
            LineJoin: styleDto.LineJoin,
            DashArray: styleDto.DashArray,
            DashOffset: styleDto.DashOffset,
            Fill: styleDto.Fill,
            FillColor: styleDto.FillColor,
            FillOpacity: styleDto.FillOpacity,
            FillRule: styleDto.FillRule,
            ClassName: styleDto.ClassName);
    }
}