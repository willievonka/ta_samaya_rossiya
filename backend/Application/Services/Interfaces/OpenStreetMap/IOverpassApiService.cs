namespace Application.Services.Interfaces.OpenStreetMap;

public interface IOverpassApiService
{
    Task<string> GetOverpassApiResponse(string commands, CancellationToken ct = default);
}