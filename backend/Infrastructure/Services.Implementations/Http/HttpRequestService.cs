using System.Web;
using Application.Services.Dtos.Http;
using Application.Services.Interfaces.Http;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Services.Implementations.Http;

/// <inheritdoc />
public class HttpRequestService : IHttpRequestService
{
    private readonly IHttpConnectionService _httpConnectionService;
    private readonly IEnumerable<ITraceWriter> _traceWriters;

    public HttpRequestService(IHttpConnectionService httpConnectionService, IEnumerable<ITraceWriter> traceWriters)
    {
        _httpConnectionService = httpConnectionService;
        _traceWriters = traceWriters;
    }
    
    // TODO
    /// <inheritdoc />
    public Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(HttpRequestData requesData,
        HttpConnectionData connectionData = default)
    {
        var client = _httpConnectionService.CreateHttpClient(connectionData);

        var uriBuilder = new UriBuilder(requesData.Uri);

        if (requesData.QueryParameterList.Any() == true)
        {
            var qp = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var kv in requesData.QueryParameterList)
                qp[kv.Key] = kv.Value;
            uriBuilder.Query = qp.ToString();
        }

        throw new NotImplementedException("Not implemented");
    }
}