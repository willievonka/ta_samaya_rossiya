using Application.Services.Dtos;
using NetTopologySuite.Geometries;
using WebApi.Controllers.AdminControllers.HistoricalObject.Request;
using WebApi.Controllers.AdminControllers.HistoricalObject.Response;

namespace WebApi.Controllers.AdminControllers.Mapper;

public static class HistoricalObjectMapper
{
    public static List<HistoricalObjectDto> CreateHistoricalObjectsRequestToDtosList(CreateHistoricalObjectsRequest request)
    {
        var list = new List<HistoricalObjectDto>();

        foreach (var point in request.Points)
        {
            list.Add(CreateHistoricalObjectRequestToDto(point));
        }
        
        return list;
    }

    public static List<HistoricalObjectResponse>? HistoricalObjectsListDtoToResponse(
        List<HistoricalObjectDto>? historicalObjects)
    {
        if (historicalObjects == null)
            return null;
        
        var list = new List<HistoricalObjectResponse>();

        foreach (var historicalObject in historicalObjects)
        {
            list.Add(HistoricalObjectDtoToResponse(historicalObject)!);
        }
        
        return list;
    }

    public static HistoricalObjectResponse? HistoricalObjectDtoToResponse(HistoricalObjectDto? histObjectDto)
    {
        if (histObjectDto == null)
            return null;

        return new HistoricalObjectResponse
        (
            Id: histObjectDto.Id!.Value,
            Title: histObjectDto.Title!,
            Coordinates: [ histObjectDto.Coordinates.X, histObjectDto.Coordinates.Y ],
            Year: histObjectDto.Year!.Value,
            ImagePath: histObjectDto.ImagePath,
            Description: histObjectDto.Description!,
            ExcursionUrl: histObjectDto.ExcursionUrl
        );
    }

    public static HistoricalObjectDto CreateHistoricalObjectRequestToDto(CreateHistoricalObjectRequest request)
    {
        return new HistoricalObjectDto
        {
            LayerRegionId = request.LayerRegionId,
            Title = request.Title,
            Description = request.Description,
            Coordinates = new Point(request.Coordinates[0], request.Coordinates[1]),
            Year = request.Year,
            ExcursionUrl = request.ExcursionUrl,
            Image = request.Image,
        };
    }

    public static HistoricalObjectDto UpdateHistoricalObjectRequestToDto(UpdateHistoricalObjectRequest request, Guid pointId)
    {
        return new HistoricalObjectDto
        {
            Id = pointId,
            Title = request.Title,
            Description = request.Description,
            Year = request.Year,
            ExcursionUrl = request.ExcursionUrl,
            Image = request.Image,
        };
    }
}