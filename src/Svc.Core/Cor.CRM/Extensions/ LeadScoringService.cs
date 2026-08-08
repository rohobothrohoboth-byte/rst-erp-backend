using Cor.CRM.Interfaces;
using Cor.CRM.Models.Entities;
using Cor.CRM.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;
using Cor.CRM.Services;
namespace Cor.CRM.Extensions;

public class LeadScoringService : ILeadScoringService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogService _logService;

    public LeadScoringService(IServiceScopeFactory serviceScopeFactory, ILogService logService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logService = logService;
    }

    public async Task<int> CalculateLeadScoreAsync(Guid leadId, CancellationToken ct = default)
    {
        try
        {
            // ✅ Use a fresh scope - NEVER use the UnitOfWork from the transaction
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

            var lead = await context.Leads
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == leadId, ct);

            if (lead == null)
            {
                _logService.LogWarning("Lead not found for scoring: {LeadId}", leadId);
                return 0;
            }

            var score = 0;

            // 1. Demographic Scoring
            if (!string.IsNullOrEmpty(lead.CompanyName)) score += 10;
            if (!string.IsNullOrEmpty(lead.Title)) score += 5;
            if (lead.Industry.HasValue) score += 10;
            if (lead.Industry == Industry.Government) score += 15;
            if (lead.Industry == Industry.RealEstate) score += 10;

            // 2. Budget Scoring
            if (lead.Budget.HasValue)
            {
                if (lead.Budget.Value > 100000) score += 25;
                else if (lead.Budget.Value > 50000) score += 15;
                else if (lead.Budget.Value > 10000) score += 10;
                else score += 5;
            }

            // 3. Estimated Value Scoring
            if (lead.EstimatedValue.HasValue)
            {
                if (lead.EstimatedValue.Value > 500000) score += 30;
                else if (lead.EstimatedValue.Value > 100000) score += 20;
                else if (lead.EstimatedValue.Value > 50000) score += 10;
                else score += 5;
            }

            // 4. Engagement Scoring
            score += Math.Min(lead.ContactCount * 2, 20);
            var activityCount = lead.Activities?.Count ?? 0;
            score += Math.Min(activityCount, 15);

            // 5. Status Scoring
            switch (lead.Status)
            {
                case LeadStatus.Qualified: score += 20; break;
                case LeadStatus.Proposal: score += 25; break;
                case LeadStatus.Negotiation: score += 30; break;
                case LeadStatus.Contacted: score += 10; break;
                case LeadStatus.New: score += 5; break;
                default: break;
            }

            // 6. Priority Scoring
            switch (lead.Priority)
            {
                case LeadPriority.Urgent: score += 20; break;
                case LeadPriority.High: score += 15; break;
                case LeadPriority.Medium: score += 10; break;
                case LeadPriority.Low: score += 5; break;
            }

            // 7. Source Scoring
            switch (lead.Source)
            {
                case LeadSource.Referral: score += 15; break;
                case LeadSource.Website: score += 10; break;
                case LeadSource.SocialMedia: score += 5; break;
                case LeadSource.Event: score += 10; break;
                case LeadSource.ColdCall: score += 3; break;
                default: score += 5; break;
            }

            // 8. Tag Scoring
            if (!string.IsNullOrEmpty(lead.Tags))
            {
                var tags = lead.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var tag in tags)
                {
                    var tagLower = tag.Trim().ToLower();
                    if (tagLower == "vip" || tagLower == "hot") score += 15;
                    else if (tagLower == "high priority" || tagLower == "decision maker") score += 10;
                    else if (tagLower == "interested" || tagLower == "budget approved") score += 8;
                }
            }

            // 9. Time Factor
            var daysSinceCreation = (DateTime.UtcNow - lead.CreatedAt).Days;
            if (daysSinceCreation <= 7) score += 10;
            else if (daysSinceCreation <= 30) score += 5;

            // 10. Expected Close Date
            if (lead.ExpectedCloseDate.HasValue)
            {
                var daysUntilClose = (lead.ExpectedCloseDate.Value - DateTime.UtcNow).Days;
                if (daysUntilClose <= 7) score += 15;
                else if (daysUntilClose <= 30) score += 10;
                else if (daysUntilClose <= 60) score += 5;
            }

            // Cap score at 100
            score = Math.Min(score, 100);

            // ✅ Update using the fresh context, NOT the UnitOfWork
            var leadToUpdate = await context.Leads.FirstOrDefaultAsync(x => x.Id == leadId, ct);
            if (leadToUpdate != null)
            {
                leadToUpdate.Score = score;
                leadToUpdate.EngagementScore = Math.Min(leadToUpdate.EngagementScore + 5, 100);
                await context.SaveChangesAsync(ct);
            }

            _logService.LogInformation("Lead score calculated: {LeadId} = {Score}", leadId, score);
            return score;
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, "Failed to calculate lead score: {LeadId}", leadId);
            return 0;
        }
    }

    public async Task UpdateLeadScoreAsync(Guid leadId, CancellationToken ct = default)
    {
        await CalculateLeadScoreAsync(leadId, ct);
    }

    public async Task CalculateScoresForAllLeadsAsync(CancellationToken ct = default)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

            var leads = await context.Leads
                .Where(x => !x.IsDeleted && !x.IsConverted)
                .ToListAsync(ct);

            _logService.LogInformation("Calculating scores for {Count} leads", leads.Count);

            foreach (var lead in leads)
            {
                await CalculateLeadScoreAsync(lead.Id, ct);
            }

            _logService.LogInformation("Scores calculated for all leads");
        }
        catch (Exception ex)
        {
            _logService.LogError(ex, "Failed to calculate scores for all leads");
            throw;
        }
    }
}