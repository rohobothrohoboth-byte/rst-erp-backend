// Controllers/HealthController.cs
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.Threading.Channels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using Cor.Finance.Middlewares;
namespace Cor.Finance.Controllers;
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IMemoryCache cache, ILogger<HealthController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var metrics = _cache.Get<PerformanceMetrics>("perf:latest") ?? new PerformanceMetrics();

        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Performance = new
            {
                metrics.TotalRequests,
                metrics.AvgResponseTime,
                metrics.TotalDbQueries
            },
            Caches = new
            {
                Dashboard = _cache.Get("full_dashboard:*") != null,
                Analytics = _cache.Get("analytics:*") != null,
                Reference = _cache.Get("reference:*") != null
            }
        });
    }
}