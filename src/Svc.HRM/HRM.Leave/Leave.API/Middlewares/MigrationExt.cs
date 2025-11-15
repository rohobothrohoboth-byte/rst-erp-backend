using Leave.Utility.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Leave.API.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HrmLeaveDbContext>();
        dbContext.Database.Migrate();
    }
}