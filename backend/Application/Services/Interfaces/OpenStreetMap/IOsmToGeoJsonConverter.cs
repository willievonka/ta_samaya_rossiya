namespace Application.Services.Interfaces.OpenStreetMap;

public interface IOsmToGeoJsonConverter
{
    Task<string> OsmToGeoJsonAsync(string osmData, CancellationToken ct = default);
}