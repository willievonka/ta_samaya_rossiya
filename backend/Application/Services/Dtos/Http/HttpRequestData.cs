using Application.Services.Http.Enums;

namespace Application.Services.Dtos.Http;

public record HttpRequestData
{
    /// <summary>
    /// Тип метода
    /// </summary>
    public required HttpMethod Method { get; init; } = HttpMethod.Get;

    /// <summary>
    /// Адрес запроса
    /// </summary>\
    public Uri Uri { init; get; } = default!;

    /// <summary>
    /// Тело метода
    /// </summary>
    public object? Body { get; set; }
    
    /// <summary>
    /// content-type, указываемый при запросе
    /// </summary>
    public ContentType ContentType { get; set; } = ContentType.ApplicationJson;

    /// <summary>
    /// Заголовки, передаваемые в запросе
    /// </summary>
    public IDictionary<string, string> HeaderDictionary { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Коллекция параметров запроса
    /// </summary>
    public ICollection<KeyValuePair<string, string>> QueryParameterList { get; set; } =
        new List<KeyValuePair<string, string>>();
}