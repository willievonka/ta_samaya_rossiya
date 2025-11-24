using Application.Services.Dtos.Http;
using Application.Services.Interfaces.Http;

namespace Infrastructure.Services.Implementations.Http;

/// <inheritdoc />
public class HttpConnectionService : IHttpConnectionService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpConnectionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public HttpClient CreateHttpClient(HttpConnectionData httpConnectionData)
    {
        var httpClient = string.IsNullOrWhiteSpace(httpConnectionData.ClientName)
            ? _httpClientFactory.CreateClient()
            : _httpClientFactory.CreateClient(httpConnectionData.ClientName);
        
        if (httpConnectionData.Timeout != null)
            httpClient.Timeout = httpConnectionData.Timeout.Value;
        
        return httpClient;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage, HttpClient httpClient, CancellationToken cancellationToken,
        HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead)
    {
        var response = await httpClient.SendAsync(httpRequestMessage, httpCompletionOption, cancellationToken);
        return response;
    }
}