namespace Cox.Cmr.Payment.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public abstract class DomainException(string message, HttpStatusCode httpStatusCode, LogLevel logLevel = LogLevel.Error) : Exception(message)
{
    public LogLevel LogLevel { get; } = logLevel;
    public HttpStatusCode HttpStatusCode { get; } = httpStatusCode;
}
