using System.Text.Json.Serialization;
using WebApi.Controllers.AdminControllers.LayerRegion.Response;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapPageResponse
{
    public MapPageResponse(MapLayersFeatureCollectionResponse layers,
        string pageTitle, string infoText, string? activeLayerColor, string? pointColor)
    {
        this.Layers = layers;
        this.PageTitle = pageTitle;
        this.InfoText = infoText;
        this.ActiveLayerColor = activeLayerColor;
        this.PointColor = pointColor;
    }
    
    [JsonPropertyOrder(0)]
    public string PageTitle { get; }
    
    [JsonPropertyOrder(1)]
    public string InfoText { get; }
    
    [JsonPropertyOrder(2)]
    public MapLayersFeatureCollectionResponse Layers { get; }
    
    [JsonPropertyOrder(3)]
    public string? ActiveLayerColor { get; }
    
    [JsonPropertyOrder(4)]
    public string? PointColor { get; }
}