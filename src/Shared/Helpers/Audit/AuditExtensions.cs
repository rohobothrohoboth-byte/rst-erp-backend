using System.Threading.Channels;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Helpers.Audit;

public static class AuditExtensions
{
    /// <summary>
    /// Registers the shared audit pipeline for a module. The module's DbContext must
    /// map <see cref="AuditLog"/> (DbSet&lt;AuditLog&gt; -> "AuditLogs"). One line
    /// gives any module an audit trail; pair with app.UseSharedAudit().
    /// </summary>
    public static IServiceCollection AddSharedAudit<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.TryAddSingleton(Channel.CreateUnbounded<AuditLog>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        }));
        // Shared writer resolves the base DbContext; TryAdd avoids clashing with
        // AddExternalSystemAccess, which registers the same mapping.
        services.TryAddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());
        services.AddHostedService<AuditWriterService>();
        return services;
    }

    /// <summary>Adds the audit middleware. Place after authentication/authorization.</summary>
    public static IApplicationBuilder UseSharedAudit(this IApplicationBuilder app)
        => app.UseMiddleware<AuditLogMiddleware>();
}
