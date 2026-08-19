// Handlers/TimesheetHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.TimesheetCommands;
using Cor.ProjectManagement.Queries.TimesheetQueries;

namespace Cor.ProjectManagement.Handlers
{
    // ============ COMMAND HANDLERS ============

    public class CreateTimesheetCommandHandler : IRequestHandler<CreateTimesheetCommand, TimesheetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTimesheetCommandHandler> _logger;

        public CreateTimesheetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateTimesheetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TimesheetDto> Handle(CreateTimesheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate project exists
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                // Validate task exists if provided
                if (request.TaskId.HasValue)
                {
                    var task = await _context.ProjectTasks
                        .FirstOrDefaultAsync(t => t.Id == request.TaskId.Value &&
                                                   t.ProjectId == request.ProjectId &&
                                                   !t.IsDeleted, cancellationToken);

                    if (task == null)
                        throw new Exception($"Task with ID {request.TaskId} not found in project");
                }

                // Check for duplicate entry (same user, project, date)
                var duplicate = await _context.Timesheets
                    .AnyAsync(t => t.UserId == request.UserId &&
                                   t.ProjectId == request.ProjectId &&
                                   t.Date.Date == request.Date.Date &&
                                   !t.IsDeleted, cancellationToken);

                if (duplicate)
                    throw new Exception("Timesheet entry already exists for this user, project, and date");

                var timesheet = new Timesheet
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    UserName = request.UserName ?? string.Empty,
                    ProjectId = request.ProjectId,
                    TaskId = request.TaskId,
                    Date = request.Date,
                    HoursWorked = request.HoursWorked,
                    OvertimeHours = request.OvertimeHours,
                    Description = request.Description,
                    Status = TimesheetStatus.Draft,
                    HourlyRate = request.HourlyRate,
                    TotalAmount = request.HoursWorked * request.HourlyRate,
                    IsBillable = request.IsBillable,
                    TaskName = request.TaskId.HasValue ?
                        await _context.ProjectTasks
                            .Where(t => t.Id == request.TaskId)
                            .Select(t => t.Title)
                            .FirstOrDefaultAsync(cancellationToken) : null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.Timesheets.AddAsync(timesheet, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Timesheet created successfully with ID: {TimesheetId}", timesheet.Id);
                return _mapper.Map<TimesheetDto>(timesheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating timesheet");
                throw;
            }
        }
    }

    public class UpdateTimesheetCommandHandler : IRequestHandler<UpdateTimesheetCommand, TimesheetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTimesheetCommandHandler> _logger;

        public UpdateTimesheetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateTimesheetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TimesheetDto> Handle(UpdateTimesheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheet = await _context.Timesheets
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

                if (timesheet == null)
                    throw new Exception($"Timesheet with ID {request.Id} not found");

                // Cannot modify approved or paid timesheets
                if (timesheet.Status == TimesheetStatus.Approved || timesheet.Status == TimesheetStatus.Paid)
                    throw new Exception("Cannot modify an approved or paid timesheet");

                // Update only provided fields
                if (request.HoursWorked.HasValue)
                    timesheet.HoursWorked = request.HoursWorked.Value;

                if (request.OvertimeHours.HasValue)
                    timesheet.OvertimeHours = request.OvertimeHours.Value;

                if (!string.IsNullOrEmpty(request.Description))
                    timesheet.Description = request.Description;

                if (request.Date.HasValue)
                    timesheet.Date = request.Date.Value;

                if (request.TaskId.HasValue)
                {
                    timesheet.TaskId = request.TaskId.Value;
                    timesheet.TaskName = await _context.ProjectTasks
                        .Where(t => t.Id == request.TaskId)
                        .Select(t => t.Title)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (request.IsBillable.HasValue)
                    timesheet.IsBillable = request.IsBillable.Value;

                // Recalculate total amount
                timesheet.TotalAmount = timesheet.HoursWorked * timesheet.HourlyRate;

                timesheet.UpdatedAt = DateTime.UtcNow;
                timesheet.UpdatedBy = request.UpdatedBy ?? "System";
                timesheet.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Timesheet updated successfully with ID: {TimesheetId}", timesheet.Id);
                return _mapper.Map<TimesheetDto>(timesheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating timesheet with ID: {TimesheetId}", request.Id);
                throw;
            }
        }
    }

    public class DeleteTimesheetCommandHandler : IRequestHandler<DeleteTimesheetCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteTimesheetCommandHandler> _logger;

        public DeleteTimesheetCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteTimesheetCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteTimesheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheet = await _context.Timesheets
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

                if (timesheet == null)
                    throw new Exception($"Timesheet with ID {request.Id} not found");

                // Cannot delete approved or paid timesheets
                if (timesheet.Status == TimesheetStatus.Approved || timesheet.Status == TimesheetStatus.Paid)
                    throw new Exception("Cannot delete an approved or paid timesheet");

