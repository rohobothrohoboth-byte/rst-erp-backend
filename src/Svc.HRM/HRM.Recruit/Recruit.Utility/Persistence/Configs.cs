using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Domain.Entities;

namespace Recruit.Utility.Persistence;

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

public class ApplicantConfig : BaseEntityConfig<Applicant>
{
    public override void Configure(EntityTypeBuilder<Applicant> b)
    {
        base.Configure(b);
        b.Property(x => x.RegisteredBy).HasMaxLength(50).IsRequired();
        b.Property(x => x.RegisteredDate).IsRequired();
        b.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId);
        b.HasOne(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);
        b.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);
    }
}

public class ApplicantAddressConfig : BaseEntityConfig<ApplicantAddress>
{
    public override void Configure(EntityTypeBuilder<ApplicantAddress> b)
    {
        base.Configure(b);
        b.Property(x => x.AddressType).HasMaxLength(2).IsRequired();
        b.Property(x => x.Country).HasMaxLength(100).IsRequired();
        b.Property(x => x.HouseNo).HasMaxLength(50);
        b.Property(x => x.Kebele).HasMaxLength(50);
        b.Property(x => x.Region).HasMaxLength(100);
        b.Property(x => x.Subcity).HasMaxLength(100);
        b.Property(x => x.Woreda).HasMaxLength(50);
        b.Property(x => x.Zone).HasMaxLength(50);
        b.HasIndex(x => x.Country);
    }
}

public class ApplicantContactConfig : BaseEntityConfig<ApplicantContact>
{
    public override void Configure(EntityTypeBuilder<ApplicantContact> b)
    {
        base.Configure(b);
        b.Property(x => x.AlternatePhone).HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(100).IsRequired();
        b.Property(x => x.Fax).HasMaxLength(30);
        b.Property(x => x.Phone).HasMaxLength(30).IsRequired();
        b.Property(x => x.PoBox).HasMaxLength(50);
        b.HasIndex(x => x.Email).IsUnique();
    }
}

public class ApplicantPersonConfig : BaseEntityConfig<ApplicantPerson>
{
    public override void Configure(EntityTypeBuilder<ApplicantPerson> b)
    {
        base.Configure(b);
        b.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
        b.Property(x => x.FirstNameAm).HasMaxLength(50);
        b.Property(x => x.Gender).HasMaxLength(10).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(50).IsRequired();
        b.Property(x => x.LastNameAm).HasMaxLength(50);
        b.Property(x => x.MiddleName).HasMaxLength(50).IsRequired();
        b.Property(x => x.MiddleNameAm).HasMaxLength(50);
        b.Property(x => x.Nationality).HasMaxLength(50).IsRequired();
    }
}

public class ApplicationRankingConfig : BaseEntityConfig<ApplicationRanking>
{
    public override void Configure(EntityTypeBuilder<ApplicationRanking> b)
    {
        base.Configure(b);
        b.Property(x => x.Rank).IsRequired();
        b.Property(x => x.TotalScore).IsRequired();
        b.Property(x => x.JobAppId).IsRequired();
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId);
        b.HasIndex(x => new { x.TotalScore, x.Rank });
    }
}

public class CoverLetterConfig : BaseEntityConfig<CoverLetter>
{
    public override void Configure(EntityTypeBuilder<CoverLetter> b)
    {
        base.Configure(b);
        b.Property(x => x.Content).IsRequired();
        b.Property(x => x.JobAppId).IsRequired();
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId);
    }
}

public class EvaluationFlowConfig : BaseEntityConfig<EvaluationFlow>
{
    public override void Configure(EntityTypeBuilder<EvaluationFlow> b)
    {
        base.Configure(b);
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.IsGlobal).IsRequired();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasMany(x => x.Steps).WithOne(x => x.EvaluationFlow).HasForeignKey(x => x.EvaluationFlowId);
        b.HasIndex(x => x.Name);
    }
}

