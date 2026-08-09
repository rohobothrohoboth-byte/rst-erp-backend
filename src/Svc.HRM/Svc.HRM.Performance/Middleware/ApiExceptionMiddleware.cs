using System.Net;
using System.Text.Json;

namespace Svc.HRM.Performance.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(context, ex, logger);
        }
    }

    internal static async Task WriteErrorAsync(HttpContext context, Exception ex, ILogger logger)
    {
        if (context.Response.HasStarted) throw ex;

        var (status, title) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Not Found"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid Operation"),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            _ => (HttpStatusCode.InternalServerError, "Server Error")
        };

        if (status == HttpStatusCode.InternalServerError)
            logger.LogError(ex, "Unhandled exception");
        else
            logger.LogWarning(ex, "Request failed: {Message}", ex.Message);

        context.Response.Clear();
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        var payload = new
        {
            success = false,
            title,
            message = ex.Message,
            status = (int)status
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