                timesheet.IsDeleted = true;
                timesheet.DeletedAt = DateTime.UtcNow;
                timesheet.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Timesheet deleted successfully with ID: {TimesheetId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting timesheet with ID: {TimesheetId}", request.Id);
                throw;
            }
        }
    }

    public class SubmitTimesheetCommandHandler : IRequestHandler<SubmitTimesheetCommand, List<TimesheetDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SubmitTimesheetCommandHandler> _logger;

        public SubmitTimesheetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<SubmitTimesheetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<TimesheetDto>> Handle(SubmitTimesheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheets = await _context.Timesheets
                    .Where(t => request.TimesheetIds.Contains(t.Id) && !t.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!timesheets.Any())
                    throw new Exception("No timesheets found to submit");

                // Verify all timesheets belong to same user and are in Draft status
                var userId = timesheets.First().UserId;
                foreach (var timesheet in timesheets)
                {
                    if (timesheet.UserId != userId)
                        throw new Exception("All timesheets must belong to the same user");

                    if (timesheet.Status != TimesheetStatus.Draft)
                        throw new Exception($"Timesheet {timesheet.Id} is already submitted or approved");
                }

                foreach (var timesheet in timesheets)
                {
                    timesheet.Status = TimesheetStatus.Submitted;
                    timesheet.SubmittedAt = DateTime.UtcNow;
                    timesheet.SubmittedById = userId;
                    timesheet.UpdatedAt = DateTime.UtcNow;
                    timesheet.UpdatedBy = request.SubmittedBy ?? "System";
                    timesheet.Version += 1;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Timesheets submitted successfully. Count: {Count}", timesheets.Count);
                return _mapper.Map<List<TimesheetDto>>(timesheets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting timesheets");
                throw;
            }
        }
    }

    public class ApproveTimesheetCommandHandler : IRequestHandler<ApproveTimesheetCommand, List<TimesheetDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ApproveTimesheetCommandHandler> _logger;

        public ApproveTimesheetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ApproveTimesheetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<TimesheetDto>> Handle(ApproveTimesheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheets = await _context.Timesheets
                    .Where(t => request.TimesheetIds.Contains(t.Id) && !t.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!timesheets.Any())
                    throw new Exception("No timesheets found to approve");

                foreach (var timesheet in timesheets)
                {
                    if (timesheet.Status != TimesheetStatus.Submitted)
                        throw new Exception($"Timesheet {timesheet.Id} is not in submitted status");
                }

                foreach (var timesheet in timesheets)
                {
                    timesheet.Status = TimesheetStatus.Approved;
                    timesheet.ApprovedAt = DateTime.UtcNow;
                    timesheet.ApprovedById = Guid.TryParse(request.ApprovedBy, out var id) ? id : null;
                    timesheet.ApprovalNote = request.Notes ?? string.Empty;
                    timesheet.UpdatedAt = DateTime.UtcNow;
                    timesheet.UpdatedBy = request.ApprovedBy ?? "System";
                    timesheet.Version += 1;
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Update project actual cost with approved timesheets
                await UpdateProjectCosts(timesheets, cancellationToken);

                _logger.LogInformation("Timesheets approved successfully. Count: {Count}", timesheets.Count);
                return _mapper.Map<List<TimesheetDto>>(timesheets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving timesheets");
                throw;
            }
        }

        private async Task UpdateProjectCosts(List<Timesheet> timesheets, CancellationToken cancellationToken)
        {
            var projectIds = timesheets.Select(t => t.ProjectId).Distinct();
            foreach (var projectId in projectIds)
            {
                var totalCost = await _context.Timesheets
                    .Where(t => t.ProjectId == projectId &&
                               t.Status == TimesheetStatus.Approved &&
                               !t.IsDeleted)
                    .SumAsync(t => t.TotalAmount, cancellationToken);

                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

                if (project != null)
                {
                    project.ActualCost = totalCost;
                    project.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public class RejectTimesheetCommandHandler : IRequestHandler<RejectTimesheetCommand, List<TimesheetDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RejectTimesheetCommandHandler> _logger;

        public RejectTimesheetCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<RejectTimesheetCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<TimesheetDto>> Handle(RejectTimesheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheets = await _context.Timesheets
                    .Where(t => request.TimesheetIds.Contains(t.Id) && !t.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!timesheets.Any())
                    throw new Exception("No timesheets found to reject");

                foreach (var timesheet in timesheets)
                {
                    if (timesheet.Status != TimesheetStatus.Submitted)
                        throw new Exception($"Timesheet {timesheet.Id} is not in submitted status");
                }

                foreach (var timesheet in timesheets)
                {
                    timesheet.Status = TimesheetStatus.Rejected;
                    timesheet.RejectedAt = DateTime.UtcNow;
                    timesheet.RejectedById = Guid.TryParse(request.RejectedBy, out var id) ? id : null;
                    timesheet.RejectionReason = request.Reason;
                    timesheet.UpdatedAt = DateTime.UtcNow;
                    timesheet.UpdatedBy = request.RejectedBy ?? "System";
                    timesheet.Version += 1;
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Timesheets rejected successfully. Count: {Count}", timesheets.Count);
                return _mapper.Map<List<TimesheetDto>>(timesheets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting timesheets");
                throw;
            }
        }
    }

    // ============ QUERY HANDLERS ============

    public class GetTimesheetByIdQueryHandler : IRequestHandler<GetTimesheetByIdQuery, TimesheetDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTimesheetByIdQueryHandler> _logger;

        public GetTimesheetByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTimesheetByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TimesheetDto> Handle(GetTimesheetByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheet = await _context.Timesheets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

                if (timesheet == null)
                    throw new Exception($"Timesheet with ID {request.Id} not found");

                return _mapper.Map<TimesheetDto>(timesheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting timesheet with ID: {TimesheetId}", request.Id);
                throw;
            }
        }
    }

    public class GetTimesheetsByUserQueryHandler : IRequestHandler<GetTimesheetsByUserQuery, PaginatedResponse<TimesheetDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTimesheetsByUserQueryHandler> _logger;

        public GetTimesheetsByUserQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTimesheetsByUserQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<TimesheetDto>> Handle(GetTimesheetsByUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Timesheets
                    .AsNoTracking()
                    .Where(t => t.UserId == request.UserId && !t.IsDeleted);

                if (request.FromDate.HasValue)
                    query = query.Where(t => t.Date >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(t => t.Date <= request.ToDate.Value);

                if (request.Status.HasValue)
                    query = query.Where(t => t.Status == request.Status.Value);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .OrderByDescending(t => t.Date)
                    .Select(t => _mapper.Map<TimesheetDto>(t))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<TimesheetDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting timesheets for user ID: {UserId}", request.UserId);
                throw;
            }
        }
    }

    public class GetTimesheetsByProjectQueryHandler : IRequestHandler<GetTimesheetsByProjectQuery, PaginatedResponse<TimesheetDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTimesheetsByProjectQueryHandler> _logger;

        public GetTimesheetsByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTimesheetsByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<TimesheetDto>> Handle(GetTimesheetsByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Timesheets
                    .AsNoTracking()
                    .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted);

                if (request.FromDate.HasValue)
                    query = query.Where(t => t.Date >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(t => t.Date <= request.ToDate.Value);

                if (request.UserId.HasValue)
                    query = query.Where(t => t.UserId == request.UserId.Value);

                if (request.Status.HasValue)
                    query = query.Where(t => t.Status == request.Status.Value);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .OrderByDescending(t => t.Date)
                    .Select(t => _mapper.Map<TimesheetDto>(t))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<TimesheetDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting timesheets for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetTimesheetSummaryQueryHandler : IRequestHandler<GetTimesheetSummaryQuery, TimesheetSummaryDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetTimesheetSummaryQueryHandler> _logger;

        public GetTimesheetSummaryQueryHandler(
            ProjectDbContext context,
            ILogger<GetTimesheetSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TimesheetSummaryDto> Handle(GetTimesheetSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var timesheets = await _context.Timesheets
                    .AsNoTracking()
                    .Where(t => t.UserId == request.UserId &&
                               t.Date >= request.FromDate &&
                               t.Date <= request.ToDate &&
                               !t.IsDeleted)
                    .ToListAsync(cancellationToken);

                var totalHours = timesheets.Sum(t => t.HoursWorked);
                var overtimeHours = timesheets.Sum(t => t.OvertimeHours);
                var totalAmount = timesheets.Sum(t => t.TotalAmount);

                var dailySummary = timesheets
                    .GroupBy(t => t.Date.Date)
                    .Select(g => new DailyTimesheetSummaryDto
                    {
                        Date = g.Key,
                        Hours = g.Sum(t => t.HoursWorked),
                        Overtime = g.Sum(t => t.OvertimeHours),
                        Amount = g.Sum(t => t.TotalAmount)
                    })
                    .OrderBy(d => d.Date)
                    .ToList();

                var projectSummary = timesheets
                    .GroupBy(t => t.ProjectId)
                    .Select(g => new ProjectTimesheetSummaryDto
                    {
                        ProjectId = g.Key,
                        ProjectName = g.First().Project?.Name ?? "Unknown",
                        TotalHours = g.Sum(t => t.HoursWorked),
                        TotalAmount = g.Sum(t => t.TotalAmount)
                    })
                    .OrderByDescending(p => p.TotalHours)
                    .ToList();

                return new TimesheetSummaryDto
                {
                    UserId = request.UserId,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    TotalHours = totalHours,
                    OvertimeHours = overtimeHours,
                    TotalAmount = totalAmount,
                    DailySummary = dailySummary,
                    ProjectSummary = projectSummary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting timesheet summary for user ID: {UserId}", request.UserId);
                throw;
            }
        }
    }
}