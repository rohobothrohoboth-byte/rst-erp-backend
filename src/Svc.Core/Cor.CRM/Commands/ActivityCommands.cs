// Cor.CRM/Commands/ActivityCommands.cs
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class ActivityAddCmd : IRequest<ActivityDto>
{
    public CreateActivityDto Dto { get; set; } = default!;
}

public class ActivityUpdateCmd : IRequest<ActivityDto>
{
    public Guid Id { get; set; }
    public UpdateActivityDto Dto { get; set; } = default!;
}

public class ActivityUpdateStatusCmd : IRequest<ActivityDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ActivityCompleteCmd : IRequest<ActivityDto>
{
    public Guid Id { get; set; }
}

public class ActivityDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// ACTIVITY ADD HANDLER
// ============================================================

public class ActivityAddHandler : IRequestHandler<ActivityAddCmd, ActivityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public ActivityAddHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<ActivityDto> Handle(ActivityAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            var activity = new Activity
            {
                Id = Guid.CreateVersion7(),
                Title = request.Dto.Title,
                Description = request.Dto.Description,
                Type = Enum.Parse<ActivityType>(request.Dto.Type),
                Status = !string.IsNullOrEmpty(request.Dto.Status)
                    ? Enum.Parse<ActivityStatus>(request.Dto.Status)
                    : ActivityStatus.Scheduled,
                StartDateTime = request.Dto.StartDateTime.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.StartDateTime.Value)
                    : null,
                EndDateTime = request.Dto.EndDateTime.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.EndDateTime.Value)
                    : null,
                LeadId = request.Dto.LeadId,
                CustomerId = request.Dto.CustomerId,
                OpportunityId = request.Dto.OpportunityId,
                AssignedToUserId = request.Dto.AssignedToUserId,
                Location = request.Dto.Location,
                IsAllDay = request.Dto.IsAllDay,
                CreatedByUserId = userId,
                CreatedByUserName = userName,
                UpdatedByUserId = userId,
                UpdatedByUserName = userName,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            // Calculate duration if start and end are provided
            if (activity.StartDateTime.HasValue && activity.EndDateTime.HasValue)
            {
                activity.DurationMinutes = (int)(activity.EndDateTime.Value - activity.StartDateTime.Value).TotalMinutes;
            }

            await _uow.Add(activity, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Activity created successfully: {Title}", activity.Title);

            return MapToDto(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to create activity");
            await _uow.Rollback(ct);
            throw;
        }
    }

    private ActivityDto MapToDto(Activity activity)
    {
        return new ActivityDto
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            Type = activity.Type.ToString(),
            Status = activity.Status.ToString(),
            StartDateTime = activity.StartDateTime,
            EndDateTime = activity.EndDateTime,
            DurationMinutes = activity.DurationMinutes,
            LeadId = activity.LeadId,
            CustomerId = activity.CustomerId,
            OpportunityId = activity.OpportunityId,
            AssignedToUserId = activity.AssignedToUserId,
            AssignedToUserName = activity.AssignedToUserName,
            Location = activity.Location,
            IsAllDay = activity.IsAllDay,
            Outcome = activity.Outcome,
            CompletedAt = activity.CompletedAt,
            CreatedAt = activity.CreatedAt,
            UpdatedAt = activity.UpdatedAt,
            CreatedByUserId = activity.CreatedByUserId,
            CreatedByUserName = activity.CreatedByUserName,
            UpdatedByUserId = activity.UpdatedByUserId,
            UpdatedByUserName = activity.UpdatedByUserName
        };
    }
}

// ============================================================
// ACTIVITY UPDATE HANDLER
// ============================================================