public class EvaluationScoreConfig : BaseEntityConfig<EvaluationScore>
{
    public override void Configure(EntityTypeBuilder<EvaluationScore> b)
    {
        base.Configure(b);
        b.Property(x => x.EvalTypeId).IsRequired();
        b.Property(x => x.EvaluationStepId).IsRequired();
        b.Property(x => x.EvaluatorId).IsRequired();
        b.Property(x => x.Feedback).IsRequired();
        b.Property(x => x.IsCurrent).IsRequired();
        b.Property(x => x.Score).IsRequired();
        b.HasOne(x => x.EvalType).WithMany().HasForeignKey(x => x.EvalTypeId);
        b.HasOne(x => x.EvaluationStep).WithMany().HasForeignKey(x => x.EvaluationStepId);
        b.HasIndex(x => new { x.EvalTypeId, x.EvaluationStepId });
    }
}

public class EvaluationStepConfig : BaseEntityConfig<EvaluationStep>
{
    public override void Configure(EntityTypeBuilder<EvaluationStep> b)
    {
        base.Configure(b);
        b.Property(x => x.EvalTypeId).IsRequired();
        b.Property(x => x.EvaluationFlowId).IsRequired();
        b.Property(x => x.IsFinal).IsRequired();
        b.Property(x => x.StepName).HasMaxLength(100).IsRequired();
        b.Property(x => x.StepOrder).IsRequired();
        b.HasOne(x => x.EvalType).WithMany().HasForeignKey(x => x.EvalTypeId);
        b.HasOne(x => x.EvaluationFlow).WithMany().HasForeignKey(x => x.EvaluationFlowId);
        b.HasIndex(x => new { x.EvaluationFlowId, x.StepOrder });
    }
}

public class EvaluationTypeConfig : BaseEntityConfig<EvaluationType>
{
    public override void Configure(EntityTypeBuilder<EvaluationType> b)
    {
        base.Configure(b);
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.MaxScore).IsRequired();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
    }
}

public class JobApplicationConfig : BaseEntityConfig<JobApplication>
{
    public override void Configure(EntityTypeBuilder<JobApplication> b)
    {
        base.Configure(b);
        b.Property(x => x.AppliedDate).IsRequired();
        b.Property(x => x.ApplicantId);
        b.Property(x => x.EmployeeId);
        b.Property(x => x.JobPostingId).IsRequired();
        b.Property(x => x.PostType).HasMaxLength(20).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.HasOne(x => x.JobPosting).WithMany(x => x.Applications).HasForeignKey(x => x.JobPostingId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.AppliedDate);
    }
}

public class JobDecConfig : BaseEntityConfig<JobDec>
{
    public override void Configure(EntityTypeBuilder<JobDec> b)
    {
        base.Configure(b);
        b.Property(x => x.ContractType).HasMaxLength(20).IsRequired();
        b.Property(x => x.Desc).IsRequired();
        b.Property(x => x.KeySkills).IsRequired();
        b.Property(x => x.PreGender).HasMaxLength(10);
        b.Property(x => x.Qualification).IsRequired();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.WorkLocation).IsRequired();
    }
}

public class JobOfferConfig : BaseEntityConfig<JobOffer>
{
    public override void Configure(EntityTypeBuilder<JobOffer> b)
    {
        base.Configure(b);
        b.Property(x => x.ExpirationDate).IsRequired();
        b.Property(x => x.JobApplicationId).IsRequired();
        b.Property(x => x.JobPostingId).IsRequired();
        b.Property(x => x.OfferDate).IsRequired();
        b.Property(x => x.OfferDocument).IsRequired();
        b.Property(x => x.OfferNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.HasOne(x => x.JobApplication).WithMany().HasForeignKey(x => x.JobApplicationId);
        b.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.OfferNumber).IsUnique();
    }
}

