namespace Application.Services.Dtos.Http;

public record HttpResponse<TResponse> : BaseHttpResponse
{
    /// <summary>
    ///     Тело ответа
    /// </summary>
    public TResponse? Body { get; set; }
}