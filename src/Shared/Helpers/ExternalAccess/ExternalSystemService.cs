using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.ExternalAccess;

public interface IExternalSystemService
{
    Task<ExternalSystem> RegisterSystemAsync(string systemName, string[]? allowedEndpoints = null);
    Task RevokeAccessAsync(string apiKey);
    Task<bool> IsSystemAllowedAsync(string apiKey, string endpoint);
    Task<IEnumerable<ExternalSystem>> GetAllSystemsAsync();
    Task<ExternalSystem?> GetSystemByApiKeyAsync(string apiKey);
}

/// <summary>
/// Single, DbContext-agnostic implementation shared by every module. It reads/writes
/// the module's own "ExternalSystems" table via the base DbContext (Set&lt;ExternalSystem&gt;()),
/// so there is one code path instead of a copy per module.
/// </summary>
public class ExternalSystemService : IExternalSystemService
{
    private readonly DbContext _db;
    private readonly ILogger<ExternalSystemService> _logger;
    private readonly ConcurrentDictionary<string, ExternalSystem> _cache = new();

    // Internal clients are allowed all endpoints regardless of their allow-list.
    private static readonly string[] InternalClients = { "CoreHRMM", "CoreModule", "AuthService", "Gateway" };

    public ExternalSystemService(DbContext db, ILogger<ExternalSystemService> logger)
    {
        _db = db;
        _logger = logger;
    }

    private DbSet<ExternalSystem> Systems => _db.Set<ExternalSystem>();

    public async Task<ExternalSystem> RegisterSystemAsync(string systemName, string[]? allowedEndpoints = null)
    {
        var system = new ExternalSystem
        {
            Id = Guid.NewGuid().ToString(),
            Name = systemName,
            ApiKey = GenerateApiKey(),
            BaseUrl = string.Empty,
            AllowedEndpoints = allowedEndpoints ?? Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(6),
            IsActive = true,
            RateLimitPerMinute = 60
        };
        Systems.Add(system);
        await _db.SaveChangesAsync();
        _cache.TryAdd(system.ApiKey, system);
        _logger.LogInformation("External system registered: {SystemName} ({ApiKey})", systemName, MaskApiKey(system.ApiKey));
        return system;
    }

    public async Task RevokeAccessAsync(string apiKey)
    {
        var system = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);
        if (system != null)
        {
            system.IsActive = false;
            system.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            _cache.TryRemove(apiKey, out _);
            _logger.LogWarning("API Key revoked for system: {SystemName}", system.Name);
        }
    }

    public async Task<bool> IsSystemAllowedAsync(string apiKey, string endpoint)
    {
        if (_cache.TryGetValue(apiKey, out var cached)) return IsAllowed(cached, endpoint);

        var system = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);
        if (system == null) return false;

        _cache.TryAdd(apiKey, system);
        return IsAllowed(system, endpoint);
    }

    private static bool IsAllowed(ExternalSystem system, string endpoint)
    {
        if (!system.IsActive || system.ExpiresAt < DateTime.UtcNow) return false;
        if (system.AllowedEndpoints == null || system.AllowedEndpoints.Length == 0) return true;
        if (InternalClients.Contains(system.Name)) return true;

        var normalized = endpoint.TrimEnd('/');
        return system.AllowedEndpoints.Any(pattern =>
        {
            var p = pattern.TrimEnd('/');
            if (p == normalized) return true;
            if (p.EndsWith("/*")) return normalized.StartsWith(p.TrimEnd('*'));
            return false;
        });
    }

    public async Task<IEnumerable<ExternalSystem>> GetAllSystemsAsync()
        => await Systems.Where(x => x.IsActive).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task<ExternalSystem?> GetSystemByApiKeyAsync(string apiKey)
    {
        if (_cache.TryGetValue(apiKey, out var cached)) return cached;
        var system = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);
        if (system != null) _cache.TryAdd(apiKey, system);
        return system;
    }

    private static string GenerateApiKey()
    {
        var key = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return $"ext_{key.Replace("+", "").Replace("/", "").Replace("=", "")}";
    }

    private static string MaskApiKey(string apiKey)
        => string.IsNullOrEmpty(apiKey) || apiKey.Length < 8 ? "***" : apiKey[..4] + "..." + apiKey[^4..];
}
