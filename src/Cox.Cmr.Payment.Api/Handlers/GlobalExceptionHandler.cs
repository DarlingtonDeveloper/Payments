using System.Net.Mime;
using Cox.Cdp.Api.Exceptions.Extensions;
using Cox.Cmr.Payment.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Serilog.Context;

namespace Cox.Cmr.Payment.Api.Handlers;

[ExcludeFromCodeCoverage]
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment hostEnvironment)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        LogException(exception, httpContext);
        httpContext.Response.StatusCode = GetExceptionStatus(exception);

        if (hostEnvironment.IsProduction())
        {
            return true;
        }

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        await httpContext.Response.WriteAsync(
            JsonConvert.SerializeObject(exception.ToProblemDetails(httpContext.Request, httpContext.Response.StatusCode,
                "Payment Api Error")),
            cancellationToken: cancellationToken);

        return true;
    }

    private static int GetExceptionStatus(Exception exception) =>
        exception switch
        {
            HttpClientException httpClientException => (int)httpClientException.StatusCode,
            DomainException domainException => (int)domainException.HttpStatusCode,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };

    private void LogException(Exception exception, HttpContext httpContext)
    {
        switch (exception)
        {
            case HttpClientException e:
                using (LogContext.PushProperty("FailedUri", e.Uri))
                using (LogContext.PushProperty("FailedStatusCode", e.StatusCode))
                using (LogContext.PushProperty("FailedResponseBody", e.ResponseBody))
                using (LogContext.PushProperty("FailedMethod", e.Method))
                {
                    logger.LogError(exception, "Http Client error: {Message}", exception.Message);
                }

                break;
            case ValidationException vex:
                using (LogContext.PushProperty("ValidationException", vex, destructureObjects: true))
                {
                    logger.LogWarning("Validation failed");
                }

                break;
            case DomainException domEx:
                using (LogContext.PushProperty("DomainException", domEx))
                {
                    logger.Log(domEx.LogLevel, domEx.Message);
                }

                break;
            default:
                logger.LogError(exception, "Failed to handle request: {Message}", exception.Message);
                break;
        }
    }
}
