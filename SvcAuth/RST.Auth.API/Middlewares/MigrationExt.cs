using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;

namespace RST.Auth.API.Middlewares
{
    public static class MigrationExt
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
