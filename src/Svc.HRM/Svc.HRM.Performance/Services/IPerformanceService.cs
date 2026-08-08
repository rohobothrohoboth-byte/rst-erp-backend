using Svc.HRM.Performance.Models.DTOs;

namespace Svc.HRM.Performance.Services;

public interface IPerformanceService
{
    // Goals
    Task<List<GoalDto>> GetGoalsAsync(Guid? employeeId = null, CancellationToken ct = default);
    Task<GoalDto?> GetGoalAsync(Guid id, CancellationToken ct = default);
    Task<GoalDto> CreateGoalAsync(GoalCreateDto dto, CancellationToken ct = default);
    Task<GoalDto> UpdateGoalAsync(Guid id, GoalUpdateDto dto, CancellationToken ct = default);
    Task DeleteGoalAsync(Guid id, CancellationToken ct = default);

    // KPIs
    Task<List<KPIDto>> GetKPIsAsync(Guid? employeeId = null, CancellationToken ct = default);
    Task<KPIDto?> GetKPIAsync(Guid id, CancellationToken ct = default);
    Task<KPIDto> CreateKPIAsync(KPICreateDto dto, CancellationToken ct = default);
    Task<KPIDto> UpdateKPIAsync(Guid id, KPIUpdateDto dto, CancellationToken ct = default);
    Task DeleteKPIAsync(Guid id, CancellationToken ct = default);

    // Reviews
    Task<List<PerformanceReviewDto>> GetReviewsAsync(Guid? employeeId = null, CancellationToken ct = default);
    Task<PerformanceReviewDto?> GetReviewAsync(Guid id, CancellationToken ct = default);
    Task<PerformanceReviewDto> CreateReviewAsync(PerformanceReviewCreateDto dto, CancellationToken ct = default);
    Task<PerformanceReviewDto> UpdateReviewAsync(Guid id, PerformanceReviewUpdateDto dto, CancellationToken ct = default);
    Task<PerformanceReviewDto> SubmitReviewAsync(Guid id, CancellationToken ct = default);
    Task<PerformanceReviewDto> ApproveReviewAsync(Guid id, ReviewDecisionDto dto, CancellationToken ct = default);
    Task<PerformanceReviewDto> RejectReviewAsync(Guid id, ReviewDecisionDto dto, CancellationToken ct = default);
    Task DeleteReviewAsync(Guid id, CancellationToken ct = default);

    // Feedback
    Task<List<FeedbackDto>> GetFeedbackAsync(Guid? employeeId = null, Guid? reviewId = null, CancellationToken ct = default);
    Task<FeedbackDto> CreateFeedbackAsync(FeedbackCreateDto dto, CancellationToken ct = default);
    Task DeleteFeedbackAsync(Guid id, CancellationToken ct = default);

    // Templates
    Task<List<ReviewTemplateDto>> GetTemplatesAsync(CancellationToken ct = default);
    Task<ReviewTemplateDto> CreateTemplateAsync(ReviewTemplateCreateDto dto, CancellationToken ct = default);
}
