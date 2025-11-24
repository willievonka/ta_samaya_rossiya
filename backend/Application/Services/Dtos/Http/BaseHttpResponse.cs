using System.Net;
using System.Net.Http.Headers;

namespace Application.Services.Dtos.Http;

/// <summary>
/// Результат выполнения действия сервиса
/// </summary>
public record BaseHttpResponse
{
    /// <summary>
    ///     Статус ответа
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    ///     Заголовки, передаваемые в ответе
    /// </summary>
    public HttpResponseHeaders? Headers { get; set; }

    /// <summary>
    ///     Заголовки контента
    /// </summary>
    public HttpContentHeaders? ContentHeaders { get; init; }

    /// <summary>
    ///     Является ли статус код успешным
    /// </summary>
    public bool IsSuccessStatusCode
    {
        get
        {
            var statusCode = (int)StatusCode;

            return statusCode >= 200 && statusCode <= 299;
        }
    }
}