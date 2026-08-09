using Microsoft.EntityFrameworkCore;
using Svc.HRM.Training.Persistence;

namespace Svc.HRM.Training.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TrainingDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Migrations");
        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Training database migrations applied.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Training database migration failed.");
        }
    }
}
