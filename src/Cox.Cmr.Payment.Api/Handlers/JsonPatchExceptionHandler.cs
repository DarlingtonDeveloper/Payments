using System.Net.Mime;
using Cox.Cdp.Api.Exceptions.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Serilog.Context;

namespace Cox.Cmr.Payment.Api.Handlers;

[ExcludeFromCodeCoverage]
public class JsonPatchExceptionHandler(ILogger<JsonPatchExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not JsonPatchException jsonPatchException)
        {
            return false;
        }

        var problemDetails = jsonPatchException.ToProblemDetails(httpContext.Request);

        using (LogContext.PushProperty("JsonPatchExceptionPath", jsonPatchException.FailedOperation?.path))
        using (LogContext.PushProperty("JsonPatchExceptionOperationType",
                   jsonPatchException.FailedOperation?.OperationType))
        {
            logger.LogError("Json Patch failed - {JsonPatchErrorMessage}", jsonPatchException.Message);
        }

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        await httpContext.Response.WriteAsync(
            JsonConvert.SerializeObject(problemDetails),
            cancellationToken);

        return true;
    }
}
