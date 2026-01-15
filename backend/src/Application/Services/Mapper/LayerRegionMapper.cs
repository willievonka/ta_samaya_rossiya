using Application.Services.Dtos.HistoricalObject.Responses;
using Application.Services.Dtos.Indicators;
using Application.Services.Dtos.LayerRegion;
using Application.Services.Dtos.LayerRegion.Requests;
using Application.Services.Dtos.LayerRegion.Responses;

namespace Application.Services.Mapper;

public static class LayerRegionMapper
{
    public static MapLayerPropertiesResponse? BasicRegionDtoToResponse(LayerRegionDto? layerRegionDto)
    {
        if (layerRegionDto == null)
            return null;

        return new MapLayerPropertiesResponse(Guid.Empty, layerRegionDto.Name);
    }
    
    public static MapLayerPropertiesResponse? LayerRegionDtoToResponse(LayerRegionDto? layerRegionDto,
        bool isAnalyticsMap = false)
    {
        if (layerRegionDto == null)
            return null;

        bool? isActive = null;
        
        if (isAnalyticsMap)
        {
            if (!layerRegionDto.IsActive!.Value)
            {
                return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name);
            }
            
            isActive = layerRegionDto.IsActive;
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
        
        return new MapLayerPropertiesResponse(layerRegionDto.Id!.Value, layerRegionDto.Name, isActive, 
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
            HistoricalObjects = HistoricalObjectMapper.UpsertHistoricalObjectsRequestToDtosList(request.Points)
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
    
    public static LayerRegionDto UpdateLayerRegionRequestToDto(UpsertLayerRegionRequest request)
    {
        return new LayerRegionDto
        {
            Id = request.Id ?? Guid.Empty,
            Name = request.RegionName ?? "",
            IsActive = request.IsActive,
            Style = LayerRegionStyleMapper.StyleRequestToDto(request.Style),
            Indicators = IndicatorsRegionRequestToDto(request.AnalyticsData),
            HistoricalObjects = HistoricalObjectMapper.UpsertHistoricalObjectsRequestToDtosList(request.Points)
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