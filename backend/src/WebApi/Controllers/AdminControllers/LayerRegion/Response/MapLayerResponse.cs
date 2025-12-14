using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using WebApi.Controllers.AdminControllers.Map.Responses;

namespace WebApi.Controllers.AdminControllers.LayerRegion.Response;

public record MapLayerResponse(
    Geometry Geometry,
    MapLayerPropertiesResponse Properties
)
{
    [JsonPropertyOrder(-1)]
    public string Type => "Feature";// изменить в обязательнные поля
};