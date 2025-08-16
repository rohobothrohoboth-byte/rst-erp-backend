using Microsoft.EntityFrameworkCore;
using RST.Auth.API.Data;
using RST.Auth.API.Services;

namespace RST.Auth.API.Middlewares
{
    public static class MigrationExt
    {
        public static async Task ApplyMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await dbContext.Database.MigrateAsync();
            var services = scope.ServiceProvider;
            await SeedData.EnsureAsync(services);
        }
    }
}
