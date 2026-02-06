using Microsoft.EntityFrameworkCore;
using Recruit.Utility.Persistence;

namespace Recruit.API.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HrmRecruitDbContext>();
        dbContext.Database.Migrate();
    }
}