public class JobOfferApprovalConfig : BaseEntityConfig<JobOfferApproval>
{
    public override void Configure(EntityTypeBuilder<JobOfferApproval> b)
    {
        base.Configure(b);
        b.Property(x => x.ApprovedById);
        b.Property(x => x.ApprovedDate);
        b.Property(x => x.JobOfferId).IsRequired();
        b.Property(x => x.Role).HasMaxLength(50).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.StepOrder).IsRequired();
        b.HasOne(x => x.JobOffer).WithMany().HasForeignKey(x => x.JobOfferId);
    }
}

public class JobOfferReviewConfig : BaseEntityConfig<JobOfferReview>
{
    public override void Configure(EntityTypeBuilder<JobOfferReview> b)
    {
        base.Configure(b);
        b.Property(x => x.AcceptanceDate);
        b.Property(x => x.ApprovalComments);
        b.Property(x => x.JobOfferId).IsRequired();
        b.Property(x => x.RejectionDate);
        b.Property(x => x.RejectionReason);
        b.HasOne(x => x.JobOffer).WithMany().HasForeignKey(x => x.JobOfferId);
    }
}

public class JobPostEvalFlowConfig : BaseEntityConfig<JobPostEvalFlow>
{
    public override void Configure(EntityTypeBuilder<JobPostEvalFlow> b)
    {
        base.Configure(b);
        b.Property(x => x.EvaluationFlowId).IsRequired();
        b.Property(x => x.JobPostingId).IsRequired();
        b.HasOne(x => x.EvaluationFlow).WithMany().HasForeignKey(x => x.EvaluationFlowId);
        b.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId);
        b.HasIndex(x => new { x.JobPostingId, x.EvaluationFlowId }).IsUnique();
    }
}

public class JobPostingConfig : BaseEntityConfig<JobPosting>
{
    public override void Configure(EntityTypeBuilder<JobPosting> b)
    {
        base.Configure(b);
        var p = b.Property(x => x.PostNumber)
            .HasMaxLength(15)
            .IsRequired()
            .HasDefaultValueSql("generate_code('JOB'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)")
            .ValueGeneratedOnAdd();
        p.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        b.Property(x => x.ClosedDate);
        b.Property(x => x.DeadlineDate).IsRequired();
        b.Property(x => x.JobReqId).IsRequired();
        b.Property(x => x.PostType).HasMaxLength(20).IsRequired();
        b.Property(x => x.PublishedDate).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.HasOne(x => x.JobReq).WithMany().HasForeignKey(x => x.JobReqId);
        b.HasMany(x => x.Applications).WithOne(x => x.JobPosting).HasForeignKey(x => x.JobPostingId);
        b.HasIndex(x => x.PostNumber).IsUnique();
        b.HasIndex(x => x.Status);
    }
}

public class JobPostReviewConfig : BaseEntityConfig<JobPostReview>
{
    public override void Configure(EntityTypeBuilder<JobPostReview> b)
    {
        base.Configure(b);
        b.Property(x => x.Comment).IsRequired();
        b.Property(x => x.JobPostingId).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId);
    }
}

public class JobReqReviewConfig : BaseEntityConfig<JobReqReview>
{
    public override void Configure(EntityTypeBuilder<JobReqReview> b)
    {
        base.Configure(b);
        b.Property(x => x.AppQuantity).IsRequired();
        b.Property(x => x.Comment).IsRequired();
        b.Property(x => x.JobReqId).IsRequired();
        b.Property(x => x.ReqQuantity).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.HasOne(x => x.JobReq).WithMany(x => x.Reviews).HasForeignKey(x => x.JobReqId);
    }
}

public class JobRequisitionConfig : BaseEntityConfig<JobRequisition>
{
    public override void Configure(EntityTypeBuilder<JobRequisition> b)
    {
        base.Configure(b);
        b.Property(x => x.BudgetCode).HasMaxLength(50).IsRequired();
        b.Property(x => x.JgStepId).IsRequired();
        b.Property(x => x.JobDecId).IsRequired();
        b.Property(x => x.PositionId).IsRequired();
        b.Property(x => x.ReqNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.ReqQuantity).IsRequired();
        b.Property(x => x.ReqReason).IsRequired();
        b.Property(x => x.StartDate).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.WorkforcePlanId).IsRequired();
        b.HasOne(x => x.WorkforcePlan).WithMany(x => x.JobRequisitions).HasForeignKey(x => x.WorkforcePlanId);
        b.HasOne(x => x.JobDec).WithMany().HasForeignKey(x => x.JobDecId);
    }
}

