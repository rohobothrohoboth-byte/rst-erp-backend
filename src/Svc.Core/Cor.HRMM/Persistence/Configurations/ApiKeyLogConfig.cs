// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Persistence\Configurations\ApiKeyLogConfig.cs
using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cor.HRMM.Persistence.Configurations;

public class ApiKeyLogConfig : IEntityTypeConfiguration<ApiKeyLog>
{
    public void Configure(EntityTypeBuilder<ApiKeyLog> builder)
    {
        builder.ToTable("ApiKeyLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApiKeyId)
            .IsRequired();

        builder.Property(x => x.Endpoint)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Method)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Success)
            .IsRequired();

        builder.Property(x => x.StatusCode)
            .IsRequired(false);

        builder.Property(x => x.ClientIp)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.Timestamp)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(x => x.Timestamp)
            .HasDatabaseName("IX_ApiKeyLogs_Timestamp");

        builder.HasIndex(x => x.ApiKeyId)
            .HasDatabaseName("IX_ApiKeyLogs_ApiKeyId");

        builder.HasIndex(x => x.Success)
            .HasDatabaseName("IX_ApiKeyLogs_Success");

        // Relationships
        builder.HasOne(x => x.ApiKey)
            .WithMany(x => x.ApiKeyLogs)
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}