using Application.Services.Dtos;
using NetTopologySuite.Geometries;
using WebApi.Controllers.AdminControllers.Map.Requests;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.Map.Mapper;

public static class MapMapper
{
    public static MapDto? MapRequestToDto(CreateMapRequest? request)
    {
        if (request == null)
            return null;

        return new MapDto
        {
            IsAnalitics = request.IsAnalytics,
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
}