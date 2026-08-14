// Extensions/HealthCheckExtensions.cs
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Cor.ProjectManagement.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IEndpointConventionBuilder MapProjectHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        totalDuration = report.TotalDuration.TotalSeconds.ToString("0.00"),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            duration = e.Value.Duration.TotalSeconds.ToString("0.00"),
                            description = e.Value.Description,
                            data = e.Value.Data
                        })
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });
        }

        public static IEndpointConventionBuilder MapProjectHealthChecksDetailed(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapHealthChecks("/health/detailed", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        totalDuration = report.TotalDuration.TotalSeconds.ToString("0.00"),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            duration = e.Value.Duration.TotalSeconds.ToString("0.00"),
                            description = e.Value.Description,
                            data = e.Value.Data
                        })
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });
        }
    }
}