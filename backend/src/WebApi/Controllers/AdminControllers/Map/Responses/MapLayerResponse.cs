using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapLayerResponse(
    Geometry Geometry,
    MapLayerPropertiesResponse Properties
)
{
    [JsonPropertyOrder(-1)]
    public string Type => "Feature";// изменить в обязательнные поля
};