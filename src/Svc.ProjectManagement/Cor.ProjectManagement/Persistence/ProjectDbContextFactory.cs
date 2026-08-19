// Persistence/ProjectDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cor.ProjectManagement.Persistence
{
    public class ProjectDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
    {
        public ProjectDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectDbContext>();

            var connectionString = "Host=localhost;Port=5432;Database=core.ProjectManagementDb;Username=postgres;Password=root;Include Error Detail=true";

            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ProjectDbContext).Assembly.FullName);
            });

            return new ProjectDbContext(optionsBuilder.Options);
        }
    }
}