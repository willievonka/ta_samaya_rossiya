using Application.Services.Dtos;
using WebApi.Controllers.AdminControllers.LayerRegion.Request;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Mapper;

public static class LayerRegionMapper
{
    public static MapLayerPropertiesResponse? LayerRegionDtoToResponse(LayerRegionDto? layerRegionDto)
    {
        if (layerRegionDto == null)
            return null;

        if (!layerRegionDto.IsActive!.Value)
        {
            return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name);
        }
        
        var style = LayerRegionStyleMapper.StyleDtoToResponse(layerRegionDto.Style);

        var indicators = layerRegionDto.Indicators;
        if (indicators != null)
        {
            var analiticsProperties = new AnalyticsMapLayerPropertiesResponse(indicators.ImagePath!, indicators.Partners, 
                indicators.Excursions, indicators.Participants);
            
            return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name,  layerRegionDto.IsActive, 
                style, analiticsProperties);
        }
        
        return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name, layerRegionDto.IsActive, 
            style, null);
    }

    public static LayerRegionDto CreateLayerRegionRequestToDto(CreateLayerRegionRequest request)
    {
        return new LayerRegionDto
        {
            IsActive = request.IsActive,
            Name = request.RegionName,
            Style = LayerRegionStyleMapper.StyleRequestToDto(request.Style),
            Indicators = IndicatorsRegionRequestToDto(request.AnalyticsData),
        };
    }

    public static LayerRegionDto UpdateAnalyticsDataRegionToLayerRegionDto(UpdateAnalyticsDataRegionRequest request)
    {
        return new LayerRegionDto
        {
            Name = "",
            Indicators = IndicatorsRegionRequestToDto(request.AnalyticsData),
        };
    }
    
    public static LayerRegionDto UpdateLayerRegionRequestToDto(UpdateLayerRegionRequest request)
    {
        return new LayerRegionDto
        {
            Name = "",
            IsActive = request.IsActive,
            Style = LayerRegionStyleMapper.StyleRequestToDto(request.Style),
            Indicators = IndicatorsRegionRequestToDto(request.AnalyticsData),
        };
    }

    public static IndicatorsRegionDto? IndicatorsRegionRequestToDto(CreateIndicatorsRequest? request)
    {
        if  (request == null)
            return null;

        if (request.IsActive == null || request.ExcursionsCount == null 
            || request.MembersCount == null || request.PartnersCount == null)
            return null;
        
        return new IndicatorsRegionDto
        {
            IsActive = (bool)request.IsActive,
            Image = request.Image,
            Excursions = (int)request.ExcursionsCount,
            Participants = (int)request.MembersCount,
            Partners = (int)request.PartnersCount,
        };
    }
}