using Microsoft.EntityFrameworkCore;
using Svc.Lup.Persistence;

namespace Svc.Lup.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<LupDbContext>();
        dbContext.Database.Migrate();
    }

    public static async Task ApplySeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<LupDbContext>();
        await LupSeeder.SeedAsync(dbContext);
    }
}