using Helpers;
using System.Text.Json;

namespace Cor.Module.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly List<IExceptionHandler> _handlers;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        _handlers = new List<IExceptionHandler>
        {
            new UnauthorizedExceptionHandler(),
            new ForbiddenExceptionHandler(),
            new ConflictExceptionHandler(),
            new DomainExceptionHandler(),
            new NotFoundExceptionHandler(),
            new BadRequestExceptionHandler(),
            new DatabaseExceptionHandler(),
            new FallbackExceptionHandler()
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception caught in middleware.");
            var handler = _handlers.First(h => h.CanHandle(ex));
            var apiResponse = await handler.HandleAsync(ex, context);
            context.Response.StatusCode = apiResponse.StatusCode ?? 500;
            context.Response.ContentType = "application/json";

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, jsonOptions));
        }
    }
}