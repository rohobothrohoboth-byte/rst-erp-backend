using Cor.Utility.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.API.Middlewares
{
    public static class MigrationExt
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
