using Microsoft.EntityFrameworkCore;
using Svc.HRM.Payroll.Persistence;

namespace Svc.HRM.Payroll.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PayrollDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}