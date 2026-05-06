using Cor.HRMM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cor.HRMM.Persistence;

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

public class BenefitSettingConfig : BaseEntityConfig<BenefitSetting>
{
    public override void Configure(EntityTypeBuilder<BenefitSetting> b)
    {
        base.Configure(b);
        b.Property(bs => bs.Name).IsRequired().HasMaxLength(200);
        b.Property(bs => bs.BenefitValue).IsRequired();
        b.Property(bs => bs.Per).IsRequired().HasMaxLength(50);
        b.HasIndex(bs => bs.Name).IsUnique();
        b.HasIndex(x => x.Per);
    }
}

public class EducationQualConfig : BaseEntityConfig<EducationQual>
{
    public override void Configure(EntityTypeBuilder<EducationQual> b)
    {
        base.Configure(b);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
    }
}

public class JgStepConfig : BaseEntityConfig<JgStep>
{
    public override void Configure(EntityTypeBuilder<JgStep> b)
    {
        base.Configure(b);
        b.Property(js => js.Name).IsRequired().HasMaxLength(200);
        b.Property(js => js.Salary).IsRequired();
        b.Property(js => js.Currency).IsRequired().HasMaxLength(5);
        b.Property(js => js.SalaryPayFreq).IsRequired().HasMaxLength(5);
        b.Property(js => js.JobGradeId).IsRequired();
        b.HasIndex(js => js.JobGradeId);
        b.HasIndex(js => new { js.JobGradeId, js.Name }).IsUnique();
    }
}

public class JobGradeConfig : BaseEntityConfig<JobGrade>
{
    public override void Configure(EntityTypeBuilder<JobGrade> b)
    {
        base.Configure(b);
        b.Property(jg => jg.Name).IsRequired().HasMaxLength(200);
        b.Property(jg => jg.StartSalary).IsRequired();
        b.Property(jg => jg.MaxSalary).IsRequired();
        b.HasIndex(jg => jg.Name).IsUnique();
    }
}

public class PositionConfig : BaseEntityConfig<Position>
{
    public override void Configure(EntityTypeBuilder<Position> b)
    {
        base.Configure(b);
        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.Property(p => p.NameAm).HasMaxLength(200);
        b.Property(p => p.NoOfPosition).IsRequired();
        b.Property(p => p.IsVacant).IsRequired().HasMaxLength(3);
        b.Property(p => p.DepartmentId).IsRequired();
        b.HasIndex(p => p.DepartmentId);
        b.HasIndex(p => p.Name);
        b.HasIndex(p => new { p.DepartmentId, p.Name }).IsUnique();
    }
}

public class PositionBenefitConfig : BaseEntityConfig<PositionBenefit>
{
    public override void Configure(EntityTypeBuilder<PositionBenefit> b)
    {
        base.Configure(b);
        b.Property(pb => pb.BenefitSettingId).IsRequired();
        b.Property(pb => pb.PositionId).IsRequired();
        b.HasOne(pb => pb.BenefitSetting).WithMany().HasForeignKey(pb => pb.BenefitSettingId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(pb => pb.Position).WithMany().HasForeignKey(pb => pb.PositionId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(pb => pb.PositionId);
        b.HasIndex(pb => pb.BenefitSettingId);
        b.HasIndex(pb => new { pb.PositionId, pb.BenefitSettingId }).IsUnique();
    }
}

public class PositionEducationConfig : BaseEntityConfig<PositionEducation>
{
    public override void Configure(EntityTypeBuilder<PositionEducation> b)
    {
        base.Configure(b);
        b.Property(pe => pe.EducationLevel).IsRequired().HasMaxLength(50);
        b.Property(pe => pe.PositionId).IsRequired();
        b.Property(pe => pe.EducationQualId).IsRequired();
        b.HasOne(pe => pe.Position).WithMany().HasForeignKey(pe => pe.PositionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(pe => pe.EducationQual).WithMany().HasForeignKey(pe => pe.EducationQualId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(pe => pe.PositionId);
        b.HasIndex(pe => pe.EducationQualId);
        b.HasIndex(pe => new { pe.PositionId, pe.EducationQualId }).IsUnique();
    }
}

public class PositionExpConfig : BaseEntityConfig<PositionExp>
{
    public override void Configure(EntityTypeBuilder<PositionExp> b)
    {
        base.Configure(b);
        b.Property(pe => pe.SamePosExp).IsRequired();
        b.Property(pe => pe.OtherPosExp).IsRequired();
        b.Property(pe => pe.MinAge).IsRequired();
        b.Property(pe => pe.MaxAge).IsRequired();
        b.Property(pe => pe.PositionId).IsRequired();
        b.HasOne(pe => pe.Position).WithMany().HasForeignKey(pe => pe.PositionId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(pe => pe.PositionId);
    }
}

public class PositionReqConfig : BaseEntityConfig<PositionReq>
{
    public override void Configure(EntityTypeBuilder<PositionReq> b)
    {
        base.Configure(b);
        b.Property(pr => pr.Gender).IsRequired().HasMaxLength(20);
        b.Property(pr => pr.ProfessionType).IsRequired().HasMaxLength(50);
        b.Property(pr => pr.SaturdayWorkOption).IsRequired().HasMaxLength(20);
        b.Property(pr => pr.SundayWorkOption).IsRequired().HasMaxLength(20);
        b.Property(pr => pr.WorkingHours).IsRequired();
        b.Property(pr => pr.PositionId).IsRequired();
        b.HasOne(pr => pr.Position).WithMany().HasForeignKey(pr => pr.PositionId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(pr => pr.PositionId);
    }
}