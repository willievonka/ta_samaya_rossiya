using System.Text.Json.Serialization;
using Application.Services.Dtos;

namespace WebApi.Controllers.AdminControllers.Map.Responses;

public record MapPageResponse
{
    public MapPageResponse(MapLayersFeatureCollectionResponse layers,
        string pageTitle, string infoText)
    {
        this.Layers = layers;
        this.PageTitle = pageTitle;
        this.InfoText = infoText;
    }
    
    [JsonPropertyOrder(0)]
    public string PageTitle { get; }
    
    [JsonPropertyOrder(1)]
    public string InfoText { get; }
    
    [JsonPropertyOrder(2)]
    public MapLayersFeatureCollectionResponse Layers { get; }
}