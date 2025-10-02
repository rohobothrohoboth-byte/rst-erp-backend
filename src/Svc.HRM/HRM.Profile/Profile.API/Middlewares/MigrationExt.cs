using Microsoft.EntityFrameworkCore;
using Profile.Utility.Persistence;

namespace Profile.API.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HrmProfileDbContext>();
        dbContext.Database.Migrate();
    }
}