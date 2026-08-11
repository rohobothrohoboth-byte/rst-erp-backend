using Microsoft.EntityFrameworkCore;
using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Models.Entities;
using Svc.HRM.Performance.Persistence;

namespace Svc.HRM.Performance.Services;

public class PerformanceService : IPerformanceService
{
    private readonly PerformanceDbContext _db;

    public PerformanceService(PerformanceDbContext db)
    {
        _db = db;
    }

    // ============================================================
    // KPI
    // ============================================================
    public async Task<List<Kpi>> GetKpisAsync(CancellationToken ct = default)
        => await _db.Kpis.AsNoTracking().ToListAsync(ct);

    public async Task<Kpi?> GetKpiAsync(Guid id, CancellationToken ct = default)
        => await _db.Kpis.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Kpi> CreateKpiAsync(KpiCreateDto dto, CancellationToken ct = default)
    {
        var kpi = new Kpi
        {
            Name = dto.Name,
            Description = dto.Description,
            Metric = dto.Metric,
            Target = dto.Target,
            Weight = dto.Weight,
            Category = dto.Category
        };
        _db.Kpis.Add(kpi);
        await _db.SaveChangesAsync(ct);
        return kpi;
    }

    public async Task<Kpi?> UpdateKpiAsync(Guid id, KpiCreateDto dto, CancellationToken ct = default)
    {
        var kpi = await _db.Kpis.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (kpi is null) return null;

        kpi.Name = dto.Name;
        kpi.Description = dto.Description;
        kpi.Metric = dto.Metric;
        kpi.Target = dto.Target;
        kpi.Weight = dto.Weight;
        kpi.Category = dto.Category;
        kpi.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return kpi;
    }

    public async Task<bool> DeleteKpiAsync(Guid id, CancellationToken ct = default)
    {
        var kpi = await _db.Kpis.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (kpi is null) return false;

        kpi.IsDeleted = true;
        kpi.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ============================================================
    // GOAL
    // ============================================================
    public async Task<List<Goal>> GetGoalsAsync(CancellationToken ct = default)
        => await _db.Goals.AsNoTracking().ToListAsync(ct);

    public async Task<Goal?> GetGoalAsync(Guid id, CancellationToken ct = default)
        => await _db.Goals.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<Goal>> GetGoalsByEmployeeAsync(Guid employeeId, CancellationToken ct = default)
        => await _db.Goals.AsNoTracking().Where(e => e.EmployeeId == employeeId).ToListAsync(ct);

    public async Task<Goal> CreateGoalAsync(GoalCreateDto dto, CancellationToken ct = default)
    {
        var goal = new Goal
        {
            EmployeeId = dto.EmployeeId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Status = dto.Status,
            Progress = dto.Progress,
            KpiId = dto.KpiId
        };
        _db.Goals.Add(goal);
        await _db.SaveChangesAsync(ct);
        return goal;
    }

    public async Task<Goal?> UpdateGoalAsync(Guid id, GoalCreateDto dto, CancellationToken ct = default)
    {
        var goal = await _db.Goals.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (goal is null) return null;

        goal.EmployeeId = dto.EmployeeId;
        goal.Title = dto.Title;
        goal.Description = dto.Description;
        goal.DueDate = dto.DueDate;
        goal.Status = dto.Status;
        goal.Progress = dto.Progress;
        goal.KpiId = dto.KpiId;
        goal.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return goal;
    }

    public async Task<bool> DeleteGoalAsync(Guid id, CancellationToken ct = default)
    {
        var goal = await _db.Goals.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (goal is null) return false;

        goal.IsDeleted = true;
        goal.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ============================================================
    // PERFORMANCE REVIEW
    // ============================================================
    public async Task<List<PerformanceReview>> GetReviewsAsync(CancellationToken ct = default)
        => await _db.PerformanceReviews.AsNoTracking().ToListAsync(ct);

    public async Task<PerformanceReview?> GetReviewAsync(Guid id, CancellationToken ct = default)
        => await _db.PerformanceReviews.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<PerformanceReview>> GetReviewsByEmployeeAsync(Guid employeeId, CancellationToken ct = default)
        => await _db.PerformanceReviews.AsNoTracking().Where(e => e.EmployeeId == employeeId).ToListAsync(ct);

    public async Task<PerformanceReview> CreateReviewAsync(PerformanceReviewCreateDto dto, CancellationToken ct = default)
    {
        var review = new PerformanceReview
        {
            EmployeeId = dto.EmployeeId,
            ReviewerId = dto.ReviewerId,
            Period = dto.Period,
            OverallScore = dto.OverallScore,
            Status = dto.Status,
            Comments = dto.Comments,
            ReviewDate = dto.ReviewDate
        };
        _db.PerformanceReviews.Add(review);
        await _db.SaveChangesAsync(ct);
        return review;
    }

    public async Task<PerformanceReview?> UpdateReviewAsync(Guid id, PerformanceReviewCreateDto dto, CancellationToken ct = default)
    {
        var review = await _db.PerformanceReviews.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (review is null) return null;

        review.EmployeeId = dto.EmployeeId;
        review.ReviewerId = dto.ReviewerId;
        review.Period = dto.Period;
        review.OverallScore = dto.OverallScore;
        review.Status = dto.Status;
        review.Comments = dto.Comments;
        review.ReviewDate = dto.ReviewDate;
        review.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return review;
    }

    public async Task<bool> DeleteReviewAsync(Guid id, CancellationToken ct = default)
    {
        var review = await _db.PerformanceReviews.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (review is null) return false;

        review.IsDeleted = true;
        review.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PerformanceReview?> ChangeReviewStatusAsync(Guid id, string status, CancellationToken ct = default)
    {
        var review = await _db.PerformanceReviews.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (review is null) return null;

        review.Status = status;
        review.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return review;
    }
}
