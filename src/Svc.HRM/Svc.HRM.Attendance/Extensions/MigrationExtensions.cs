using Microsoft.EntityFrameworkCore;
using Svc.HRM.Attendance.Persistence;

namespace Svc.HRM.Attendance.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AttendanceDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}