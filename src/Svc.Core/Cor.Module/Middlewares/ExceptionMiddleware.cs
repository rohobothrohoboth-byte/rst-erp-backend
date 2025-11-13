using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cor.Module.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await HandleDbConcurrencyExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private async Task HandleDomainExceptionAsync(HttpContext context, DomainException ex)
    {
        _logger.LogWarning(ex, "Domain exception occurred.");

        var errors = ex is ValidationException valEx ? valEx.Errors : [ex.Message];

        var response = new ApiResponse<string>(
            message: ex.Message,
            errors: errors,
            statusCode: ex.StatusCode
        )
        {
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));
    }

    private async Task HandleDbConcurrencyExceptionAsync(HttpContext context, DbUpdateConcurrencyException ex)
    {
        _logger.LogWarning(ex, "Concurrency conflict occurred.");

        var response = new ApiResponse<string>(
            message: "The resource was modified by another user. Please reload and try again.",
            errors: [ex.Message],
            statusCode: StatusCodes.Status409Conflict
        )
        {
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));
    }

    private async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occurred.");

        var response = new ApiResponse<string>(
            message: "An unexpected error occurred.",
            errors: [ex.Message],
            statusCode: StatusCodes.Status500InternalServerError
        )
        {
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));
    }
}