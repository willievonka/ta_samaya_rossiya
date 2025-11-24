using Application.Services.Dtos.Http;

namespace Application.Services.Interfaces.Http;

/// <summary>
/// Функционал для Http соединений
/// </summary>
public interface IHttpConnectionService
{
    /// <summary>
    /// Создание Клиента для Http подключения
    /// </summary>
    /// <returns></returns>
    HttpClient CreateHttpClient(HttpConnectionData httpConnectionData);
    
    Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage, HttpClient httpClient,
        CancellationToken cancellationToken, HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead);
}