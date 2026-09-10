using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Netwise_Task;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException &&
            httpContext.RequestAborted.IsCancellationRequested)
        {
            return false;
        }

        var (statusCode, title, detail) = MapException(exception);

        logger.LogError(
            exception,
            "Request failed with status code {StatusCode}.",
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(
        Exception exception)
    {
        return exception switch
        {
            HttpRequestException =>
                (StatusCodes.Status502BadGateway,
                "External service unavailable",
                "The Cat Facts service could not be reached."),
            JsonException =>
                (StatusCodes.Status502BadGateway,
                "Invalid external response",
                "The Cat Facts service returned an invalid response."),
            Azure.RequestFailedException =>
                (StatusCodes.Status503ServiceUnavailable,
                "Storage unavailable",
                "Azure Blob Storage is temporarily unavailable."),
            OperationCanceledException or TimeoutException =>
                (StatusCodes.Status504GatewayTimeout,
                "External service timeout",
                "The external service did not respond in time."),
            IOException =>
                (StatusCodes.Status500InternalServerError,
                "Storage error",
                "The cat fact could not be persisted."),
            _ =>
                (StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred.")
        };
    }
}
