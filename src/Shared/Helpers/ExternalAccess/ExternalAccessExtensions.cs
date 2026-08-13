using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Helpers.ExternalAccess;

public static class ExternalAccessExtensions
{
    /// <summary>
    /// Registers the shared external-system + API-key services for a module, bound to
    /// the module's DbContext (which must expose an ExternalSystems set / map
    /// <see cref="ExternalSystem"/>). One line replaces the per-module copies.
    /// </summary>
    public static IServiceCollection AddExternalSystemAccess<TDbContext>(
        this IServiceCollection services, IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddMemoryCache();
        services.Configure<ApiKeySettings>(configuration.GetSection("ApiKey"));

        // Expose the module's DbContext as the base DbContext for the shared services.
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());
        services.AddScoped<IExternalSystemService, ExternalSystemService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        return services;
    }

    /// <summary>
    /// Adds the shared "ApiKey" authentication scheme (X-API-Key header) to the
    /// authentication builder. Call after AddAuthentication(...).AddJwtBearer(...).
    /// </summary>
    public static AuthenticationBuilder AddSharedApiKey(
        this AuthenticationBuilder builder, string scheme = ApiKeyAuthenticationOptions.DefaultScheme)
        => builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(scheme, _ => { });
}
