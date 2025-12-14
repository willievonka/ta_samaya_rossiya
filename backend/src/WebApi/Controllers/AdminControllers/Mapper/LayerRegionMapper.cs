using Application.Services.Dtos;
using WebApi.Controllers.AdminControllers.HistoricalObject.Response;
using WebApi.Controllers.AdminControllers.LayerRegion.Request;
using WebApi.Controllers.AdminControllers.LayerRegion.Response;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Mapper;

public static class LayerRegionMapper
{
    public static MapLayerPropertiesResponse? LayerRegionDtoToResponse(LayerRegionDto? layerRegionDto, bool isAnalyticsMap = false)
    {
        if (layerRegionDto == null)
            return null;

        if (!layerRegionDto.IsActive!.Value && isAnalyticsMap)
        {
            return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name);
        }
        
        var style = LayerRegionStyleMapper.StyleDtoToResponse(layerRegionDto.Style);

        AnalyticsMapLayerPropertiesResponse? analiticsProperties = null;
        var indicators = layerRegionDto.Indicators;
        if (indicators != null)
        {
            analiticsProperties = new AnalyticsMapLayerPropertiesResponse(indicators.ImagePath!, indicators.Partners!.Value, 
                indicators.Excursions!.Value, indicators.Participants!.Value);
        }
        
        List<HistoricalObjectResponse>? points = null;
        if (layerRegionDto.HistoricalObjects != null && !isAnalyticsMap)
        {
            points = new List<HistoricalObjectResponse>();

            foreach (var historicalObject in layerRegionDto.HistoricalObjects)
            {
                points.Add(HistoricalObjectMapper.HistoricalObjectDtoToResponse(historicalObject)!);
            }
        }
        
        return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name, null, 
            style, analiticsProperties, points);
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

    public static IndicatorsRegionDto? IndicatorsRegionRequestToDto(UpsertIndicatorsRequest? request)
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