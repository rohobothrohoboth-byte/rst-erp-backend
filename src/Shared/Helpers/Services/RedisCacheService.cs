// src/Shared/Helpers/Services/RedisCacheService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Shared.Helpers.Services;

public class RedisCacheService : BaseCacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        : base(logger)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
    }

    public override async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return default;
        }
    }

    public override async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);

            if (expiry.HasValue)
            {
                await _database.StringSetAsync(key, json, expiry.Value);
            }
            else
            {
                await _database.StringSetAsync(key, json);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }

    public override async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

    public override async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
    {
        try
        {
            var endpoints = _redis.GetEndPoints();
            if (!endpoints.Any())
                return;

            var keys = new List<RedisKey>();

            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                var serverKeys = server.Keys(pattern: pattern).ToArray();
                keys.AddRange(serverKeys);
            }

            if (keys.Any())
            {
                await _database.KeyDeleteAsync(keys.ToArray());
                _logger.LogInformation("Removed {Count} keys with pattern: {Pattern}", keys.Count, pattern);
            }
            else
            {
                _logger.LogDebug("No keys found with pattern: {Pattern}", pattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys with pattern: {Pattern}", pattern);
        }
    }

    public override async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking key existence: {Key}", key);
            return false;
        }
    }

    // ✅ Get keys by pattern (useful for debugging)
    public async Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
    {
        try
        {
            var endpoints = _redis.GetEndPoints();
            if (!endpoints.Any())
                return Enumerable.Empty<string>();

            var keys = new List<string>();

            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                var serverKeys = server.Keys(pattern: pattern);

                foreach (var key in serverKeys)
                {
                    keys.Add(key.ToString());
                }
            }

            return keys;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting keys by pattern: {Pattern}", pattern);
            return Enumerable.Empty<string>();
        }
    }

    // ✅ Get cache statistics
    public async Task<Dictionary<string, object>> GetCacheStatsAsync()
    {
        try
        {
            var stats = new Dictionary<string, object>();
            var endpoints = _redis.GetEndPoints();

            if (!endpoints.Any())
                return stats;

            var server = _redis.GetServer(endpoints.First());

            // Get server info
            var info = await server.InfoAsync();

            if (info != null)
            {
                // Iterate through all groups
                foreach (var group in info)
                {
                    foreach (var item in group)
                    {
                        stats[$"{group.Key}_{item.Key}"] = item.Value;
                    }
                }
            }

            // Add custom stats
            stats["ServerType"] = server.ServerType.ToString();
            stats["Version"] = server.Version.ToString();

            // ✅ FIXED: Properly handle the info groups
            // Get connected clients
            var clientsGroup = info?.FirstOrDefault(g => g.Key == "Clients");
            if (clientsGroup != null)
            {
                var connectedClients = clientsGroup.FirstOrDefault(i => i.Key == "connected_clients");
                stats["ConnectedClients"] = connectedClients.Value ?? "N/A";
            }
            else
            {
                stats["ConnectedClients"] = "N/A";
            }

            // Get memory info
            var memoryGroup = info?.FirstOrDefault(g => g.Key == "Memory");
            if (memoryGroup != null)
            {
                var usedMemory = memoryGroup.FirstOrDefault(i => i.Key == "used_memory_human");
                stats["UsedMemory"] = usedMemory.Value ?? "N/A";

                var maxMemory = memoryGroup.FirstOrDefault(i => i.Key == "maxmemory_human");
                stats["MaxMemory"] = maxMemory.Value ?? "N/A";
            }

            // Get keyspace info
            var keyspaceGroup = info?.FirstOrDefault(g => g.Key == "Keyspace");
            if (keyspaceGroup != null)
            {
                var keyspace = keyspaceGroup.FirstOrDefault(i => i.Key == "db0");
                stats["Keyspace"] = keyspace.Value ?? "N/A";

                // Get total keys across all databases
                var totalKeys = 0L;
                foreach (var dbInfo in keyspaceGroup)
                {
                    if (dbInfo.Key.StartsWith("db"))
                    {
                        var parts = dbInfo.Value.Split(',');
                        foreach (var part in parts)
                        {
                            if (part.StartsWith("keys="))
                            {
                                if (long.TryParse(part.Substring(5), out var count))
                                {
                                    totalKeys += count;
                                }
                            }
                        }
                    }
                }
                stats["TotalKeys"] = totalKeys;
            }

            // Get uptime
            var serverGroup = info?.FirstOrDefault(g => g.Key == "Server");
            if (serverGroup != null)
            {
                var uptime = serverGroup.FirstOrDefault(i => i.Key == "uptime_in_seconds");
                if (uptime.Value != null && long.TryParse(uptime.Value, out var seconds))
                {
                    var uptimeSpan = TimeSpan.FromSeconds(seconds);
                    stats["Uptime"] = $"{uptimeSpan.Days}d {uptimeSpan.Hours}h {uptimeSpan.Minutes}m";
                }
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            return new Dictionary<string, object>();
        }
    }

    // ✅ Clear all cache (use with caution)
    public async Task ClearAllAsync()
    {
        try
        {
            var endpoints = _redis.GetEndPoints();
            if (!endpoints.Any())
                return;

            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                await server.FlushDatabaseAsync();
            }

            _logger.LogWarning("Cleared ALL cache data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all cache");
        }
    }

    // ✅ Get TTL for a key
    public async Task<TimeSpan?> GetKeyTtlAsync(string key)
    {
        try
        {
            var ttl = await _database.KeyTimeToLiveAsync(key);
            return ttl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for key: {Key}", key);
            return null;
        }
    }

    // ✅ Get database size
    public async Task<long> GetDatabaseSizeAsync(int databaseId = 0)
    {
        try
        {
            var endpoints = _redis.GetEndPoints();
            if (!endpoints.Any())
                return 0;

            var server = _redis.GetServer(endpoints.First());

            // Get keys count using INFO command
            var info = await server.InfoAsync("keyspace");
            if (info != null)
            {
                var dbInfo = info.FirstOrDefault(g => g.Key == "Keyspace");
                if (dbInfo != null)
                {
                    var dbKey = dbInfo.FirstOrDefault(i => i.Key == $"db{databaseId}");
                    if (dbKey.Value != null)
                    {
                        // Parse "keys=123,expires=456,avg_ttl=789"
                        var parts = dbKey.Value.Split(',');
                        foreach (var part in parts)
                        {
                            if (part.StartsWith("keys="))
                            {
                                return long.Parse(part.Substring(5));
                            }
                        }
                    }
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database size");
            return 0;
        }
    }

    // ✅ Ping Redis server
    public async Task<TimeSpan> PingAsync()
    {
        try
        {
            return await _database.PingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinging Redis");
            return TimeSpan.Zero;
        }
    }

    // ✅ Get Redis version
    public async Task<string> GetRedisVersionAsync()
    {
        try
        {
            var endpoints = _redis.GetEndPoints();
            if (!endpoints.Any())
                return "Unknown";

            var server = _redis.GetServer(endpoints.First());
            return server.Version?.ToString() ?? "Unknown";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Redis version");
            return "Unknown";
        }
    }

    // ✅ Check Redis connection health
    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            var ping = await _database.PingAsync();
            return ping.TotalMilliseconds < 1000; // Less than 1 second
        }
        catch
        {
            return false;
        }
    }
}