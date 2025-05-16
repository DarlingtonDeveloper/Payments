namespace Cox.Cmr.Payment.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class HttpClientException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public HttpMethod? Method { get; }
    public Uri? Uri { get; }

    public string? ResponseBody { get; init; } = null;

    public HttpClientException(HttpStatusCode status, HttpMethod? method, Uri? uri) : base($"Failed to call {method} {uri} with {status}")
    {
        StatusCode = status;
        Method = method;
        Uri = uri;
    }

}
