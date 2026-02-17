using Microsoft.EntityFrameworkCore;
using Recruit.Domain.Entities;

namespace Recruit.Utility.Persistence;

public class HrmRecruitDbContext : DbContext
{
    public HrmRecruitDbContext(DbContextOptions<HrmRecruitDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrmRecruitDbContext).Assembly);
    }

    public DbSet<Applicant> Applicant { get; set; }
    public DbSet<ApplicantAddress> ApplicantAddress { get; set; }
    public DbSet<ApplicantContact> ApplicantContact { get; set; }
    public DbSet<ApplicantPerson> ApplicantPerson { get; set; }
    public DbSet<ApprovalInstance> ApprovalInstance { get; set; }
    public DbSet<ApprovalStep> ApprovalStep { get; set; }
    public DbSet<ApprovalWorkflow> ApprovalWorkflow { get; set; }
    public DbSet<CoverLetter> CoverLetter { get; set; }
    public DbSet<Interview> Interview { get; set; }
    public DbSet<InterviewFeedback> InterviewFeedback { get; set; }
    public DbSet<InterviewRound> InterviewRound { get; set; }
    public DbSet<JobApplication> JobApplication { get; set; }
    public DbSet<JobAppScreening> JobAppScreening { get; set; }
    public DbSet<JobDec> JobDec { get; set; }
    public DbSet<JobOffer> JobOffer { get; set; }
    public DbSet<JobOfferReview> JobOfferReview { get; set; }
    public DbSet<JobPosting> JobPosting { get; set; }
    public DbSet<JobPostReview> JobPostReview { get; set; }
    public DbSet<JobReqReview> JobReqReview { get; set; }
    public DbSet<JobRequisition> JobRequisition { get; set; }
    public DbSet<OnboardingTask> OnboardingTask { get; set; }
    public DbSet<Resume> Resume { get; set; }
    public DbSet<ResumeBlob> ResumeBlob { get; set; }
    public DbSet<WorkforcePlan> WorkforcePlan { get; set; }
    public DbSet<WorkforcePlanReview> WorkforcePlanReview { get; set; }
}