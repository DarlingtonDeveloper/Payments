namespace Cox.Cmr.Payment.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException(string message) : DomainException($"Not Found: {message}",
    HttpStatusCode.NotFound, Microsoft.Extensions.Logging.LogLevel.Information);