public class OnboardingAssignConfig : BaseEntityConfig<OnboardingAssign>
{
    public override void Configure(EntityTypeBuilder<OnboardingAssign> b)
    {
        base.Configure(b);
        b.Property(x => x.CompletedDate);
        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.IsMandatory).IsRequired();
        b.Property(x => x.OnboardingTaskId).IsRequired();
        b.Property(x => x.ScheduledDate).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.VerifyById);
        b.HasOne(x => x.OnboardingTask).WithMany().HasForeignKey(x => x.OnboardingTaskId);
    }
}

public class OnboardingTaskConfig : BaseEntityConfig<OnboardingTask>
{
    public override void Configure(EntityTypeBuilder<OnboardingTask> b)
    {
        base.Configure(b);
        b.Property(x => x.Description).IsRequired();
        b.Property(x => x.SequenceOrder).IsRequired();
        b.Property(x => x.TaskName).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.SequenceOrder);
    }
}

public class ResumeConfig : BaseEntityConfig<Resume>
{
    public override void Configure(EntityTypeBuilder<Resume> b)
    {
        base.Configure(b);
        b.Property(x => x.ContentType).HasMaxLength(50).IsRequired();
        b.Property(x => x.FileName).HasMaxLength(100).IsRequired();
        b.Property(x => x.FileSize).IsRequired();
        b.Property(x => x.JobAppId).IsRequired();
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId);
    }
}

public class ResumeBlobConfig : BaseEntityConfig<ResumeBlob>
{
    public override void Configure(EntityTypeBuilder<ResumeBlob> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).IsRequired();
        b.Property(x => x.ResumeId).IsRequired();
        b.HasOne(x => x.Resume).WithMany().HasForeignKey(x => x.ResumeId);
    }
}

public class WorkforcePlanConfig : BaseEntityConfig<WorkforcePlan>
{
    public override void Configure(EntityTypeBuilder<WorkforcePlan> b)
    {
        base.Configure(b);
        var p = b.Property(x => x.PlanCode)
                    .HasMaxLength(15)
                    .IsRequired()
                    .HasDefaultValueSql("generate_code('WFP'::text, EXTRACT(YEAR FROM CURRENT_DATE)::int)")
                    .ValueGeneratedOnAdd();
        p.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        b.Property(x => x.AppPositions).IsRequired();
        b.Property(x => x.DepartmentId).IsRequired();
        b.Property(x => x.Desc).IsRequired();
        b.Property(x => x.EndDate).IsRequired();
        b.Property(x => x.RequistionById).IsRequired();
        b.Property(x => x.StartDate).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.Title).IsRequired();
        b.Property(x => x.TotalPositions).IsRequired();
        b.Property(x => x.PeriodId);
        b.HasMany(x => x.JobRequisitions).WithOne(x => x.WorkforcePlan).HasForeignKey(x => x.WorkforcePlanId);
        b.HasMany(x => x.Reviews).WithOne(x => x.WorkforcePlan).HasForeignKey(x => x.WorkforcePlanId);
        b.HasIndex(x => x.PlanCode).IsUnique();
    }
}

public class WorkforcePlanReviewConfig : BaseEntityConfig<WorkforcePlanReview>
{
    public override void Configure(EntityTypeBuilder<WorkforcePlanReview> b)
    {
        base.Configure(b);
        b.Property(x => x.AppPositions).IsRequired();
        b.Property(x => x.Comment).IsRequired();
        b.Property(x => x.ReqPositions).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.WorkforcePlanId).IsRequired();
        b.HasOne(x => x.WorkforcePlan).WithMany(x => x.Reviews).HasForeignKey(x => x.WorkforcePlanId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.WorkforcePlanId, x.Status });
    }
}