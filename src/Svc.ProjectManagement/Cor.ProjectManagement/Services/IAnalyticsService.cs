// Services/IAnalyticsService.cs
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Services
{
    public interface IAnalyticsService
    {
        Task<ProjectDashboardDto> GetDashboardAsync(Guid? userId);
        Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId);
        Task<BudgetUtilizationDto> GetBudgetUtilizationAsync(Guid projectId, DateTime fromDate, DateTime toDate);
        Task<RiskHeatmapDto> GetRiskHeatmapAsync(Guid projectId);
    }

    public class AnalyticsService : IAnalyticsService
    {
        public Task<ProjectDashboardDto> GetDashboardAsync(Guid? userId) => throw new NotImplementedException();
        public Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId) => throw new NotImplementedException();
        public Task<BudgetUtilizationDto> GetBudgetUtilizationAsync(Guid projectId, DateTime fromDate, DateTime toDate) => throw new NotImplementedException();
        public Task<RiskHeatmapDto> GetRiskHeatmapAsync(Guid projectId) => throw new NotImplementedException();
    }
}