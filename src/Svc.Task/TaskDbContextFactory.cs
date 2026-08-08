using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Svc.Task.Data;

namespace Svc.Task;

public class TaskDbContextFactory : IDesignTimeDbContextFactory<TaskDbContext>
{
    public TaskDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TaskDbContext>();

        // Connection string for design time (update with your actual connection string)
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Task.Mgr;Username=postgres;Password=root;Include Error Detail=true");

        return new TaskDbContext(optionsBuilder.Options);
    }
}