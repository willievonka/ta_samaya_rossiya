namespace Application.Services.Dtos.Http;

public record struct HttpConnectionData
{
    public HttpConnectionData(string? clientName = null)
    {
        ClientName = clientName;
        Timeout = null;
        CancellationToken = default;
    }
    
    public TimeSpan? Timeout { get; set; }
    public CancellationToken CancellationToken { get; set; }
    public string? ClientName { get; set; }
}