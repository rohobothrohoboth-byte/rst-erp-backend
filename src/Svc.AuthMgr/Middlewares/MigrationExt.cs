using Microsoft.EntityFrameworkCore;
using Svc.AuthMgr.Persistence;

namespace Svc.AuthMgr.Middlewares;

public static class MigrationExt
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using AuthDbContext dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();

            app.Logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while applying database migrations.");
            throw;
        }
    }
}