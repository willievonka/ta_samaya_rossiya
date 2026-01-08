using Application.Services.Dtos;
using WebApi.Controllers.AdminControllers.LayerRegion.Response;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Mapper;

public static class MapMapper
{
    public static MapDto? CreateMapRequestToDto(CreateMapRequest? request)
    {
        if (request == null)
            return null;

        return new MapDto
        {
            IsAnalytics = request.IsAnalytics,
            BackgroundImage = request.BackgroundImage,
            Title = request.Title,
            Description = request.Description,
            Info = request.InfoText,
            ActiveLayerRegionsColor = request.ActiveLayerColor,
            HistoricalObjectPointColor = request.PointColor,
        };
    }

    public static MapPageResponse? EmptyMapDtoToResponse(MapDto? mapDto)
    {
        if (mapDto == null)
            return null;

        var features = new List<MapLayerResponse>();

        if (mapDto.Regions != null)
        {
            foreach (var region in mapDto.Regions)
            {
                var regionResponse = LayerRegionMapper.BasicRegionDtoToResponse(region);
                
                var geometry = region.Geometry;
                features.Add(new MapLayerResponse(geometry,
                    regionResponse!));
            }
        }
        
        var layers = new MapLayersFeatureCollectionResponse(features);

        return new MapPageResponse(layers, "", "", null, null);
    }
    
    public static MapPageResponse? MapDtoToResponse(MapDto? dto)
    {
        if (dto == null)
            return null;

        var features = new List<MapLayerResponse>();

        if (dto.Regions != null)
        {
            foreach (var region in dto.Regions)
            {
                var regionResponse = dto.IsAnalytics != null && dto.IsAnalytics!.Value
                    ? LayerRegionMapper.LayerRegionDtoToResponse(region, true)
                    : LayerRegionMapper.LayerRegionDtoToResponse(region);
                
                var geometry = region.Geometry;
                features.Add(new MapLayerResponse(geometry,
                    regionResponse!));
            }
        }
        
        var layers = new MapLayersFeatureCollectionResponse(features);

        return new MapPageResponse(layers, dto.Title!, dto.Info!,
            dto.ActiveLayerRegionsColor, dto.HistoricalObjectPointColor);
    }

    public static MapDto? UpdateMapCardRequestToDto(UpdateMapCardRequest? request, Guid mapId)
    {
        if (request == null)
            return null;

        return new MapDto
        {
            Id = mapId,
            Title = request.Title,
            Description = request.Description,
            IsAnalytics = request.IsAnalytics,
            BackgroundImage = request.BackgroundImage,
        };
    }

    public static MapDto? UpdateMapRequestToDto(UpdateMapRequest? request, Guid mapId)
    {
        if (request == null)
            return null;

        var mapDto = new MapDto
        {
            Id = mapId,
            IsAnalytics = request.IsAnalytics,
            BackgroundImage = request.BackgroundImage,
            Title = request.Title,
            Description = request.Description,
            Info = request.InfoText,
            ActiveLayerRegionsColor = request.ActiveLayerColor,
            HistoricalObjectPointColor = request.PointColor,
        };

        if (request.Layers != null)
        {
            var regionsDtos = new List<LayerRegionDto>();
            foreach (var updatingRegion in request.Layers)
            {
                regionsDtos.Add(LayerRegionMapper.UpdateLayerRegionRequestToDto(updatingRegion));
            }
            mapDto.Regions = regionsDtos;
        }
        
        return mapDto;
    }

    public static List<MapCardResponse> MapsDtosToMapsCardsResponse(List<MapDto>? mapDtos)
    {
        var response = new List<MapCardResponse>();
        
        if (mapDtos == null)
            return response;
        
        foreach (var mapDto in mapDtos)
        {
            response.Add(new MapCardResponse(
                mapDto.Id!.Value,
                mapDto.IsAnalytics,
                mapDto.Title!,
                mapDto.Description!,
                mapDto.BackgroundImagePath
                ));
        }

        return response;
    }
}