// E:\untitled46\RST_ERP\src\Svc.Core\Cor.HRMM\Persistence\Configurations\ApiKeyConfig.cs
using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cor.HRMM.Persistence;
namespace Cor.HRMM.Persistence.Configurations;

public class ApiKeyConfig : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ApiKeyValue)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.ApiKeyValue)
            .IsUnique()
            .HasDatabaseName("IX_ApiKeys_ApiKey");

        builder.Property(x => x.ClientType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("External");

        builder.Property(x => x.BaseUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.AllowedEndpointsJson)
            .HasColumnName("AllowedEndpoints")
            .HasColumnType("jsonb")
            .HasDefaultValue("[]");

        builder.Property(x => x.RateLimitPerMinute)
            .HasDefaultValue(60);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.RequestCount)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_ApiKeys_IsActive");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_ApiKeys_CreatedAt");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_ApiKeys_ExpiresAt");

        // Query filter for active keys
        builder.HasQueryFilter(x => x.IsActive && (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow));
    }
}