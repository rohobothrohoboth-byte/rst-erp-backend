using Microsoft.EntityFrameworkCore;
using Cor.CRM.Models.Entities;
using TaskEntity = Cor.CRM.Models.Entities.Task;
using Cor.CRM.Models.Entities.Local;

namespace Cor.CRM.Persistence
{
    public class CrmDbContext : DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options)
            : base(options)
        {
        }

        // Core Entities
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteLine> QuoteLines { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<SmsLog> SmsLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Integration> Integrations { get; set; }
        public DbSet<LeadScoreRule> LeadScoreRules { get; set; }
        public DbSet<LeadRoutingRule> LeadRoutingRules { get; set; }
        public DbSet<CustomFieldDefinition> CustomFieldDefinitions { get; set; }
        public DbSet<CampaignLead> CampaignLeads { get; set; }
        public DbSet<CampaignCustomer> CampaignCustomers { get; set; }
        public DbSet<LeadTask> LeadTasks { get; set; }
        public DbSet<OpportunityProduct> OpportunityProducts { get; set; }

        // Local copies
        public DbSet<LocalCompany> LocalCompanies { get; set; }
        public DbSet<LocalBranch> LocalBranches { get; set; }
        public DbSet<LocalDepartment> LocalDepartments { get; set; }
        public DbSet<LocalEmployee> LocalEmployees { get; set; }
        public DbSet<LocalPosition> LocalPositions { get; set; }
        public DbSet<LocalJobGrade> LocalJobGrades { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        public DbSet<SocialMediaAccount> SocialMediaAccounts { get; set; }
        public DbSet<SocialMediaPost> SocialMediaPosts { get; set; }
        public DbSet<SMSCampaign> SMSCampaigns { get; set; }
        public DbSet<EmailCampaign> EmailCampaigns { get; set; }

        public DbSet<Commission> Commissions { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<PropertyInquiry> PropertyInquiry { get; set; }
        public DbSet<PropertyViewing> PropertyViewings { get; set; }
        public DbSet<RealEstateTransaction> RealEstateTransactions { get; set; }
        public DbSet<ContractLine> ContractLines { get; set; }
         public DbSet<Ticket> Tickets { get; set; }
          public DbSet<TicketComment> TicketComments { get; set; }
           public DbSet<TicketAttachment> TicketAttachments { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================================
            // SOFT DELETE FILTER - FIXED
            // ============================================================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Set CreatedAt default value
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.CreatedAt))
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    // Apply soft delete filter
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                    var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            // ============================================================
            // FIX: Global Query Filter Relationship Warnings
            // Make relationships optional to avoid filter conflicts
            // ============================================================

            // Opportunity -> OpportunityProduct - Make optional
            modelBuilder.Entity<Opportunity>()
                .HasMany(o => o.OpportunityProducts)
                .WithOne(op => op.Opportunity)
                .HasForeignKey(op => op.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // ? FIX: Makes optional

            // Team -> TeamMember - Make optional
            modelBuilder.Entity<Team>()
                .HasMany(t => t.TeamMembers)
                .WithOne(tm => tm.Team)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // ? FIX: Makes optional

            // Apply matching filters to child entities (alternative approach)
            modelBuilder.Entity<OpportunityProduct>().HasQueryFilter(op => !op.IsDeleted);
            modelBuilder.Entity<TeamMember>().HasQueryFilter(tm => !tm.IsDeleted);

            // ============================================================
            // FIX: Add Sentinel Values for all Enums with Defaults
            // ============================================================

            // LEAD
            modelBuilder.Entity<Lead>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Lead_Email");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Lead_Status");
                entity.HasIndex(e => e.AssignedToUserId).HasDatabaseName("IX_Lead_AssignedToUserId");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Lead_CreatedAt");
                entity.HasIndex(e => e.Score).HasDatabaseName("IX_Lead_Score");

                entity.Property(e => e.CustomFieldsJson).HasColumnType("jsonb");
                entity.Property(e => e.Budget).HasPrecision(18, 2);
                entity.Property(e => e.EstimatedValue).HasPrecision(18, 2);
                entity.Property(e => e.PropertyPrice).HasPrecision(18, 2);

                // ? FIXED: Added HasSentinel
                entity.Property(e => e.Status)
                    .HasDefaultValue(LeadStatus.New)
                    .HasSentinel(LeadStatus.New);
                entity.Property(e => e.Source)
                    .HasDefaultValue(LeadSource.Website)
                    .HasSentinel(LeadSource.Website);
                entity.Property(e => e.Priority)
                    .HasDefaultValue(LeadPriority.Medium)
                    .HasSentinel(LeadPriority.Medium);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Score).HasDefaultValue(0);
                entity.Property(e => e.EngagementScore).HasDefaultValue(0);
                entity.Property(e => e.ContactCount).HasDefaultValue(0);

                // Relationships
                entity.HasMany(l => l.Activities)
                    .WithOne(a => a.Lead)
                    .HasForeignKey(a => a.LeadId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(l => l.Tasks)
                    .WithOne(t => t.Lead)
                    .HasForeignKey(t => t.LeadId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(l => l.Notes)
                    .WithOne(n => n.Lead)
                    .HasForeignKey(n => n.LeadId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(l => l.Opportunities)
                    .WithOne(o => o.Lead)
                    .HasForeignKey(o => o.LeadId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(l => l.Campaigns)
                    .WithMany(c => c.Leads)
                    .UsingEntity<CampaignLead>(
                        j => j.HasOne(cl => cl.Campaign)
                            .WithMany()
                            .HasForeignKey(cl => cl.CampaignId),
                        j => j.HasOne(cl => cl.Lead)
                            .WithMany()
                            .HasForeignKey(cl => cl.LeadId),
                        j => j.HasKey(cl => new { cl.CampaignId, cl.LeadId })
                    );
            });

            // CUSTOMER
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Customer_Email");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Customer_Status");
                entity.HasIndex(e => e.Type).HasDatabaseName("IX_Customer_Type");

                entity.Property(e => e.AnnualRevenue).HasPrecision(18, 2);
                entity.Property(e => e.LifetimeValue).HasPrecision(18, 2);

                // ? FIXED: Added HasSentinel
                entity.Property(e => e.Status)
                    .HasDefaultValue(CustomerStatus.Active)
                    .HasSentinel(CustomerStatus.Active);
                entity.Property(e => e.Type)
                    .HasDefaultValue(CustomerType.Individual)
                    .HasSentinel(CustomerType.Individual);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.TotalOrders).HasDefaultValue(0);

                // Relationships
                entity.HasMany(c => c.Contacts)
                    .WithOne(c => c.Customer)
                    .HasForeignKey(c => c.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Opportunities)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Activities)
                    .WithOne(a => a.Customer)
                    .HasForeignKey(a => a.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Notes)
                    .WithOne(n => n.Customer)
                    .HasForeignKey(n => n.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Campaigns)
                    .WithMany(c => c.Customers)
                    .UsingEntity<CampaignCustomer>(
                        j => j.HasOne(cc => cc.Campaign)
                            .WithMany()
                            .HasForeignKey(cc => cc.CampaignId),
                        j => j.HasOne(cc => cc.Customer)
                            .WithMany()
                            .HasForeignKey(cc => cc.CustomerId),
                        j => j.HasKey(cc => new { cc.CampaignId, cc.CustomerId })
                    );
            });

            // CONTACT
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Contact_Email");
                entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Contact_CustomerId");

                entity.Property(e => e.IsPrimary).HasDefaultValue(false);
                entity.Property(e => e.IsDecisionMaker).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ContactCount).HasDefaultValue(0);
            });

            // OPPORTUNITY
            modelBuilder.Entity<Opportunity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Stage).HasDatabaseName("IX_Opportunity_Stage");
                entity.HasIndex(e => e.ExpectedCloseDate).HasDatabaseName("IX_Opportunity_ExpectedCloseDate");
                entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Opportunity_CustomerId");

                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.ForecastedRevenue).HasPrecision(18, 2);
                entity.Property(e => e.ProductsJson).HasColumnType("jsonb");

                // ? FIXED: Added HasSentinel
                entity.Property(e => e.Stage)
                    .HasDefaultValue(OpportunityStage.Discovery)
                    .HasSentinel(OpportunityStage.Discovery);
                entity.Property(e => e.WinProbability)
                    .HasDefaultValue(WinProbability.Medium)
                    .HasSentinel(WinProbability.Medium);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ActivityCount).HasDefaultValue(0);

                // Relationships - fixed to be optional
                entity.HasMany(o => o.Activities)
                    .WithOne(a => a.Opportunity)
                    .HasForeignKey(a => a.OpportunityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(o => o.Tasks)
                    .WithOne(t => t.Opportunity)
                    .HasForeignKey(t => t.OpportunityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(o => o.Notes)
                    .WithOne(n => n.Opportunity)
                    .HasForeignKey(n => n.OpportunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                // OpportunityProduct relationship - already fixed above
            });

            // ACTIVITY
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StartDateTime).HasDatabaseName("IX_Activity_StartDateTime");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Activity_Status");
                entity.HasIndex(e => e.AssignedToUserId).HasDatabaseName("IX_Activity_AssignedToUserId");

                entity.Property(e => e.MetadataJson).HasColumnType("jsonb");

                // ? FIXED: Added HasSentinel
                entity.Property(e => e.Status)
                    .HasDefaultValue(ActivityStatus.Scheduled)
                    .HasSentinel(ActivityStatus.Scheduled);
                entity.Property(e => e.IsAllDay).HasDefaultValue(false);
                entity.Property(e => e.DurationMinutes).HasDefaultValue(0);
            });

            // TASK
            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DueDate).HasDatabaseName("IX_Task_DueDate");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Task_Status");
                entity.HasIndex(e => e.Priority).HasDatabaseName("IX_Task_Priority");
                entity.HasIndex(e => e.AssignedToUserId).HasDatabaseName("IX_Task_AssignedToUserId");

                entity.Property(e => e.CompletionPercentage).HasPrecision(5, 2);

                // ? FIXED: Added HasSentinel
                entity.Property(e => e.Status)
                    .HasDefaultValue(TaskState.Pending)
                    .HasSentinel(TaskState.Pending);
                entity.Property(e => e.Priority)
                    .HasDefaultValue(TaskPriority.Medium)
                    .HasSentinel(TaskPriority.Medium);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsRecurring).HasDefaultValue(false);
                entity.Property(e => e.EstimatedHours).HasDefaultValue(0);
                entity.Property(e => e.ActualHours).HasDefaultValue(0);

                // Relationships with LeadTasks
                if (modelBuilder.Model.FindEntityType(typeof(LeadTask)) != null)
                {
                    entity.HasOne(t => t.Lead)
                        .WithMany(l => l.Tasks)
                        .HasForeignKey(t => t.LeadId)
                        .OnDelete(DeleteBehavior.SetNull);
                }
            });

            // NOTE
            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.LeadId).HasDatabaseName("IX_Note_LeadId");
                entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Note_CustomerId");
                entity.HasIndex(e => e.IsPinned).HasDatabaseName("IX_Note_IsPinned");

                entity.Property(e => e.IsPinned).HasDefaultValue(false);
                entity.Property(e => e.IsPrivate).HasDefaultValue(false);
            });

            // CAMPAIGN
            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Campaign_Status");
                entity.HasIndex(e => e.Type).HasDatabaseName("IX_Campaign_Type");
                entity.HasIndex(e => e.StartDate).HasDatabaseName("IX_Campaign_StartDate");

                entity.Property(e => e.Budget).HasPrecision(18, 2);
                entity.Property(e => e.ActualCost).HasPrecision(18, 2);
                entity.Property(e => e.ExpectedRevenue).HasPrecision(18, 2);
                entity.Property(e => e.ActualRevenue).HasPrecision(18, 2);
                entity.Property(e => e.ConversionRate).HasPrecision(5, 2);
                entity.Property(e => e.EngagementRate).HasPrecision(5, 2);
                entity.Property(e => e.MetricsJson).HasColumnType("jsonb");
                entity.Property(e => e.ContentJson).HasColumnType("jsonb");

                // ? FIXED: Added HasSentinel
                entity.Property(e => e.Status)
                    .HasDefaultValue(CampaignStatus.Draft)
                    .HasSentinel(CampaignStatus.Draft);
                entity.Property(e => e.Type)
                    .HasDefaultValue(CampaignType.Email)
                    .HasSentinel(CampaignType.Email);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.TargetCount).HasDefaultValue(0);
                entity.Property(e => e.ReachCount).HasDefaultValue(0);
                entity.Property(e => e.EngagementCount).HasDefaultValue(0);
                entity.Property(e => e.ConversionCount).HasDefaultValue(0);

                // Relationships
                entity.HasMany(c => c.Activities)
                    .WithOne(a => a.Campaign)
                    .HasForeignKey(a => a.CampaignId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Tasks)
                    .WithOne(t => t.Campaign)
                    .HasForeignKey(t => t.CampaignId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Notes)
                    .WithOne(n => n.Campaign)
                    .HasForeignKey(n => n.CampaignId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================================
            // JUNCTION TABLES - PRIMARY KEYS
            // ============================================================

            // LeadTask
            modelBuilder.Entity<LeadTask>(entity =>
            {
                entity.HasKey(e => new { e.LeadId, e.TaskId });
                // ? FIX: Add query filter to match parent
                entity.HasQueryFilter(e => true); // Or apply soft delete if needed
            });

            // CampaignLead
            modelBuilder.Entity<CampaignLead>(entity =>
            {
                entity.HasKey(e => new { e.CampaignId, e.LeadId });
                entity.HasQueryFilter(e => true);
            });

            // CampaignCustomer
            modelBuilder.Entity<CampaignCustomer>(entity =>
            {
                entity.HasKey(e => new { e.CampaignId, e.CustomerId });
                entity.HasQueryFilter(e => true);
            });

            // OpportunityProduct - already has query filter above
            // TeamMember - already has query filter above

            // ============================================================
            // DEPARTMENT CONFIGURATION
            // ============================================================
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).HasDatabaseName("IX_Department_Name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            // ============================================================
            // LOCAL EMPLOYEE CONFIGURATION (Additional)
            // ============================================================
            modelBuilder.Entity<LocalEmployee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).HasDatabaseName("IX_LocalEmployee_Code");
                entity.HasIndex(e => e.AppUserId).HasDatabaseName("IX_LocalEmployee_AppUserId");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_LocalEmployee_Email");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.SyncedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // ============================================================
            // ADDITIONAL FIX: EmailCampaign and SMSCampaign
            // ============================================================
            modelBuilder.Entity<EmailCampaign>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_EmailCampaign_Status");
                entity.HasIndex(e => e.ScheduledDate).HasDatabaseName("IX_EmailCampaign_ScheduledDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Content)
                    .IsRequired();

                entity.Property(e => e.OpenRate).HasPrecision(5, 2);
                entity.Property(e => e.ClickRate).HasPrecision(5, 2);

                entity.HasOne(e => e.Campaign)
                    .WithMany()
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<SMSCampaign>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_SMSCampaign_Status");
                entity.HasIndex(e => e.ScheduledDate).HasDatabaseName("IX_SMSCampaign_ScheduledDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1600);

                entity.Property(e => e.FromNumber)
                    .HasMaxLength(20);

                entity.HasOne(e => e.Campaign)
                    .WithMany()
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================================
            // SOCIAL MEDIA POST CONFIGURATION
            // ============================================================
            modelBuilder.Entity<SocialMediaPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Platform).HasDatabaseName("IX_SocialMediaPost_Platform");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_SocialMediaPost_Status");
                entity.HasIndex(e => e.ScheduledDate).HasDatabaseName("IX_SocialMediaPost_ScheduledDate");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.VideoUrl).HasMaxLength(500);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Hashtags).HasMaxLength(500);
                entity.Property(e => e.PostId).HasMaxLength(100);
                entity.Property(e => e.AnalyticsJson).HasColumnType("jsonb");

                entity.HasOne(e => e.Campaign)
                    .WithMany()
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================================
            // COMMISSION CONFIGURATION
            // ============================================================
            modelBuilder.Entity<Commission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.AgentId).HasDatabaseName("IX_Commission_AgentId");
                entity.HasIndex(e => e.TransactionId).HasDatabaseName("IX_Commission_TransactionId");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_Commission_Status");

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(e => e.Percentage)
                    .HasPrecision(5, 2);

                entity.Property(e => e.Status)
                    .HasDefaultValue(CommissionStatus.Pending)
                    .HasSentinel(CommissionStatus.Pending);

                entity.HasOne(e => e.Transaction)
                    .WithMany()
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Agent)
                    .WithMany()
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}