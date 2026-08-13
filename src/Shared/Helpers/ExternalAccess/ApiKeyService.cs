using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Helpers.ExternalAccess;

public interface IApiKeyService
{
    Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default);
    Task<string?> GetClientNameAsync(string apiKey, CancellationToken ct = default);
    Task LogApiKeyUsageAsync(string apiKey, string endpoint, string method, bool success, CancellationToken ct = default);
    Task<IEnumerable<ApiKeyInfo>> GetAllApiKeysAsync(CancellationToken ct = default);
    Task RevokeApiKeyAsync(string apiKey, CancellationToken ct = default);
}

/// <summary>
/// Single shared implementation. Validates API keys against appsettings "ApiKeys"
/// and the module's "ExternalSystems" table (via the base DbContext).
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ApiKeyService> _logger;
    private readonly ApiKeySettings _options;
    private readonly DbContext _db;

    public ApiKeyService(
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<ApiKeyService> logger,
        IOptions<ApiKeySettings> options,
        DbContext db)
    {
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
        _options = options.Value;
        _db = db;
    }

    private DbSet<ExternalSystem> Systems => _db.Set<ExternalSystem>();

    public async Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"apikey_valid_{apiKey}";
            if (_cache.TryGetValue(cacheKey, out bool isValid)) return isValid;

            var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>() ?? new();
            var isValidKey = configKeys.ContainsValue(apiKey);

            if (!isValidKey)
            {
                var externalSystem = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);
                isValidKey = externalSystem != null;
                if (isValidKey)
                {
                    externalSystem!.LastUsedAt = DateTime.UtcNow;
                    externalSystem.RequestCount++;
                    await _db.SaveChangesAsync(ct);
                }
            }

            _cache.Set(cacheKey, isValidKey, TimeSpan.FromMinutes(_options.CacheDurationMinutes));
            return isValidKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating API key");
            return false;
        }
    }

    public async Task<string?> GetClientNameAsync(string apiKey, CancellationToken ct = default)
    {
        var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>() ?? new();
        var clientName = configKeys.FirstOrDefault(x => x.Value == apiKey).Key;
        if (string.IsNullOrEmpty(clientName))
        {
            var externalSystem = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);
            if (externalSystem != null) clientName = externalSystem.Name;
        }
        return clientName;
    }

    public async Task LogApiKeyUsageAsync(string apiKey, string endpoint, string method, bool success, CancellationToken ct = default)
    {
        try
        {
            var clientName = await GetClientNameAsync(apiKey, ct) ?? "Unknown";
            _logger.LogInformation("API Key Usage | Client: {Client} | {Method} {Endpoint} | Success: {Success}",
                clientName, method, endpoint, success);

            var externalSystem = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);
            if (externalSystem != null)
            {
                externalSystem.LastUsedAt = DateTime.UtcNow;
                externalSystem.RequestCount++;
                await _db.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging API key usage");
        }
    }

    public async Task<IEnumerable<ApiKeyInfo>> GetAllApiKeysAsync(CancellationToken ct = default)
    {
        var allKeys = new List<ApiKeyInfo>();
        var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>() ?? new();
        foreach (var kvp in configKeys)
            allKeys.Add(new ApiKeyInfo { ClientName = kvp.Key, ApiKey = MaskApiKey(kvp.Value), CreatedAt = DateTime.UtcNow, IsActive = true });

        var dbSystems = await Systems.Where(x => x.IsActive).ToListAsync(ct);
        foreach (var s in dbSystems)
            allKeys.Add(new ApiKeyInfo
            {
                ClientName = s.Name,
                ApiKey = MaskApiKey(s.ApiKey),
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpiresAt,
                IsActive = s.IsActive,
                LastUsedAt = s.LastUsedAt,
                RequestCount = (int)s.RequestCount
            });

        return allKeys;
    }

    public async Task RevokeApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        var externalSystem = await Systems.FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);
        if (externalSystem != null)
        {
            externalSystem.IsActive = false;
            externalSystem.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            _cache.Remove($"apikey_valid_{apiKey}");
            _logger.LogWarning("API Key revoked for system: {SystemName}", externalSystem.Name);
        }
    }

    private static string MaskApiKey(string apiKey)
        => string.IsNullOrEmpty(apiKey) || apiKey.Length < 8 ? "***" : apiKey[..4] + "..." + apiKey[^4..];
}
