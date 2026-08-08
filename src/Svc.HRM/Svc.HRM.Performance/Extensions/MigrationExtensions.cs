using Microsoft.EntityFrameworkCore;
using Svc.HRM.Performance.Persistence;

namespace Svc.HRM.Performance.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PerformanceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Migrations");
        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Performance database migrations applied.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Performance database migration failed.");
        }
    }
}