public class ActivityUpdateHandler : IRequestHandler<ActivityUpdateCmd, ActivityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public ActivityUpdateHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<ActivityDto> Handle(ActivityUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var activity = await _uow.Set<Activity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (activity == null)
                throw new DomainException($"Activity with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            if (!string.IsNullOrEmpty(request.Dto.Title))
                activity.Title = request.Dto.Title;

            if (request.Dto.Description != null)
                activity.Description = request.Dto.Description;

            if (!string.IsNullOrEmpty(request.Dto.Type))
                activity.Type = Enum.Parse<ActivityType>(request.Dto.Type);

            if (!string.IsNullOrEmpty(request.Dto.Status))
                activity.Status = Enum.Parse<ActivityStatus>(request.Dto.Status);

            if (request.Dto.StartDateTime.HasValue)
                activity.StartDateTime = DateTimeHelper.EnsureUtc(request.Dto.StartDateTime.Value);

            if (request.Dto.EndDateTime.HasValue)
                activity.EndDateTime = DateTimeHelper.EnsureUtc(request.Dto.EndDateTime.Value);

            // Recalculate duration
            if (activity.StartDateTime.HasValue && activity.EndDateTime.HasValue)
            {
                activity.DurationMinutes = (int)(activity.EndDateTime.Value - activity.StartDateTime.Value).TotalMinutes;
            }

            if (request.Dto.LeadId.HasValue)
                activity.LeadId = request.Dto.LeadId;

            if (request.Dto.CustomerId.HasValue)
                activity.CustomerId = request.Dto.CustomerId;

            if (request.Dto.OpportunityId.HasValue)
                activity.OpportunityId = request.Dto.OpportunityId;

            if (request.Dto.AssignedToUserId.HasValue)
                activity.AssignedToUserId = request.Dto.AssignedToUserId;

            if (request.Dto.Location != null)
                activity.Location = request.Dto.Location;

            if (request.Dto.IsAllDay.HasValue)
                activity.IsAllDay = request.Dto.IsAllDay.Value;

            activity.UpdatedByUserId = userId;
            activity.UpdatedByUserName = userName;
            activity.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(activity);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Activity updated: {Title}", activity.Title);

            return new ActivityDto
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Type = activity.Type.ToString(),
                Status = activity.Status.ToString(),
                StartDateTime = activity.StartDateTime,
                EndDateTime = activity.EndDateTime,
                DurationMinutes = activity.DurationMinutes,
                LeadId = activity.LeadId,
                CustomerId = activity.CustomerId,
                OpportunityId = activity.OpportunityId,
                AssignedToUserId = activity.AssignedToUserId,
                AssignedToUserName = activity.AssignedToUserName,
                Location = activity.Location,
                IsAllDay = activity.IsAllDay,
                Outcome = activity.Outcome,
                CompletedAt = activity.CompletedAt,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt,
                CreatedByUserId = activity.CreatedByUserId,
                CreatedByUserName = activity.CreatedByUserName,
                UpdatedByUserId = activity.UpdatedByUserId,
                UpdatedByUserName = activity.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to update activity: {ActivityId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ACTIVITY UPDATE STATUS HANDLER
// ============================================================

public class ActivityUpdateStatusHandler : IRequestHandler<ActivityUpdateStatusCmd, ActivityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public ActivityUpdateStatusHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<ActivityDto> Handle(ActivityUpdateStatusCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var activity = await _uow.Set<Activity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (activity == null)
                throw new DomainException($"Activity with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            var newStatus = Enum.Parse<ActivityStatus>(request.Status);
            activity.Status = newStatus;

            if (newStatus == ActivityStatus.Completed)
            {
                activity.CompletedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                activity.Outcome = "Completed";
            }

            activity.UpdatedByUserId = userId;
            activity.UpdatedByUserName = userName;
            activity.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(activity);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Activity status updated: {Title} -> {Status}", activity.Title, request.Status);

            return new ActivityDto
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Type = activity.Type.ToString(),
                Status = activity.Status.ToString(),
                StartDateTime = activity.StartDateTime,
                EndDateTime = activity.EndDateTime,
                DurationMinutes = activity.DurationMinutes,
                LeadId = activity.LeadId,
                CustomerId = activity.CustomerId,
                OpportunityId = activity.OpportunityId,
                AssignedToUserId = activity.AssignedToUserId,
                AssignedToUserName = activity.AssignedToUserName,
                Location = activity.Location,
                IsAllDay = activity.IsAllDay,
                Outcome = activity.Outcome,
                CompletedAt = activity.CompletedAt,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt,
                CreatedByUserId = activity.CreatedByUserId,
                CreatedByUserName = activity.CreatedByUserName,
                UpdatedByUserId = activity.UpdatedByUserId,
                UpdatedByUserName = activity.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to update activity status: {ActivityId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ACTIVITY COMPLETE HANDLER
// ============================================================

public class ActivityCompleteHandler : IRequestHandler<ActivityCompleteCmd, ActivityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public ActivityCompleteHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<ActivityDto> Handle(ActivityCompleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var activity = await _uow.Set<Activity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (activity == null)
                throw new DomainException($"Activity with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            activity.Status = ActivityStatus.Completed;
            activity.CompletedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            activity.Outcome = "Completed";
            activity.UpdatedByUserId = userId;
            activity.UpdatedByUserName = userName;
            activity.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(activity);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Activity completed: {Title}", activity.Title);

            return new ActivityDto
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Type = activity.Type.ToString(),
                Status = activity.Status.ToString(),
                StartDateTime = activity.StartDateTime,
                EndDateTime = activity.EndDateTime,
                DurationMinutes = activity.DurationMinutes,
                LeadId = activity.LeadId,
                CustomerId = activity.CustomerId,
                OpportunityId = activity.OpportunityId,
                AssignedToUserId = activity.AssignedToUserId,
                AssignedToUserName = activity.AssignedToUserName,
                Location = activity.Location,
                IsAllDay = activity.IsAllDay,
                Outcome = activity.Outcome,
                CompletedAt = activity.CompletedAt,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt,
                CreatedByUserId = activity.CreatedByUserId,
                CreatedByUserName = activity.CreatedByUserName,
                UpdatedByUserId = activity.UpdatedByUserId,
                UpdatedByUserName = activity.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to complete activity: {ActivityId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ACTIVITY DELETE HANDLER
// ============================================================

public class ActivityDeleteHandler : IRequestHandler<ActivityDeleteCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public ActivityDeleteHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task Handle(ActivityDeleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var activity = await _uow.Set<Activity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (activity == null)
                throw new DomainException($"Activity with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            activity.IsDeleted = true;
            activity.UpdatedByUserId = userId;
            activity.UpdatedByUserName = userName;
            activity.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(activity);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Activity deleted: {Title}", activity.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to delete activity: {ActivityId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}