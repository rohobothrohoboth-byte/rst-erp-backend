// Middleware/RequestLoggingMiddleware.cs
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text;

namespace Cor.ProjectManagement.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Log request
            await LogRequest(context);

            // Read response body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                // Log response
                await LogResponse(context, stopwatch.ElapsedMilliseconds);

                // Copy response body back
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            try
            {
                var request = context.Request;
                var body = await ReadRequestBody(request);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var logEntry = new
                    {
                        timestamp = DateTime.UtcNow,
                        method = request.Method,
                        path = request.Path,
                        queryString = request.QueryString.ToString(),
                        headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                        body = body,
                        ip = context.Connection.RemoteIpAddress?.ToString(),
                        userAgent = request.Headers["User-Agent"].ToString()
                    };

                    _logger.LogInformation("Request: {Request}", JsonSerializer.Serialize(logEntry));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging request");
            }
        }

        private async Task LogResponse(HttpContext context, long elapsedMilliseconds)
        {
            try
            {
                var response = context.Response;
                response.Body.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    var logEntry = new
                    {
                        timestamp = DateTime.UtcNow,
                        statusCode = response.StatusCode,
                        elapsedMs = elapsedMilliseconds,
                        headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                        body = body
                    };

                    _logger.LogInformation("Response: {Response}", JsonSerializer.Serialize(logEntry));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging response");
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                return body;
            }
            catch
            {
                return "[Unable to read body]";
            }
        }
    }
}