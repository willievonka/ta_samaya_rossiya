using NetTopologySuite.Geometries;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapLayerResponse(
    Geometry Geometry,
    MapLayerPropertiesResponse Properties
)
{
    public string Type => "Feature";// изменить в обязательнные поля
};