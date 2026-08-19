// Services/IReportService.cs
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Services
{
    public interface IReportService
    {
        Task<BudgetSummaryDto> GetBudgetSummaryAsync(Guid projectId);
        Task<RiskSummaryDto> GetRiskSummaryAsync(Guid projectId);
        Task<IssueSummaryDto> GetIssueSummaryAsync(Guid projectId);
        Task<ChangeSummaryDto> GetChangeSummaryAsync(Guid projectId);
        Task<AuditLogSummaryDto> GetAuditLogSummaryAsync(Guid projectId, DateTime? fromDate, DateTime? toDate);
    }

    public class ReportService : IReportService
    {
        public Task<BudgetSummaryDto> GetBudgetSummaryAsync(Guid projectId) => throw new NotImplementedException();
        public Task<RiskSummaryDto> GetRiskSummaryAsync(Guid projectId) => throw new NotImplementedException();
        public Task<IssueSummaryDto> GetIssueSummaryAsync(Guid projectId) => throw new NotImplementedException();
        public Task<ChangeSummaryDto> GetChangeSummaryAsync(Guid projectId) => throw new NotImplementedException();
        public Task<AuditLogSummaryDto> GetAuditLogSummaryAsync(Guid projectId, DateTime? fromDate, DateTime? toDate) => throw new NotImplementedException();
    }
}