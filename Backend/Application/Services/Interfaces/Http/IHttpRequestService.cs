using Application.Services.Dtos.Http;

namespace Application.Services.Interfaces.Http;

/// <summary>
///     Отправка HTTP запросов и обработка ответов
/// </summary>
public interface IHttpRequestService
{
    Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(HttpRequestData requestData,
        HttpConnectionData connectionData = default);
}