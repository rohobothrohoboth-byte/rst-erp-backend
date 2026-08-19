// Persistence/ProjectWarehouseContext.cs
using Microsoft.EntityFrameworkCore;

namespace Cor.ProjectManagement.Persistence
{
    public class ProjectWarehouseContext : DbContext
    {
        public ProjectWarehouseContext(DbContextOptions<ProjectWarehouseContext> options)
            : base(options)
        {
        }

        // Add warehouse-specific DbSets here
        // public DbSet<ProjectWarehouse> ProjectWarehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure warehouse tables here
        }
    }
}