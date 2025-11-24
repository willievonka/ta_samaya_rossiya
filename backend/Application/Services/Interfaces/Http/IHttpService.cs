using Application.Services.Dtos.Http;

namespace Application.Services.Interfaces.Http;

public interface IHttpService
{
    /// <summary>
    /// Интерфейс для сервиса, реализующего отправку запросов на api сервер
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// метод для отправки http запроса в Api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<HttpResponse<T>> SendHttpRequestAsync<T>(HttpRequestData requestData, CancellationToken ct) where T : class;
    }
}