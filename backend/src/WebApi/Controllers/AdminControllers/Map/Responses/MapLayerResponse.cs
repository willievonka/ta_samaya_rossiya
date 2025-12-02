using NetTopologySuite.Geometries;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapLayerResponse(
    Geometry GeoData,
    MapLayerPropertiesResponse Properties
)
{
    public string Type => "Feature";
};