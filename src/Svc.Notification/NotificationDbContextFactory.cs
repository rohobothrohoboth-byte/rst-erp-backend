using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Svc.Notification.Data;

namespace Svc.Notification;

public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();

        // Connection string for design time (update with your actual connection string)
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Notification.Mgr;Username=postgres;Password=root;Include Error Detail=true");

        return new NotificationDbContext(optionsBuilder.Options);
    }
}