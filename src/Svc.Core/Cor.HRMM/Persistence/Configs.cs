using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cor.HRMM.Persistence;

// Base Entity Configuration
public abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.DateAdd).IsRequired().HasColumnType("timestamp with time zone");
        b.Property(x => x.DateMod).HasColumnType("timestamp with time zone");
        b.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        b.Property(x => x.xmin).HasColumnName("xmin").HasColumnType("xid").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
        b.HasIndex(x => x.IsDeleted);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

// BenefitSetting Configuration
public class BenefitSettingConfig : BaseEntityConfig<BenefitSetting>
{
    public override void Configure(EntityTypeBuilder<BenefitSetting> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.BenefitValue).IsRequired();
        b.Property(x => x.Per).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.Name).IsUnique();
        b.HasIndex(x => x.Per);
    }
}

// EducationQual Configuration
public class EducationQualConfig : BaseEntityConfig<EducationQual>
{
    public override void Configure(EntityTypeBuilder<EducationQual> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.HasIndex(x => x.Name).IsUnique();
    }
}

// JobGrade Configuration
public class JobGradeConfig : BaseEntityConfig<JobGrade>
{
    public override void Configure(EntityTypeBuilder<JobGrade> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.StartSalary).IsRequired();
        b.Property(x => x.MaxSalary).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
    }
}

// JgStep Configuration
public class JgStepConfig : BaseEntityConfig<JgStep>
{
    public override void Configure(EntityTypeBuilder<JgStep> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Salary).IsRequired();
        b.Property(x => x.Currency).IsRequired().HasMaxLength(5);
        b.Property(x => x.SalaryPayFreq).IsRequired().HasMaxLength(5);
        b.Property(x => x.JobGradeId).IsRequired();

        b.HasOne(x => x.JobGrade)
            .WithMany(x => x.JgSteps)
            .HasForeignKey(x => x.JobGradeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.JobGradeId);
        b.HasIndex(x => new { x.JobGradeId, x.Name }).IsUnique();
    }
}

// Position Configuration
public class PositionConfig : BaseEntityConfig<Position>
{
    public override void Configure(EntityTypeBuilder<Position> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameAm).HasMaxLength(200);
        b.Property(x => x.NoOfPosition).IsRequired();
        b.Property(x => x.IsVacant).IsRequired().HasMaxLength(3);
        b.Property(x => x.DepartmentId).IsRequired();
        b.Property(x => x.JobGradeId).IsRequired(false);

        // DepartmentId is from Core module - no navigation
        b.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_Position_DepartmentId");

        // JobGrade relationship (within HRMM)
        b.HasOne(x => x.JobGrade)
            .WithMany(x => x.Positions)
            .HasForeignKey(x => x.JobGradeId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-one with PositionReq
        b.HasOne(x => x.PositionReq)
            .WithOne(x => x.Position)
            .HasForeignKey<PositionReq>(x => x.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-one with PositionExp
        b.HasOne(x => x.PositionExp)
            .WithOne(x => x.Position)
            .HasForeignKey<PositionExp>(x => x.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.Name);
        b.HasIndex(x => x.JobGradeId);
        b.HasIndex(x => new { x.DepartmentId, x.Name }).IsUnique();
    }
}

// PositionReq Configuration
public class PositionReqConfig : BaseEntityConfig<PositionReq>
{
    public override void Configure(EntityTypeBuilder<PositionReq> b)
    {
        base.Configure(b);
        b.Property(x => x.Gender).IsRequired().HasMaxLength(20);
        b.Property(x => x.ProfessionType).IsRequired().HasMaxLength(50);
        b.Property(x => x.SaturdayWorkOption).IsRequired().HasMaxLength(20);
        b.Property(x => x.SundayWorkOption).IsRequired().HasMaxLength(20);
        b.Property(x => x.WorkingHours).IsRequired();
        b.Property(x => x.PositionId).IsRequired();

        b.HasIndex(x => x.PositionId).IsUnique();
    }
}

// PositionExp Configuration
public class PositionExpConfig : BaseEntityConfig<PositionExp>
{
    public override void Configure(EntityTypeBuilder<PositionExp> b)
    {
        base.Configure(b);
        b.Property(x => x.SamePosExp).IsRequired();
        b.Property(x => x.OtherPosExp).IsRequired();
        b.Property(x => x.MinAge).IsRequired();
        b.Property(x => x.MaxAge).IsRequired();
        b.Property(x => x.PositionId).IsRequired();

        b.HasIndex(x => x.PositionId).IsUnique();
    }
}

// PositionEducation Configuration
public class PositionEducationConfig : BaseEntityConfig<PositionEducation>
{
    public override void Configure(EntityTypeBuilder<PositionEducation> b)
    {
        base.Configure(b);
        b.Property(x => x.EducationLevel).IsRequired().HasMaxLength(50);
        b.Property(x => x.PositionId).IsRequired();
        b.Property(x => x.EducationQualId).IsRequired();

        b.HasOne(x => x.Position)
            .WithMany(x => x.PositionEducations)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.EducationQual)
            .WithMany()
            .HasForeignKey(x => x.EducationQualId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.PositionId);
        b.HasIndex(x => x.EducationQualId);
        b.HasIndex(x => new { x.PositionId, x.EducationQualId }).IsUnique();
    }
}

// PositionBenefit Configuration
public class PositionBenefitConfig : BaseEntityConfig<PositionBenefit>
{
    public override void Configure(EntityTypeBuilder<PositionBenefit> b)
    {
        base.Configure(b);
        b.Property(x => x.BenefitSettingId).IsRequired();
        b.Property(x => x.PositionId).IsRequired();

        b.HasOne(x => x.BenefitSetting)
            .WithMany()
            .HasForeignKey(x => x.BenefitSettingId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Position)
            .WithMany(x => x.PositionBenefits)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.PositionId);
        b.HasIndex(x => x.BenefitSettingId);
        b.HasIndex(x => new { x.PositionId, x.BenefitSettingId }).IsUnique();
    }
}
