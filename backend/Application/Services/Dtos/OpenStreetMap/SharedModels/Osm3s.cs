using Newtonsoft.Json;

namespace Application.Services.Dtos.OpenStreetMap.SharedModels;

public class Osm3s
{
    [JsonProperty("timestamp_osm_base")]  // Указывает соответствие JSON-поля
    public DateTime TimestampOsmBase { get; set; }
    
    public string Copyright { get; set; } = null!;
}