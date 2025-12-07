using Application.Services.Dtos;
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
        };
    }

    public static MapLayersFeatureCollectionResponse? MapDtoToResponse(MapDto? dto)
    {
        if (dto == null)
            return null;

        var response = new List<MapLayerResponse>();

        if (dto.Regions != null)
        {
            foreach (var region in dto.Regions)
            {
                var regionResponse = LayerRegionMapper.LayerRegionDtoToResponse(region);
                var geometry = region.Geometry;
                response.Add(new MapLayerResponse(geometry!,
                    regionResponse!));
            }
        }

        return new MapLayersFeatureCollectionResponse(response);
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
        };

        if (request.Regions != null)
        {
            var regionsDtos = new List<LayerRegionDto>();
            foreach (var updatingRegion in request.Regions)
            {
                regionsDtos.Add(LayerRegionMapper.CreateLayerRegionRequestToDto(updatingRegion));
            }
            mapDto.Regions = regionsDtos;
        }
        
        return mapDto;
    }

    public static List<MapCardResponse>? MapsDtosToMapsCardsResponse(List<MapDto>? mapDtos)
    {
        var response = new List<MapCardResponse>();
        
        if (mapDtos == null)
            return response;
        
        foreach (var mapDto in mapDtos)
        {
            response.Add(new MapCardResponse(
                mapDto.Id!.Value,
                mapDto.IsAnalytics,
                mapDto.Title,
                mapDto.Description,
                mapDto.BackgroundImagePath
                ));
        }

        return response;
    }
}