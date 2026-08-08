using Leave.Utility.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Leave.API.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HrmLeaveDbContext>();

        try
        {
            // ✅ Check if there are pending migrations
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
                Console.WriteLine("✅ Database migration applied successfully.");
            }
            else
            {
                Console.WriteLine("✅ Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error applying migration: {ex.Message}");
            // Don't throw - let the app continue
        }
    }
}