namespace Application.Services.Dtos.OpenStreetMap.SharedModels;

public class BaseOverpassApiResponse
{
    public string Version { get; set; } = null!;
    
    public string Generator { get; set; } = null!;
    
    public Osm3s Osm3s { get; set; } = null!;
}