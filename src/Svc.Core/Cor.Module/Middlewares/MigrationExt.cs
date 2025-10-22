using Cor.Module.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<CoreModuleDbContext>();
        dbContext.Database.Migrate();
    }
}