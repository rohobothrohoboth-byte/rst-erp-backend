using Microsoft.EntityFrameworkCore;
using Recruit.Domain.Entities;
using System.Data;

namespace Recruit.Utility.Persistence;

public class HrmRecruitDbContext : DbContext
{
    public HrmRecruitDbContext(DbContextOptions<HrmRecruitDbContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var indexes = entityType.GetIndexes().Where(i => i.IsUnique);
            foreach (var index in indexes)
            {
                index.SetFilter("\"IsDeleted\" = false");
            }
        }

        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrmRecruitDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DateAdd = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.DateMod = now;
                    break;
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DBConcurrencyException("The record was modified by another transaction.", ex);
        }
    }

    public DbSet<Applicant> Applicant { get; set; }
    public DbSet<ApplicantAddress> ApplicantAddress { get; set; }
    public DbSet<ApplicantContact> ApplicantContact { get; set; }
    public DbSet<ApplicantPerson> ApplicantPerson { get; set; }
    public DbSet<ApplicationRanking> ApplicationRanking { get; set; }
    public DbSet<CoverLetter> CoverLetter { get; set; }
    public DbSet<EvaluationFlow> EvaluationFlow { get; set; }
    public DbSet<EvaluationScore> EvaluationScore { get; set; }
    public DbSet<EvaluationStep> EvaluationStep { get; set; }
    public DbSet<EvaluationType> EvaluationType { get; set; }
    public DbSet<JobAppEvalProgress> JobAppEvalProgress { get; set; }
    public DbSet<JobApplication> JobApplication { get; set; }
    public DbSet<JobDec> JobDec { get; set; }
    public DbSet<JobOffer> JobOffer { get; set; }
    public DbSet<JobOfferApproval> JobOfferApproval { get; set; }
    public DbSet<JobOfferReview> JobOfferReview { get; set; }
    public DbSet<JobPostEvalFlow> JobPostEvalFlow { get; set; }
    public DbSet<JobPosting> JobPosting { get; set; }
    public DbSet<JobPostReview> JobPostReview { get; set; }
    public DbSet<JobReqReview> JobReqReview { get; set; }
    public DbSet<JobRequisition> JobRequisition { get; set; }
    public DbSet<OnboardingAssign> OnboardingAssign { get; set; }
    public DbSet<OnboardingTask> OnboardingTask { get; set; }
    public DbSet<Resume> Resume { get; set; }
    public DbSet<ResumeBlob> ResumeBlob { get; set; }
    public DbSet<WorkforcePlan> WorkforcePlan { get; set; }
    public DbSet<WorkforcePlanReview> WorkforcePlanReview { get; set; }
}