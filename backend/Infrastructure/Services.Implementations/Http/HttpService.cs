using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using Application.Services.Dtos.Http;
using Application.Services.Interfaces.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Retry;
using Serilog;
using ContentType = Application.Services.Http.Enums.ContentType;

namespace Infrastructure.Services.Implementations.Http;

public class HttpService : IHttpService
{
    public readonly Dictionary<string, string> DefaultHeaders;
    private AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    
    public HttpService()
    {
        DefaultHeaders = new Dictionary<string, string>();
        _retryPolicy = Policy.Handle<HttpRequestException>().Or<SocketException>().Or<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(response => (int)response.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt * 2),
                onRetry: (result, delay, attempt, context) =>
                {
                    Log.Error($"Http request retry {attempt} due to {result.Exception?.Message}. Waiting {delay} before next retry.");
                });
    }
    
    public async Task<HttpResponse<T>> SendHttpRequestAsync<T>(HttpRequestData requestData) where T : class
    {
        /*var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            UseCookies = true
        };
        using (var httpClient = new HttpClient(handler))
        {
            foreach (var header in DefaultHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            
            var uri = requestData.Uri;
            
            var request = new HttpRequestMessage
            {
                Method = requestData.Method,
                RequestUri = GetUriWithQuery(new Uri(uri), requestData.QueryParameterList)
            };
            
            if (requestData.Body != null)
            {
                request.Content = PrepairContent(requestData.Body, requestData.ContentType);
            }
            
            var response = await _retryPolicy.ExecuteAsync(async () => await httpClient.SendAsync(request));
            var contentString = await response.Content.ReadAsStringAsync();
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            
            if (!response.IsSuccessStatusCode)
            {
                Log.Error($"> HTTP request {request.Method.Method} {request.RequestUri} failed with response code {response.StatusCode}.");
                return new BaseHttpResponse<T>
                {
                    Item = null,
                    Completed = false,
                    Message = $"HTTP request {request.Method.Method} {request.RequestUri} failed with response code {response.StatusCode}."
                };
            }
            
            try
            {
                var result = JsonConvert.DeserializeObject<T>(contentString, jsonSettings);
                Log.Information($"HTTP request {request.Method.Method} {request.RequestUri} sent successfully with response code {response.StatusCode}.");
                return new BaseHttpResponse<T>
                {
                    Item = result,
                    Completed = true,
                    Message = ""
                };
            }
            catch (Exception ex) when (ex is JsonSerializationException || ex is JsonReaderException)
            {
                Log.Warning($"HTTP response {request.Method.Method} {request.RequestUri} couldn't deserialize to type {typeof(T).Name}.");
                return new BaseHttpResponse<T>
                {
                    Item = null,
                    Completed = false,
                    Message = $"HTTP response {request.Method.Method} {request.RequestUri} couldn't deserialize to type {typeof(T).Name}."
                };
            }
        }*/
        throw new NotImplementedException("Http service is not implemented");
    }
    
    private static Uri GetUriWithQuery(Uri uri, ICollection<KeyValuePair<string, string>> queryParameterList)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(uri);
        var isFirstQuery = true;
        foreach (var queryKvPair in queryParameterList)
        {
            if (isFirstQuery)
            {
                stringBuilder.Append('?');
                isFirstQuery = false;
            }
            else
            {
                stringBuilder.Append('&');
            }
            stringBuilder.Append(queryKvPair.Key);
            stringBuilder.Append('=');
            stringBuilder.Append(queryKvPair.Value);
        }

        return new Uri(stringBuilder.ToString());
    }
    
    private static HttpContent PrepairContent(object body, ContentType contentType)
    {
        switch (contentType)
        {
            case ContentType.ApplicationJson:
            {
                if (body is string stringBody)
                {
                    body = JToken.Parse(stringBody);
                }

                var serializeSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                var serializedBody = JsonConvert.SerializeObject(body, serializeSettings);
                var content = new StringContent(serializedBody, Encoding.UTF8, MediaTypeNames.Application.Json);
                return content;
            }

            case ContentType.XWwwFormUrlEncoded:
            {
                if (body is not IEnumerable<KeyValuePair<string, string>> list)
                {
                    throw new Exception(
                        $"Body for content type {contentType} must be {typeof(IEnumerable<KeyValuePair<string, string>>).Name}");
                }

                return new FormUrlEncodedContent(list);
            }
            case ContentType.ApplicationXml:
            {
                if (body is not string s)
                {
                    throw new Exception($"Body for content type {contentType} must be XML string");
                }

                return new StringContent(s, Encoding.UTF8, MediaTypeNames.Application.Xml);
            }
            case ContentType.Binary:
            {
                if (body.GetType() != typeof(byte[]))
                {
                    throw new Exception($"Body for content type {contentType} must be {typeof(byte[]).Name}");
                }

                return new ByteArrayContent((byte[])body);
            }
            case ContentType.TextXml:
            {
                if (body is not string s)
                {
                    throw new Exception($"Body for content type {contentType} must be XML string");
                }

                return new StringContent(s, Encoding.UTF8, MediaTypeNames.Text.Xml);
            }
            case ContentType.MultipartFormData:
                return (MultipartFormDataContent)body;
            default:
                throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
        }
    }
}