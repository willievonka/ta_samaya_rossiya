using Application.Services.Dtos;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Map.Mapper;

public static class LayerRegionMapper
{
    public static MapLayerPropertiesResponse? LayerRegionDtoToResponse(LayerRegionDto? layerRegionDto)
    {
        if (layerRegionDto == null)
            return null;

        if (!layerRegionDto.IsActive)
        {
            return new MapLayerPropertiesResponse(layerRegionDto.Name, layerRegionDto.Id);
        }
        
        var style = new PathOptionsResponse(layerRegionDto.FillColor);

        var indicators = layerRegionDto.Indicators;
        if (indicators != null)
        {
            var analiticsProperties = new AnalyticsMapLayerPropertiesResponse(indicators.ImagePath!, indicators.Partners, 
                indicators.Excursions, indicators.Participants);
            
            return new MapLayerPropertiesResponse(layerRegionDto.Name, layerRegionDto.Id, layerRegionDto.IsActive, 
                style, analiticsProperties);
        }
        
        return new MapLayerPropertiesResponse(layerRegionDto.Name, layerRegionDto.Id, layerRegionDto.IsActive, 
            style, null);
    }

    public static LayerRegionDto LayerRegionRequestToDto(CreateLayerRegionRequest request)
    {
        return new LayerRegionDto
        {
            IsActive = request.IsActive,
            Name = request.Name,
            FillColor = request.FillColor,
            Indicators = IndicatorsRegionRequestToDto(request.Indicators),
        };
    }

    public static IndicatorsRegionDto? IndicatorsRegionRequestToDto(CreateIndicatorsRequest? request)
    {
        if  (request == null)
            return null;

        if (request.IsActive == null || request.Excursions == null 
            || request.Participants == null || request.Partners == null)
            return null;
        
        return new IndicatorsRegionDto
        {
            IsActive = (bool)request.IsActive,
            Image = request.Image,
            Excursions = (int)request.Excursions,
            Participants = (int)request.Participants,
            Partners = (int)request.Partners,
        };
    }
}