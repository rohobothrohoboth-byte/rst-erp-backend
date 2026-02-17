using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Domain.Entities;

namespace Recruit.Utility.Persistence;

public class ApplicantConfig : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RegisteredBy).HasMaxLength(100);
        b.HasOne(x => x.Person).WithOne().HasForeignKey<Applicant>(x => x.PersonId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Contact).WithOne().HasForeignKey<Applicant>(x => x.ContactId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Address).WithOne().HasForeignKey<Applicant>(x => x.AddressId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ApplicantAddressConfig : IEntityTypeConfiguration<ApplicantAddress>
{
    public void Configure(EntityTypeBuilder<ApplicantAddress> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Country).HasMaxLength(50);
        b.Property(x => x.Region).HasMaxLength(50);
        b.Property(x => x.Subcity).HasMaxLength(50);
        b.Property(x => x.Woreda).HasMaxLength(20);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ApplicantContactConfig : IEntityTypeConfiguration<ApplicantContact>
{
    public void Configure(EntityTypeBuilder<ApplicantContact> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(150);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ApplicantPersonConfig : IEntityTypeConfiguration<ApplicantPerson>
{
    public void Configure(EntityTypeBuilder<ApplicantPerson> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.FirstName).HasMaxLength(100);
        b.Property(x => x.MiddleName).HasMaxLength(100);
        b.Property(x => x.LastName).HasMaxLength(100);
        b.Property(x => x.FirstNameAm).HasMaxLength(100);
        b.Property(x => x.MiddleNameAm).HasMaxLength(100);
        b.Property(x => x.LastNameAm).HasMaxLength(100);
        b.Property(x => x.Gender).HasMaxLength(20);
        b.Property(x => x.Nationality).HasMaxLength(50);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class AccrualHistoryConf : IEntityTypeConfiguration<ApprovalInstance>
{
    public void Configure(EntityTypeBuilder<ApprovalInstance> b)
    {
        b.HasKey(e => e.Id);
        b.Property(x => x.EntityType).IsRequired().HasMaxLength(100);
        b.HasIndex(x => new { x.EntityType, x.EntityId, x.Status });
        b.HasOne(x => x.ApprovalStep).WithMany(x => x.ApprovalInstances).HasForeignKey(x => x.ApprovalStepId);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ApprovalStepConfig : IEntityTypeConfiguration<ApprovalStep>
{
    public void Configure(EntityTypeBuilder<ApprovalStep> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.StepOrder).IsRequired();
        b.Property(x => x.StepName).HasMaxLength(150);
        b.Property(x => x.ApproverRole).HasMaxLength(100);
        b.HasIndex(x => new { x.WorkflowId, x.StepOrder }).IsUnique();
        b.HasMany(x => x.ApprovalInstances).WithOne(x => x.ApprovalStep).HasForeignKey(x => x.ApprovalStepId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ApprovalWorkflowConfig : IEntityTypeConfiguration<ApprovalWorkflow>
{
    public void Configure(EntityTypeBuilder<ApprovalWorkflow> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.WorkflowCode).IsRequired().HasMaxLength(50);
        b.Property(x => x.WorkflowName).IsRequired().HasMaxLength(200);
        b.Property(x => x.EntityType).IsRequired().HasMaxLength(100);
        b.HasIndex(x => new { x.WorkflowCode, x.EntityType }).IsUnique();
        b.HasMany(x => x.ApprovalSteps).WithOne(x => x.Workflow).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class CoverLetterConfig : IEntityTypeConfiguration<CoverLetter>
{
    public void Configure(EntityTypeBuilder<CoverLetter> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Content);
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class InterviewConfig : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.InterviewType).HasMaxLength(50);
        b.Property(x => x.Status).HasMaxLength(50);
        b.HasMany(x => x.Rounds).WithOne(x => x.Interview).HasForeignKey(x => x.InterviewId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class InterviewFeedbackConfig : IEntityTypeConfiguration<InterviewFeedback>
{
    public void Configure(EntityTypeBuilder<InterviewFeedback> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Recommendation).HasMaxLength(50);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class InterviewRoundConfig : IEntityTypeConfiguration<InterviewRound>
{
    public void Configure(EntityTypeBuilder<InterviewRound> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RoundName).HasMaxLength(150);
        b.Property(x => x.Status).HasMaxLength(50);
        b.HasIndex(x => new { x.InterviewId, x.RoundNumber }).IsUnique();
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobApplicationConfig : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).IsRequired().HasMaxLength(50);
        b.HasIndex(x => new { x.ApplicantId, x.JobPostingId, x.EmployeeId }).IsUnique();
        b.HasMany(x => x.Interviews).WithOne(x => x.JobApplication).HasForeignKey(x => x.JobApplicationId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobAppScreeningConfig : IEntityTypeConfiguration<JobAppScreening>
{
    public void Configure(EntityTypeBuilder<JobAppScreening> b)
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ScreeningById, x.JobAppId, x.Score }).IsUnique();
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobDecConfig : IEntityTypeConfiguration<JobDec>
{
    public void Configure(EntityTypeBuilder<JobDec> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Desc).HasMaxLength(4000).IsRequired();
        b.Property(x => x.Qualification).HasMaxLength(1000);
        b.Property(x => x.KeySkills).HasMaxLength(1000);
        b.Property(x => x.WorkLocation).HasMaxLength(200);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasIndex(x => x.Title);
        b.HasIndex(x => x.PreGender);
        b.HasIndex(x => x.ContractType);
    }
}

public class JobOfferConfig : IEntityTypeConfiguration<JobOffer>
{
    public void Configure(EntityTypeBuilder<JobOffer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.OfferNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.Status).HasMaxLength(50);
        b.HasIndex(x => x.OfferNumber).IsUnique();
        b.HasOne(x => x.CanApplication).WithMany().HasForeignKey(x => x.CanApplicationId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobOfferReviewConfig : IEntityTypeConfiguration<JobOfferReview>
{
    public void Configure(EntityTypeBuilder<JobOfferReview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RejectionReason).HasMaxLength(500);
        b.Property(x => x.ApprovalComments).HasMaxLength(500);
        b.HasIndex(x => x.JobOfferId).IsUnique();
        b.HasOne(x => x.JobOffer).WithOne().HasForeignKey<JobOfferReview>(x => x.JobOfferId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobPostingConfig : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PostNumber).IsRequired().HasMaxLength(50);
        b.Property(x => x.PostType).IsRequired().HasMaxLength(50);
        b.HasMany(x => x.Applications).WithOne(x => x.JobPosting).HasForeignKey(x => x.JobPostingId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.Status, x.PostType, x.JobReqId });
        b.HasIndex(x => x.PostNumber).IsUnique();
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobPostReviewConfig : IEntityTypeConfiguration<JobPostReview>
{
    public void Configure(EntityTypeBuilder<JobPostReview> b)
    {

        b.HasKey(x => x.Id);
        b.Property(x => x.Comment).HasMaxLength(2000);
        b.HasIndex(x => new { x.Status, x.JobPostingId, x.ReviewById });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class JobReqReviewConfig : IEntityTypeConfiguration<JobReqReview>
{
    public void Configure(EntityTypeBuilder<JobReqReview> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Comment).HasMaxLength(2000).IsRequired();
        b.Property(x => x.ReqQuantity).IsRequired();
        b.Property(x => x.AppQuantity).IsRequired();
        b.Property(x => x.ReviewById).IsRequired();
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.JobReqId).IsRequired();
        b.HasOne(x => x.JobReq).WithMany(j => j.Reviews).HasForeignKey(x => x.JobReqId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.JobReqId);
        b.HasIndex(x => x.Status);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
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
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.StartDate).IsRequired();
        b.HasOne(x => x.WorkforcePlan).WithMany().HasForeignKey(x => x.WorkforcePlanId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.JobDec).WithMany().HasForeignKey(x => x.JobDecId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Reviews).WithOne(r => r.JobReq).HasForeignKey(r => r.JobReqId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.WorkforcePlanId);
        b.HasIndex(x => x.JobDecId);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class OnboardingTaskConfig : IEntityTypeConfiguration<OnboardingTask>
{
    public void Configure(EntityTypeBuilder<OnboardingTask> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.TaskName).IsRequired().HasMaxLength(150);
        b.Property(x => x.Category).HasMaxLength(50);
        b.Property(x => x.AssignedTo).HasMaxLength(100);
        b.Property(x => x.Status).IsRequired().HasMaxLength(50);
        b.HasIndex(x => new { x.EmployeeId, x.SequenceOrder });
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ResumeConfig : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        b.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        b.HasOne(x => x.JobApp).WithMany().HasForeignKey(x => x.JobAppId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class ResumeBlobConfig : IEntityTypeConfiguration<ResumeBlob>
{
    public void Configure(EntityTypeBuilder<ResumeBlob> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Data).IsRequired();
        b.HasIndex(x => x.ResumeId).IsUnique();
        b.HasOne(x => x.Resume).WithOne().HasForeignKey<ResumeBlob>(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class WorkforcePlanConfig : IEntityTypeConfiguration<WorkforcePlan>
{
    public void Configure(EntityTypeBuilder<WorkforcePlan> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.PlanCode).IsRequired().HasMaxLength(50);
        b.Property(x => x.Title).IsRequired().HasMaxLength(150);
        b.Property(x => x.Status).IsRequired().HasMaxLength(50);
        b.HasIndex(x => x.PlanCode).IsUnique();
        b.HasMany(x => x.JobRequisitions).WithOne(x => x.WorkforcePlan).HasForeignKey(x => x.WorkforcePlanId).OnDelete(DeleteBehavior.Restrict);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class WorkforcePlanReviewConfig : IEntityTypeConfiguration<WorkforcePlanReview>
{
    public void Configure(EntityTypeBuilder<WorkforcePlanReview> b)
    {

        b.HasKey(x => x.Id);
        b.Property(x => x.Comment).HasMaxLength(2000);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.ReqPositions).IsRequired();
        b.Property(x => x.AppPositions).IsRequired();
        b.HasOne(x => x.WorkforcePlan).WithMany(p => p.Reviews).HasForeignKey(x => x.WorkforcePlanId).OnDelete(DeleteBehavior.Cascade);
        b.Property(e => e.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}