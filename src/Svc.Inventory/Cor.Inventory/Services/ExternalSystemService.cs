// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Services\ExternalSystemService.cs
using System.Collections.Concurrent;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cor.Inventory.Services;



public class ExternalSystemService : IExternalSystemService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<ExternalSystemService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, ExternalSystem> _cache = new();

    public ExternalSystemService(
        InventoryDbContext context,
        ILogger<ExternalSystemService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ExternalSystem> RegisterSystemAsync(string systemName, string[]? allowedEndpoints = null)
    {
        var system = new ExternalSystem
        {
            Id = Guid.NewGuid().ToString(),
            Name = systemName,
            ApiKey = GenerateApiKey(),
            BaseUrl = string.Empty,
            AllowedEndpoints = allowedEndpoints ?? new[] { "/api/core/hrmm/v1/*" },
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(6), // 6 months validity
            IsActive = true,
            RateLimitPerMinute = 60
        };

        // Save to database
        _context.ExternalSystems.Add(system);
        await _context.SaveChangesAsync();

        // Add to cache
        _cache.TryAdd(system.ApiKey, system);

        _logger.LogInformation(
            "✅ External system registered: {SystemName} with API Key: {ApiKey}",
            systemName, MaskApiKey(system.ApiKey));

        return system;
    }

    public async Task RevokeAccessAsync(string apiKey)
    {
        var system = await _context.ExternalSystems
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);

        if (system != null)
        {
            system.IsActive = false;
            system.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _cache.TryRemove(apiKey, out _);
            _logger.LogWarning("🔒 API Key revoked for system: {SystemName}", system.Name);
        }
    }


   public async Task<bool> IsSystemAllowedAsync(string apiKey, string endpoint)
   {
       // Check cache first
       if (_cache.TryGetValue(apiKey, out var cachedSystem))
       {
           return IsAllowed(cachedSystem, endpoint);
       }

       // Check database
       var system = await _context.ExternalSystems
           .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);

       if (system == null)
           return false;

       // Cache for future requests
       _cache.TryAdd(apiKey, system);

       return IsAllowed(system, endpoint);
   }

  private bool IsAllowed(ExternalSystem system, string endpoint)
  {
      if (!system.IsActive || system.ExpiresAt < DateTime.UtcNow)
          return false;

      // ✅ If no endpoints specified, allow all (for internal clients)
      if (system.AllowedEndpoints == null || system.AllowedEndpoints.Length == 0)
          return true;

      // ✅ For internal clients, allow all endpoints
      // ✅ ADD "Finance" to the list of internal clients
      var internalClients = new[] { "CoreHRMM", "CoreModule", "Finance", "Procurement", "Gateway", "Inventory"};
      if (internalClients.Contains(system.Name))
          return true;

      // Check if endpoint is allowed for external clients
      var normalizedEndpoint = endpoint.TrimEnd('/');
      return system.AllowedEndpoints.Any(pattern =>
      {
          var normalizedPattern = pattern.TrimEnd('/');
          if (normalizedPattern == normalizedEndpoint)
              return true;
          if (normalizedPattern.EndsWith("/*"))
          {
              var prefix = normalizedPattern.TrimEnd('*');
              return normalizedEndpoint.StartsWith(prefix);
          }
          return false;
      });
  }
    public async Task<IEnumerable<ExternalSystem>> GetAllSystemsAsync()
    {
        return await _context.ExternalSystems
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ExternalSystem?> GetSystemByApiKeyAsync(string apiKey)
    {
        if (_cache.TryGetValue(apiKey, out var cached))
            return cached;

        var system = await _context.ExternalSystems
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);

        if (system != null)
            _cache.TryAdd(apiKey, system);

        return system;
    }



    private string GenerateApiKey()
    {
        // Generate a secure random API key
        var key = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return $"ext_{key.Replace("+", "").Replace("/", "").Replace("=", "")}";
    }

    private string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 8)
            return "***";
        return apiKey[..4] + "..." + apiKey[^4..];
    }
}