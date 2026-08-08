// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Services\ApiKeyService.cs
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Cor.HRMM.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ApiKeyService> _logger;
    private readonly ApiKeySettings _options;
    private readonly coreHRMMDbContext _context;

    public ApiKeyService(
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<ApiKeyService> logger,
        IOptions<ApiKeySettings> options,
        coreHRMMDbContext context)
    {
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
        _options = options.Value;
        _context = context;
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        try
        {
            // Check cache first
            var cacheKey = $"apikey_valid_{apiKey}";
            if (_cache.TryGetValue(cacheKey, out bool isValid))
            {
                return isValid;
            }

            // ✅ Check 1: appsettings.json
            var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>()
                             ?? new Dictionary<string, string>();

            var isValidKey = configKeys.ContainsValue(apiKey);

            // ✅ Check 2: Database (External Systems)
            if (!isValidKey)
            {
                var externalSystem = await _context.ExternalSystems
                    .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);

                isValidKey = externalSystem != null;

                if (isValidKey)
                {
                    // Update last used and request count
                    externalSystem!.LastUsedAt = DateTime.UtcNow;
                    externalSystem.RequestCount++;
                    await _context.SaveChangesAsync(ct);

                    _logger.LogInformation("✅ API Key validated from database for: {ClientName}", externalSystem.Name);
                }
            }

            // Cache the result
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
        // ✅ Check 1: appsettings.json
        var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>()
                         ?? new Dictionary<string, string>();

        var clientName = configKeys.FirstOrDefault(x => x.Value == apiKey).Key;

        // ✅ Check 2: Database (External Systems)
        if (string.IsNullOrEmpty(clientName))
        {
            var externalSystem = await _context.ExternalSystems
                .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);

            if (externalSystem != null)
            {
                clientName = externalSystem.Name;
            }
        }

        return clientName;
    }

    public async Task LogApiKeyUsageAsync(
        string apiKey,
        string endpoint,
        string method,
        bool success,
        CancellationToken ct = default)
    {
        try
        {
            var clientName = await GetClientNameAsync(apiKey, ct) ?? "Unknown";

            // Log to structured logging
            _logger.LogInformation(
                "API Key Usage | Client: {Client} | Endpoint: {Endpoint} | Method: {Method} | Success: {Success}",
                clientName, endpoint, method, success);

            // ✅ Update database usage if external system
            var externalSystem = await _context.ExternalSystems
                .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);

            if (externalSystem != null)
            {
                externalSystem.LastUsedAt = DateTime.UtcNow;
                externalSystem.RequestCount++;
                await _context.SaveChangesAsync(ct);
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

        // ✅ Get from config
        var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>()
                         ?? new Dictionary<string, string>();

        foreach (var kvp in configKeys)
        {
            allKeys.Add(new ApiKeyInfo
            {
                ClientName = kvp.Key,
                ApiKey = MaskApiKey(kvp.Value),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastUsedAt = null,
                RequestCount = 0
            });
        }

        // ✅ Get from database
        var dbSystems = await _context.ExternalSystems
            .Where(x => x.IsActive)
            .ToListAsync(ct);

        foreach (var system in dbSystems)
        {
            allKeys.Add(new ApiKeyInfo
            {
                ClientName = system.Name,
                ApiKey = MaskApiKey(system.ApiKey),
                CreatedAt = system.CreatedAt,
                ExpiresAt = system.ExpiresAt,
                IsActive = system.IsActive,
                LastUsedAt = system.LastUsedAt,
                RequestCount = (int)system.RequestCount
            });
        }

        return allKeys;
    }

    public async Task RevokeApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        // ✅ Check database for external system
        var externalSystem = await _context.ExternalSystems
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive, ct);

        if (externalSystem != null)
        {
            externalSystem.IsActive = false;
            externalSystem.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            // Clear cache
            _cache.Remove($"apikey_valid_{apiKey}");

            _logger.LogWarning("🔒 API Key revoked for system: {SystemName}", externalSystem.Name);
        }
        else
        {
            _logger.LogWarning("API Key revocation requested but not found: {ApiKey}", MaskApiKey(apiKey));
        }
    }

    private string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 8)
            return "***";

        return apiKey[..4] + "..." + apiKey[^4..];
    }
}

