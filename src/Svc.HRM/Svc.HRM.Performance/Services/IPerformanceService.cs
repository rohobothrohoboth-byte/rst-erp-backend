using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Models.Entities;

namespace Svc.HRM.Performance.Services;

public interface IPerformanceService
{
    // KPI
    Task<List<Kpi>> GetKpisAsync(CancellationToken ct = default);
    Task<Kpi?> GetKpiAsync(Guid id, CancellationToken ct = default);
    Task<Kpi> CreateKpiAsync(KpiCreateDto dto, CancellationToken ct = default);
    Task<Kpi?> UpdateKpiAsync(Guid id, KpiCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteKpiAsync(Guid id, CancellationToken ct = default);

    // Goal
    Task<List<Goal>> GetGoalsAsync(CancellationToken ct = default);
    Task<Goal?> GetGoalAsync(Guid id, CancellationToken ct = default);
    Task<List<Goal>> GetGoalsByEmployeeAsync(Guid employeeId, CancellationToken ct = default);
    Task<Goal> CreateGoalAsync(GoalCreateDto dto, CancellationToken ct = default);
    Task<Goal?> UpdateGoalAsync(Guid id, GoalCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteGoalAsync(Guid id, CancellationToken ct = default);

    // Performance Review
    Task<List<PerformanceReview>> GetReviewsAsync(CancellationToken ct = default);
    Task<PerformanceReview?> GetReviewAsync(Guid id, CancellationToken ct = default);
    Task<List<PerformanceReview>> GetReviewsByEmployeeAsync(Guid employeeId, CancellationToken ct = default);
    Task<PerformanceReview> CreateReviewAsync(PerformanceReviewCreateDto dto, CancellationToken ct = default);
    Task<PerformanceReview?> UpdateReviewAsync(Guid id, PerformanceReviewCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteReviewAsync(Guid id, CancellationToken ct = default);
    Task<PerformanceReview?> ChangeReviewStatusAsync(Guid id, string status, CancellationToken ct = default);
}
