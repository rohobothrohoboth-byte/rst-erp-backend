// src/Shared/Helpers/ConfigurationHelper.cs

using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Shared.Helpers;

public static class ConfigurationHelper
{
    private static IConfigurationRoot? _configuration;
    private static readonly object _lock = new();

    public static IConfigurationRoot GetConfiguration(string? environment = null)
    {
        if (_configuration != null) return _configuration;

        lock (_lock)
        {
            if (_configuration != null) return _configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory);

            // ✅ Add shared configuration
            var sharedConfigPath = Path.Combine(AppContext.BaseDirectory, "appsettings.shared.json");
            if (File.Exists(sharedConfigPath))
            {
                builder.AddJsonFile(sharedConfigPath, optional: true, reloadOnChange: true);
                Console.WriteLine($"✅ Loaded shared config from: {sharedConfigPath}");
            }
            else
            {
                // Try to find from solution root
                var solutionRoot = FindSolutionRoot();
                if (solutionRoot != null)
                {
                    var sharedPath = Path.Combine(solutionRoot, "src", "Shared", "Helpers", "appsettings.json");
                    if (File.Exists(sharedPath))
                    {
                        builder.AddJsonFile(sharedPath, optional: true, reloadOnChange: true);
                        Console.WriteLine($"✅ Loaded shared config from: {sharedPath}");
                    }
                }
            }

            // ✅ Add environment-specific overrides
            if (!string.IsNullOrEmpty(environment))
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
            }

            // ✅ Add environment variables (highest priority)
            builder.AddEnvironmentVariables();

            _configuration = builder.Build();
            return _configuration;
        }
    }

    /// <summary>
    /// Resolves a service URL by replacing {ServiceHost} placeholder with the actual host
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="key">The service URL key (e.g., "FinanceApi")</param>
    /// <returns>The resolved URL with the host replaced</returns>
    public static string ResolveServiceUrl(this IConfiguration configuration, string key)
    {
        var url = configuration[$"ServiceUrls:{key}"];
        var host = configuration["ServiceHost"];

        if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(url))
        {
            return url.Replace("{ServiceHost}", host);
        }

        return url ?? string.Empty;
    }

    /// <summary>
    /// Resolves a service URL by replacing {ServiceHost} placeholder with the actual host
    /// </summary>
    /// <param name="serviceName">The service name (e.g., "FinanceApi")</param>
    /// <param name="defaultValue">Default value if not found</param>
    /// <returns>The resolved URL with the host replaced</returns>
    public static string GetServiceUrl(string serviceName, string? defaultValue = null)
    {
        var config = GetConfiguration();
        return config.ResolveServiceUrl(serviceName);
    }

    public static string GetRabbitMQHost()
    {
        var config = GetConfiguration();
        return config["RabbitMQ:Host"] ?? "localhost";
    }

    public static int GetRabbitMQPort()
    {
        var config = GetConfiguration();
        return int.TryParse(config["RabbitMQ:Port"], out var port) ? port : 5672;
    }

    public static string GetRedisConnectionString()
    {
        var config = GetConfiguration();

        // Try ConnectionStrings:Redis first
        var cs = config.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(cs)) return cs;

        // Try Redis:ConnectionString
        cs = config["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(cs)) return cs;

        return "localhost:6379";
    }

    public static string GetConnectionString(string name)
    {
        var config = GetConfiguration();
        return config.GetConnectionString(name) ?? string.Empty;
    }

    private static string? FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var slnFiles = directory.GetFiles("*.sln");
            if (slnFiles.Length > 0)
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        return null;
    }
}