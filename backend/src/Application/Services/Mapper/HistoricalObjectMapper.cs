using Application.Services.Dtos.HistoricalObject;
using Application.Services.Dtos.HistoricalObject.Requests;
using Application.Services.Dtos.HistoricalObject.Responses;
using NetTopologySuite.Geometries;

namespace Application.Services.Mapper;

public static class HistoricalObjectMapper
{
    public static List<HistoricalObjectDto>? UpsertHistoricalObjectsRequestToDtosList(List<UpsertHistoricalObjectRequest>? request, Guid? layerId = null)
    {
        if (request == null)
            return null;
        
        var list = new List<HistoricalObjectDto>();

        foreach (var point in request)
        {
            list.Add(UpsertHistoricalObjectRequestToDto(point, layerId));
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
            Coordinates: [ histObjectDto.Coordinates!.X, histObjectDto.Coordinates.Y ],
            Year: histObjectDto.Year!.Value,
            ImagePath: histObjectDto.ImagePath,
            Description: histObjectDto.Description!,
            ExcursionUrl: histObjectDto.ExcursionUrl
        );
    }

    public static HistoricalObjectDto UpsertHistoricalObjectRequestToDto(UpsertHistoricalObjectRequest request, Guid? layerId)
    {
        return new HistoricalObjectDto
        {
            Id = request.Id ?? Guid.Empty,
            LayerRegionId = layerId,
            Title = request.Title,
            Description = request.Description,
            Coordinates = request.Coordinates == null ? null : new Point(request.Coordinates[0], request.Coordinates[1]),
            Year = request.Year,
            ExcursionUrl = request.ExcursionUrl,
            Image = request.Image,
        };
    }
}