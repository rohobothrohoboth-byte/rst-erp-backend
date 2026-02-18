using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Domain.Entities;

namespace Recruit.Utility.Persistence;

public class ApplicantConfig : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RegisteredDate).IsRequired();
        b.Property(x => x.RegisteredBy).HasMaxLength(150).IsRequired();
        b.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.PersonId); b.HasIndex(x => x.ContactId); b.HasIndex(x => x.AddressId);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ApplicantAddressConfig : IEntityTypeConfiguration<ApplicantAddress>
{
    public void Configure(EntityTypeBuilder<ApplicantAddress> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.AddressType).HasMaxLength(20).IsRequired();
        b.Property(x => x.Country).HasMaxLength(100).IsRequired();
        b.Property(x => x.Region).HasMaxLength(100).IsRequired();
        b.Property(x => x.Subcity).HasMaxLength(100);
        b.Property(x => x.Zone).HasMaxLength(100);
        b.Property(x => x.Woreda).HasMaxLength(100);
        b.Property(x => x.Kebele).HasMaxLength(100);
        b.Property(x => x.HouseNo).HasMaxLength(50);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ApplicantContactConfig : IEntityTypeConfiguration<ApplicantContact>
{
    public void Configure(EntityTypeBuilder<ApplicantContact> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Phone).HasMaxLength(20).IsRequired();
        b.Property(x => x.AlternatePhone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(150).IsRequired();
        b.Property(x => x.PoBox).HasMaxLength(50);
        b.Property(x => x.Fax).HasMaxLength(50);
        b.HasIndex(x => x.Email);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ApplicantPersonConfig : IEntityTypeConfiguration<ApplicantPerson>
{
    public void Configure(EntityTypeBuilder<ApplicantPerson> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.FirstNameAm).HasMaxLength(100);
        b.Property(x => x.MiddleName).HasMaxLength(100);
        b.Property(x => x.MiddleNameAm).HasMaxLength(100);
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastNameAm).HasMaxLength(100);
        b.Property(x => x.Gender).HasMaxLength(20).IsRequired();
        b.Property(x => x.Nationality).HasMaxLength(100).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ApplicationRankingConfig : IEntityTypeConfiguration<ApplicationRanking>
{
    public void Configure(EntityTypeBuilder<ApplicationRanking> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.TotalScore).IsRequired();
        b.Property(x => x.Rank).IsRequired();
        b.Property(x => x.JobAppId).IsRequired();
        b.HasIndex(x => x.JobAppId).IsUnique(); // One ranking per application
        b.HasIndex(x => x.Rank);
        b.HasIndex(x => x.TotalScore);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class CoverLetterConfig : IEntityTypeConfiguration<CoverLetter>
{
    public void Configure(EntityTypeBuilder<CoverLetter> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Content).HasColumnType("text").IsRequired();
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.JobAppId).IsUnique();
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class EvaluationFlowConfig : IEntityTypeConfiguration<EvaluationFlow>
{
    public void Configure(EntityTypeBuilder<EvaluationFlow> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.IsGlobal).IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.HasMany(x => x.Steps).WithOne(x => x.EvaluationFlow).HasForeignKey(x => x.EvaluationFlowId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class EvaluationScoreConfig : IEntityTypeConfiguration<EvaluationScore>
{
    public void Configure(EntityTypeBuilder<EvaluationScore> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Score).HasPrecision(5, 2).IsRequired();
        b.Property(x => x.IsCurrent).IsRequired();
        b.Property(x => x.Feedback).HasMaxLength(1000);
        b.Property(x => x.EvaluatorId).IsRequired();
        b.HasOne(x => x.EvaluationStep).WithMany().HasForeignKey(x => x.EvaluationStepId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.EvalType).WithMany().HasForeignKey(x => x.EvalTypeId).OnDelete(DeleteBehavior.Restrict);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class EvaluationStepConfig : IEntityTypeConfiguration<EvaluationStep>
{
    public void Configure(EntityTypeBuilder<EvaluationStep> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.StepName).HasMaxLength(150).IsRequired();
        b.Property(x => x.StepOrder).IsRequired();
        b.Property(x => x.IsFinal).IsRequired();
        b.HasOne(x => x.EvaluationFlow).WithMany(x => x.Steps).HasForeignKey(x => x.EvaluationFlowId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.EvaluationFlowId, x.StepOrder }).IsUnique();
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class EvaluationTypeConfig : IEntityTypeConfiguration<EvaluationType>
{
    public void Configure(EntityTypeBuilder<EvaluationType> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.MaxScore).HasPrecision(5, 2).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobApplicationConfig : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.PostType).HasMaxLength(50).IsRequired();
        b.Property(x => x.AppliedDate).IsRequired();
        b.HasOne(x => x.JobPosting).WithMany(x => x.Applications).HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.JobPostingId);
        b.HasIndex(x => x.ApplicantId);
        b.HasIndex(x => x.EmployeeId);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobDecConfig : IEntityTypeConfiguration<JobDec>
{
    public void Configure(EntityTypeBuilder<JobDec> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Desc).HasColumnType("text").IsRequired();
        b.Property(x => x.Qualification).HasColumnType("text");
        b.Property(x => x.KeySkills).HasColumnType("text");
        b.Property(x => x.WorkLocation).HasMaxLength(150);
        b.Property(x => x.PreGender).HasMaxLength(20);
        b.Property(x => x.ContractType).HasMaxLength(50);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobOfferConfig : IEntityTypeConfiguration<JobOffer>
{
    public void Configure(EntityTypeBuilder<JobOffer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.OfferNumber).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.OfferNumber).IsUnique();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.OfferDate).IsRequired();
        b.Property(x => x.ExpirationDate).IsRequired();
        b.Property(x => x.OfferDocument).HasMaxLength(500);
        b.HasOne(x => x.JobApplication).WithMany().HasForeignKey(x => x.JobApplicationId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.Restrict);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobOfferApprovalConfig : IEntityTypeConfiguration<JobOfferApproval>
{
    public void Configure(EntityTypeBuilder<JobOfferApproval> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.StepOrder).IsRequired();
        b.Property(x => x.Role).HasMaxLength(50).IsRequired();
        b.Property(x => x.Status).HasMaxLength(30).IsRequired();
        b.Property(x => x.ApprovedDate).HasColumnType("datetime2").IsRequired(false);
        b.Property(x => x.ApprovedById).IsRequired(false);
        b.Property(x => x.JobOfferId).IsRequired();
        b.HasIndex(x => x.JobOfferId);
        b.HasIndex(x => new { x.JobOfferId, x.StepOrder }).IsUnique(); // Prevent duplicate step order per offer
        b.HasIndex(x => x.Status);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobOfferReviewConfig : IEntityTypeConfiguration<JobOfferReview>
{
    public void Configure(EntityTypeBuilder<JobOfferReview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.AcceptanceDate);
        b.Property(x => x.RejectionDate);
        b.Property(x => x.RejectionReason).HasMaxLength(1000);
        b.Property(x => x.ApprovalComments).HasMaxLength(1000);
        b.HasOne(x => x.JobOffer).WithMany().HasForeignKey(x => x.JobOfferId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.JobOfferId);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobPostEvalFlowConfig : IEntityTypeConfiguration<JobPostEvalFlow>
{
    public void Configure(EntityTypeBuilder<JobPostEvalFlow> b)
    {
        b.HasKey(x => x.Id);
        b.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.EvaluationFlow).WithMany().HasForeignKey(x => x.EvaluationFlowId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.JobPostingId).IsUnique(); // One flow per posting
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobPostingConfig : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PostNumber).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.PostNumber).IsUnique();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.PostType).HasMaxLength(50).IsRequired();
        b.Property(x => x.PublishedDate).IsRequired();
        b.Property(x => x.DeadlineDate).IsRequired();
        b.HasOne(x => x.JobReq).WithMany().HasForeignKey(x => x.JobReqId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Applications).WithOne(x => x.JobPosting).HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobPostReviewConfig : IEntityTypeConfiguration<JobPostReview>
{
    public void Configure(EntityTypeBuilder<JobPostReview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Comment).HasMaxLength(1000).IsRequired();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.JobPostingId);
        b.HasIndex(x => x.ReviewById);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobReqReviewConfig : IEntityTypeConfiguration<JobReqReview>
{
    public void Configure(EntityTypeBuilder<JobReqReview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Comment).HasMaxLength(1000).IsRequired();
        b.Property(x => x.ReqQuantity).IsRequired();
        b.Property(x => x.AppQuantity).IsRequired();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.HasOne(x => x.JobReq).WithMany(x => x.Reviews).HasForeignKey(x => x.JobReqId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.JobReqId);
        b.HasIndex(x => x.ReviewById);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class JobRequisitionConfig : IEntityTypeConfiguration<JobRequisition>
{
    public void Configure(EntityTypeBuilder<JobRequisition> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ReqNumber).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.ReqNumber).IsUnique();
        b.Property(x => x.ReqReason).HasMaxLength(500).IsRequired();
        b.Property(x => x.ReqQuantity).IsRequired();
        b.Property(x => x.BudgetCode).HasMaxLength(100).IsRequired();
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.StartDate).IsRequired();
        b.HasOne(x => x.WorkforcePlan).WithMany(w => w.JobRequisitions).HasForeignKey(x => x.WorkforcePlanId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.JobDec).WithMany().HasForeignKey(x => x.JobDecId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Reviews).WithOne(r => r.JobReq).HasForeignKey(r => r.JobReqId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.WorkforcePlanId);
        b.HasIndex(x => x.JobDecId);
        b.HasIndex(x => new { x.WorkforcePlanId, x.PositionId }).HasDatabaseName("IX_JobReq_WorkforcePlan_Position");
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class OnboardingAssignConfig : IEntityTypeConfiguration<OnboardingAssign>
{
    public void Configure(EntityTypeBuilder<OnboardingAssign> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.IsMandatory).IsRequired();
        b.Property(x => x.Status).HasMaxLength(30).IsRequired();
        b.Property(x => x.ScheduledDate).IsRequired();
        b.Property(x => x.CompletedDate).IsRequired(false);
        b.Property(x => x.VerifyById).IsRequired(false);
        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.OnboardingTaskId).IsRequired();
        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => x.OnboardingTaskId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ScheduledDate);
        b.HasIndex(x => new { x.EmployeeId, x.OnboardingTaskId }).IsUnique();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class OnboardingTaskConfig : IEntityTypeConfiguration<OnboardingTask>
{
    public void Configure(EntityTypeBuilder<OnboardingTask> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.TaskName).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasColumnType("text");
        b.Property(x => x.SequenceOrder).IsRequired();
        b.HasIndex(x => x.SequenceOrder);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ResumeConfig : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.FileName).HasMaxLength(250).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        b.Property(x => x.FileSize).IsRequired();
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ResumeBlobConfig : IEntityTypeConfiguration<ResumeBlob>
{
    public void Configure(EntityTypeBuilder<ResumeBlob> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.HasOne(x => x.Resume).WithOne().HasForeignKey<ResumeBlob>(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class WorkforcePlanConfig : IEntityTypeConfiguration<WorkforcePlan>
{
    public void Configure(EntityTypeBuilder<WorkforcePlan> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PlanCode).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.PlanCode).IsUnique();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Desc).HasMaxLength(1000);
        b.Property(x => x.TotalPositions).IsRequired();
        b.Property(x => x.AppPositions).IsRequired();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.HasMany(x => x.JobRequisitions).WithOne(x => x.WorkforcePlan).HasForeignKey(x => x.WorkforcePlanId);
        b.HasMany(x => x.Reviews).WithOne(x => x.WorkforcePlan).HasForeignKey(x => x.WorkforcePlanId);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class WorkforcePlanReviewConfig : IEntityTypeConfiguration<WorkforcePlanReview>
{
    public void Configure(EntityTypeBuilder<WorkforcePlanReview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Comment).HasMaxLength(1000).IsRequired();
        b.Property(x => x.ReqPositions).IsRequired();
        b.Property(x => x.AppPositions).IsRequired();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.HasOne(x => x.WorkforcePlan).WithMany(x => x.Reviews).HasForeignKey(x => x.WorkforcePlanId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.WorkforcePlanId);
        b.HasIndex(x => x.ReviewById);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}