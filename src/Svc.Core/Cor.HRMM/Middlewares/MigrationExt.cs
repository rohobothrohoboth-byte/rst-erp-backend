using Cor.HRMM.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<coreHRMMDbContext>();
        dbContext.Database.Migrate();
    }

    public static async Task ApplySeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<coreHRMMDbContext>();
    }
}