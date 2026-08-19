// E:\untitled46\RST_ERP\src\Svc.Core\Cor.Module\Services\ExternalSystemService.cs
using System.Collections.Concurrent;
using Cor.Module.Models.Entities;
using Cor.Module.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cor.Module.Services;

public class ExternalSystemService : IExternalSystemService
{
    private readonly CoreModuleDbContext _context;
    private readonly ILogger<ExternalSystemService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, (ExternalSystem System, DateTime CacheTime)> _cache = new();
    private readonly ConcurrentDictionary<string, string[]> _permissionCache = new();

    // ✅ የውስጥ ክላይንቶች ዝርዝር
    private readonly string[] _internalClients = new[] { "CoreHRMM", "CoreModule", "AuthService", "Gateway" };

    // ✅ ሁሉም የሚገኙ ፈቃዶች
    private readonly string[] _allPermissions = new[]
    {
        "core.company.view",
        "core.branch.view",
        "core.department.view",

    };

    public ExternalSystemService(
        CoreModuleDbContext context,
        ILogger<ExternalSystemService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ExternalSystem> RegisterSystemAsync(string systemName, string[]? allowedEndpoints = null, string[]? permissions = null)
    {
        var system = new ExternalSystem
        {
            Id = Guid.NewGuid().ToString(),
            Name = systemName,
            ApiKey = GenerateApiKey(),
            BaseUrl = string.Empty,
            AllowedEndpoints = allowedEndpoints ?? new[] { "/api/core/module/v1/*" },
            Permissions = permissions ?? Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(6),
            IsActive = true,
            RateLimitPerMinute = 60
        };

        _context.ExternalSystems.Add(system);
        await _context.SaveChangesAsync();

        _cache.TryAdd(system.ApiKey, (system, DateTime.UtcNow.AddHours(1)));

        _logger.LogInformation("✅ External system registered: {SystemName} with API Key: {ApiKey}",
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
            _permissionCache.TryRemove(apiKey, out _);
            _logger.LogWarning("🔒 API Key revoked for system: {SystemName}", system.Name);
        }
    }

    public async Task<bool> IsSystemAllowedAsync(string apiKey, string endpoint)
    {
        var system = await GetSystemByApiKeyAsync(apiKey);
        if (system == null || !system.IsActive)
            return false;

        return IsAllowed(system, endpoint);
    }

    private bool IsAllowed(ExternalSystem system, string endpoint)
    {
        if (!system.IsActive || system.ExpiresAt < DateTime.UtcNow)
            return false;

        // ✅ Internal clients have access to all endpoints
        if (_internalClients.Contains(system.Name))
            return true;

        // ✅ If no endpoints specified, allow all
        if (system.AllowedEndpoints == null || system.AllowedEndpoints.Length == 0)
            return true;

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

    // ✅ አዲስ: ፈቃድ መኖሩን ያረጋግጡ
    public async Task<bool> HasPermissionAsync(string apiKey, string permission)
    {
        var system = await GetSystemByApiKeyAsync(apiKey);
        if (system == null || !system.IsActive)
            return false;

        // ✅ Internal clients have all permissions
        if (_internalClients.Contains(system.Name))
            return true;

        // ✅ Check permissions from database
        var permissions = await GetPermissionsAsync(apiKey);
        return permissions.Contains(permission) || permissions.Contains("*");
    }

    // ✅ አዲስ: ፈቃዶችን ያግኙ
    public async Task<string[]> GetPermissionsAsync(string apiKey)
    {
        // Check cache first
        if (_permissionCache.TryGetValue(apiKey, out var cachedPermissions))
            return cachedPermissions;

        var system = await GetSystemByApiKeyAsync(apiKey);
        if (system == null || !system.IsActive)
            return Array.Empty<string>();

        // ✅ Internal clients have all permissions
        if (_internalClients.Contains(system.Name))
        {
            var allPerms = _allPermissions;
            _permissionCache.TryAdd(apiKey, allPerms);
            return allPerms;
        }

        var permissions = system.Permissions ?? Array.Empty<string>();
        _permissionCache.TryAdd(apiKey, permissions);
        return permissions;
    }

    // ✅ አዲስ: ፈቃዶችን ያዘምኑ
    public async Task UpdatePermissionsAsync(string apiKey, string[] permissions)
    {
        var system = await _context.ExternalSystems
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey);

        if (system == null)
            throw new ArgumentException($"System with API Key {MaskApiKey(apiKey)} not found");

        system.Permissions = permissions ?? Array.Empty<string>();
        await _context.SaveChangesAsync();

        // Update cache
        _permissionCache.TryRemove(apiKey, out _);
        _cache.TryRemove(apiKey, out _);

        _logger.LogInformation("✅ Updated permissions for system: {SystemName} ({PermissionCount} permissions)",
            system.Name, permissions?.Length ?? 0);
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
        // Check cache first
        if (_cache.TryGetValue(apiKey, out var cached) && cached.CacheTime > DateTime.UtcNow)
            return cached.System;

        var system = await _context.ExternalSystems
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey && x.IsActive);

        if (system != null)
            _cache.TryAdd(apiKey, (system, DateTime.UtcNow.AddHours(1)));

        return system;
    }

    private string GenerateApiKey()
    {
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