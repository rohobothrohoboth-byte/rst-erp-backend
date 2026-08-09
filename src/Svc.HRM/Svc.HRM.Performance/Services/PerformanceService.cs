using Microsoft.EntityFrameworkCore;
using Svc.HRM.Performance.Models.DTOs;
using Svc.HRM.Performance.Models.Entities;
using Svc.HRM.Performance.Models.Enums;
using Svc.HRM.Performance.Persistence;

namespace Svc.HRM.Performance.Services;

public class PerformanceService : IPerformanceService
{
    private readonly PerformanceDbContext _db;

    public PerformanceService(PerformanceDbContext db) => _db = db;

    public async Task<List<GoalDto>> GetGoalsAsync(Guid? employeeId = null, CancellationToken ct = default)
    {
        var q = _db.Goals.AsQueryable();
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        var items = await q.OrderByDescending(x => x.DateAdd).ToListAsync(ct);
        return items.Select(MapGoal).ToList();
    }

    public async Task<GoalDto?> GetGoalAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Goals.FirstOrDefaultAsync(x => x.Id == id, ct);
        return e == null ? null : MapGoal(e);
    }

    public async Task<GoalDto> CreateGoalAsync(GoalCreateDto dto, CancellationToken ct = default)
    {
        var e = new LocalGoal
        {
            EmployeeId = dto.EmployeeId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Status = GoalStatus.NotStarted.ToString(),
            StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
            TargetDate = DateTime.SpecifyKind(dto.TargetDate, DateTimeKind.Utc),
            Weight = dto.Weight,
            ReviewId = dto.ReviewId,
            Notes = dto.Notes
        };
        _db.Goals.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapGoal(e);
    }

    public async Task<GoalDto> UpdateGoalAsync(Guid id, GoalUpdateDto dto, CancellationToken ct = default)
    {
        var e = await _db.Goals.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Goal {id} not found");
        if (dto.Title != null) e.Title = dto.Title.Trim();
        if (dto.Description != null) e.Description = dto.Description.Trim();
        if (dto.Status != null) e.Status = dto.Status;
        if (dto.TargetDate.HasValue) e.TargetDate = DateTime.SpecifyKind(dto.TargetDate.Value, DateTimeKind.Utc);
        if (dto.ProgressPercent.HasValue) e.ProgressPercent = dto.ProgressPercent.Value;
        if (dto.Weight != null) e.Weight = dto.Weight;
        if (dto.Notes != null) e.Notes = dto.Notes;
        if (e.Status == GoalStatus.Completed.ToString() && e.CompletedDate == null)
            e.CompletedDate = DateTime.UtcNow;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapGoal(e);
    }

    public async Task DeleteGoalAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Goals.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Goal {id} not found");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<KPIDto>> GetKPIsAsync(Guid? employeeId = null, CancellationToken ct = default)
    {
        var q = _db.KPIs.AsQueryable();
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        var items = await q.OrderByDescending(x => x.DateAdd).ToListAsync(ct);
        return items.Select(MapKpi).ToList();
    }

    public async Task<KPIDto?> GetKPIAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.KPIs.FirstOrDefaultAsync(x => x.Id == id, ct);
        return e == null ? null : MapKpi(e);
    }

    public async Task<KPIDto> CreateKPIAsync(KPICreateDto dto, CancellationToken ct = default)
    {
        var e = new LocalKPI
        {
            EmployeeId = dto.EmployeeId,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            MetricUnit = dto.MetricUnit,
            TargetValue = dto.TargetValue,
            ActualValue = dto.ActualValue,
            PeriodStart = DateTime.SpecifyKind(dto.PeriodStart, DateTimeKind.Utc),
            PeriodEnd = DateTime.SpecifyKind(dto.PeriodEnd, DateTimeKind.Utc),
            GoalId = dto.GoalId,
            Notes = dto.Notes
        };
        _db.KPIs.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapKpi(e);
    }

    public async Task<KPIDto> UpdateKPIAsync(Guid id, KPIUpdateDto dto, CancellationToken ct = default)
    {
        var e = await _db.KPIs.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"KPI {id} not found");
        if (dto.Name != null) e.Name = dto.Name.Trim();
        if (dto.Description != null) e.Description = dto.Description;
        if (dto.MetricUnit != null) e.MetricUnit = dto.MetricUnit;
        if (dto.TargetValue.HasValue) e.TargetValue = dto.TargetValue.Value;
        if (dto.ActualValue.HasValue) e.ActualValue = dto.ActualValue.Value;
        if (dto.PeriodEnd.HasValue) e.PeriodEnd = DateTime.SpecifyKind(dto.PeriodEnd.Value, DateTimeKind.Utc);
        if (dto.Notes != null) e.Notes = dto.Notes;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapKpi(e);
    }

    public async Task DeleteKPIAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.KPIs.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"KPI {id} not found");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<PerformanceReviewDto>> GetReviewsAsync(Guid? employeeId = null, CancellationToken ct = default)
    {
        var q = _db.Reviews.AsQueryable();
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        var items = await q.OrderByDescending(x => x.DateAdd).ToListAsync(ct);
        return items.Select(MapReview).ToList();
    }

    public async Task<PerformanceReviewDto?> GetReviewAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct);
        return e == null ? null : MapReview(e);
    }

    public async Task<PerformanceReviewDto> CreateReviewAsync(PerformanceReviewCreateDto dto, CancellationToken ct = default)
    {
        var e = new LocalPerformanceReview
        {
            EmployeeId = dto.EmployeeId,
            ReviewerId = dto.ReviewerId,
            TemplateId = dto.TemplateId,
            Title = dto.Title.Trim(),
            Status = ReviewStatus.Draft.ToString(),
            PeriodStart = DateTime.SpecifyKind(dto.PeriodStart, DateTimeKind.Utc),
            PeriodEnd = DateTime.SpecifyKind(dto.PeriodEnd, DateTimeKind.Utc),
            Summary = dto.Summary
        };
        _db.Reviews.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapReview(e);
    }

    public async Task<PerformanceReviewDto> UpdateReviewAsync(Guid id, PerformanceReviewUpdateDto dto, CancellationToken ct = default)
    {
        var e = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Review {id} not found");
        if (e.Status is not (nameof(ReviewStatus.Draft) or nameof(ReviewStatus.Rejected)))
            throw new InvalidOperationException("Only Draft/Rejected reviews can be edited.");
        if (dto.Title != null) e.Title = dto.Title.Trim();
        if (dto.ReviewerId.HasValue) e.ReviewerId = dto.ReviewerId;
        if (dto.OverallRating != null) e.OverallRating = dto.OverallRating;
        if (dto.Score.HasValue) e.Score = dto.Score;
        if (dto.Summary != null) e.Summary = dto.Summary;
        if (dto.EmployeeComments != null) e.EmployeeComments = dto.EmployeeComments;
        if (dto.ManagerComments != null) e.ManagerComments = dto.ManagerComments;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapReview(e);
    }

    public async Task<PerformanceReviewDto> SubmitReviewAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Review {id} not found");
        if (e.Status is not (nameof(ReviewStatus.Draft) or nameof(ReviewStatus.Rejected)))
            throw new InvalidOperationException("Only Draft/Rejected reviews can be submitted.");
        e.Status = ReviewStatus.Submitted.ToString();
        e.SubmittedAt = DateTime.UtcNow;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapReview(e);
    }

    public async Task<PerformanceReviewDto> ApproveReviewAsync(Guid id, ReviewDecisionDto dto, CancellationToken ct = default)
    {
        var e = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Review {id} not found");

        // Allow approving Draft/Rejected by auto-submitting first (common UI flow).
        if (e.Status == ReviewStatus.Draft.ToString() || e.Status == ReviewStatus.Rejected.ToString())
        {
            e.Status = ReviewStatus.Submitted.ToString();
            e.SubmittedAt = DateTime.UtcNow;
        }

        if (e.Status != ReviewStatus.Submitted.ToString() && e.Status != ReviewStatus.InReview.ToString())
            throw new InvalidOperationException($"Only Draft/Submitted/InReview reviews can be approved. Current status: {e.Status}.");
        e.Status = ReviewStatus.Approved.ToString();
        e.ApprovedAt = DateTime.UtcNow;
        e.ReviewerId = dto.ReviewerId ?? e.ReviewerId;
        e.ManagerComments = dto.Comments ?? e.ManagerComments;
        e.OverallRating = dto.OverallRating ?? e.OverallRating;
        e.Score = dto.Score ?? e.Score;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapReview(e);
    }

    public async Task<PerformanceReviewDto> RejectReviewAsync(Guid id, ReviewDecisionDto dto, CancellationToken ct = default)
    {
        var e = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Review {id} not found");

        if (e.Status == ReviewStatus.Draft.ToString())
        {
            e.Status = ReviewStatus.Submitted.ToString();
            e.SubmittedAt = DateTime.UtcNow;
        }

        if (e.Status != ReviewStatus.Submitted.ToString() && e.Status != ReviewStatus.InReview.ToString())
            throw new InvalidOperationException($"Only Draft/Submitted/InReview reviews can be rejected. Current status: {e.Status}.");
        e.Status = ReviewStatus.Rejected.ToString();
        e.RejectionReason = dto.RejectionReason ?? dto.Comments;
        e.ReviewerId = dto.ReviewerId ?? e.ReviewerId;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return MapReview(e);
    }

    public async Task DeleteReviewAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Review {id} not found");
        if (e.Status == ReviewStatus.Approved.ToString())
            throw new InvalidOperationException("Approved reviews cannot be deleted.");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<FeedbackDto>> GetFeedbackAsync(Guid? employeeId = null, Guid? reviewId = null, CancellationToken ct = default)
    {
        var q = _db.Feedbacks.AsQueryable();
        if (employeeId.HasValue) q = q.Where(x => x.EmployeeId == employeeId);
        if (reviewId.HasValue) q = q.Where(x => x.ReviewId == reviewId);
        var items = await q.OrderByDescending(x => x.DateAdd).ToListAsync(ct);
        return items.Select(MapFeedback).ToList();
    }

    public async Task<FeedbackDto> CreateFeedbackAsync(FeedbackCreateDto dto, CancellationToken ct = default)
    {
        var e = new LocalFeedback
        {
            EmployeeId = dto.EmployeeId,
            FromEmployeeId = dto.IsAnonymous ? null : dto.FromEmployeeId,
            ReviewId = dto.ReviewId,
            FeedbackType = dto.FeedbackType,
            Content = dto.Content.Trim(),
            Rating = dto.Rating,
            IsAnonymous = dto.IsAnonymous
        };
        _db.Feedbacks.Add(e);
        await _db.SaveChangesAsync(ct);
        return MapFeedback(e);
    }

    public async Task DeleteFeedbackAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Feedbacks.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException($"Feedback {id} not found");
        e.IsDeleted = true;
        e.DateMod = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<ReviewTemplateDto>> GetTemplatesAsync(CancellationToken ct = default) =>
        await _db.ReviewTemplates.Where(x => x.IsActive).OrderBy(x => x.Name)
            .Select(x => new ReviewTemplateDto
            {
                Id = x.Id, Name = x.Name, Description = x.Description,
                CriteriaJson = x.CriteriaJson, IsActive = x.IsActive
            }).ToListAsync(ct);

    public async Task<ReviewTemplateDto> CreateTemplateAsync(ReviewTemplateCreateDto dto, CancellationToken ct = default)
    {
        var e = new LocalReviewTemplate
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            CriteriaJson = dto.CriteriaJson,
            IsActive = dto.IsActive
        };
        _db.ReviewTemplates.Add(e);
        await _db.SaveChangesAsync(ct);
        return new ReviewTemplateDto
        {
            Id = e.Id, Name = e.Name, Description = e.Description,
            CriteriaJson = e.CriteriaJson, IsActive = e.IsActive
        };
    }

    private static GoalDto MapGoal(LocalGoal x) => new()
    {
        Id = x.Id, EmployeeId = x.EmployeeId, Title = x.Title, Description = x.Description,
        Status = x.Status, StartDate = x.StartDate, TargetDate = x.TargetDate,
        CompletedDate = x.CompletedDate, ProgressPercent = x.ProgressPercent,
        Weight = x.Weight, ReviewId = x.ReviewId, Notes = x.Notes
    };

    private static KPIDto MapKpi(LocalKPI x) => new()
    {
        Id = x.Id, EmployeeId = x.EmployeeId, Name = x.Name, Description = x.Description,
        MetricUnit = x.MetricUnit, TargetValue = x.TargetValue, ActualValue = x.ActualValue,
        PeriodStart = x.PeriodStart, PeriodEnd = x.PeriodEnd, GoalId = x.GoalId, Notes = x.Notes
    };

    private static PerformanceReviewDto MapReview(LocalPerformanceReview x) => new()
    {
        Id = x.Id, EmployeeId = x.EmployeeId, ReviewerId = x.ReviewerId, TemplateId = x.TemplateId,
        Title = x.Title, Status = x.Status, PeriodStart = x.PeriodStart, PeriodEnd = x.PeriodEnd,
        SubmittedAt = x.SubmittedAt, ApprovedAt = x.ApprovedAt, OverallRating = x.OverallRating,
        Score = x.Score, Summary = x.Summary, EmployeeComments = x.EmployeeComments,
        ManagerComments = x.ManagerComments, RejectionReason = x.RejectionReason
    };

    private static FeedbackDto MapFeedback(LocalFeedback x) => new()
    {
        Id = x.Id, EmployeeId = x.EmployeeId, FromEmployeeId = x.FromEmployeeId, ReviewId = x.ReviewId,
        FeedbackType = x.FeedbackType, Content = x.Content, Rating = x.Rating,
        IsAnonymous = x.IsAnonymous, DateAdd = x.DateAdd
    };
}
