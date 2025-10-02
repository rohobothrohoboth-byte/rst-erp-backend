using Microsoft.EntityFrameworkCore;
using Svc.Lup.Models;

namespace Svc.Lup.Persistence;

public class LupDbContext : DbContext
{
    public LupDbContext(DbContextOptions<LupDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public DbSet<AbsentReason> AbsentReason { get; set; }
    public DbSet<AddressType> AddressType { get; set; }
    public DbSet<AdmissionType> AdmissionType { get; set; }
    public DbSet<AwardReason> AwardReason { get; set; }
    public DbSet<AwardType> AwardType { get; set; }
    public DbSet<CommitteeRole> CommitteeRole { get; set; }
    public DbSet<CriterionType> CriterionType { get; set; }
    public DbSet<EmploymentNature> EmploymentNature { get; set; }
    public DbSet<EmploymentType> EmploymentType { get; set; }
    public DbSet<EducationLevel> EducationLevel { get; set; }
    public DbSet<HolidayCondition> HolidayCondition { get; set; }
    public DbSet<LanguageSkill> LanguageSkill { get; set; }
    public DbSet<LeaveCondition> LeaveCondition { get; set; }
    public DbSet<LeaveType> LeaveType { get; set; }
    public DbSet<LeaveUsage> LeaveUsage { get; set; }
    public DbSet<MaritalStatus> MaritalStatus { get; set; }
    public DbSet<MeasureTaken> MeasureTaken { get; set; }
    public DbSet<MeasureType> MeasureType { get; set; }
    public DbSet<PerformanceEvaluation> PerformanceEvaluation { get; set; }
    public DbSet<PositionChangeReason> PositionChangeReason { get; set; }
    public DbSet<ProfessionType> ProfessionType { get; set; }
    public DbSet<Quarter> Quarter { get; set; }
    public DbSet<Rating> Rating { get; set; }
    public DbSet<Region> Region { get; set; }
    public DbSet<Relation> Relation { get; set; }
    public DbSet<ReportType> ReportType { get; set; }
    public DbSet<SalaryChangeReason> SalaryChangeReason { get; set; }
    public DbSet<SkillLevel> SkillLevel { get; set; }
    public DbSet<SponsorType> SponsorType { get; set; }
    public DbSet<TerminationReason> TerminationReason { get; set; }
    public DbSet<TrainingSource> TrainingSource { get; set; }
    public DbSet<TrainingType> TrainingType { get; set; }
    public DbSet<TransferReason> TransferReason { get; set; }
    public DbSet<VoucherType> VoucherType { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }
    //public DbSet<LeaveUsage> LeaveUsage { get; set; }

}
