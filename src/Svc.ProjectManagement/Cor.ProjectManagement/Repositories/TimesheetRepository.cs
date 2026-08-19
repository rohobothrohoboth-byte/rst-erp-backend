// Repositories/TimesheetRepository.cs
using Microsoft.EntityFrameworkCore;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;

namespace Cor.ProjectManagement.Repositories
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly ProjectDbContext _context;

        public TimesheetRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<Timesheet?> GetByIdAsync(Guid id)
        {
            return await _context.Timesheets
                .Include(t => t.Project)
                .Include(t => t.Task)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<IQueryable<Timesheet>> GetQueryableAsync()
        {
            return await Task.FromResult(_context.Timesheets
                .AsNoTracking()
                .Where(t => !t.IsDeleted));
        }

        public async Task<PaginatedResponse<Timesheet>> GetByUserAsync(Guid userId, TimesheetFilterDto filter)
        {
            var query = _context.Timesheets
                .AsNoTracking()
                .Where(t => t.UserId == userId && !t.IsDeleted);

            if (filter.FromDate.HasValue)
                query = query.Where(t => t.Date >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.Date <= filter.ToDate.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.ProjectId.HasValue)
                query = query.Where(t => t.ProjectId == filter.ProjectId.Value);

            query = query.OrderByDescending(t => t.Date);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponse<Timesheet>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<PaginatedResponse<Timesheet>> GetByProjectAsync(Guid projectId, TimesheetFilterDto filter)
        {
            var query = _context.Timesheets
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId && !t.IsDeleted);

            if (filter.FromDate.HasValue)
                query = query.Where(t => t.Date >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.Date <= filter.ToDate.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            query = query.OrderByDescending(t => t.Date);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponse<Timesheet>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<Timesheet> AddAsync(Timesheet timesheet)
        {
            await _context.Timesheets.AddAsync(timesheet);
            await _context.SaveChangesAsync();
            return timesheet;
        }

        public async Task<Timesheet> UpdateAsync(Timesheet timesheet)
        {
            _context.Timesheets.Update(timesheet);
            await _context.SaveChangesAsync();
            return timesheet;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var timesheet = await GetByIdAsync(id);
            if (timesheet == null) return false;

            if (timesheet.Status == TimesheetStatus.Approved || timesheet.Status == TimesheetStatus.Paid)
                throw new InvalidOperationException("Cannot delete an approved or paid timesheet");

            timesheet.IsDeleted = true;
            timesheet.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Timesheet>> GetByUserAndDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Timesheets
                .Where(t => t.UserId == userId &&
                           t.Date >= fromDate &&
                           t.Date <= toDate &&
                           !t.IsDeleted)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        public async Task<bool> HasDuplicateEntryAsync(Guid userId, Guid projectId, DateTime date, Guid? excludeId = null)
        {
            var query = _context.Timesheets
                .Where(t => t.UserId == userId &&
                           t.ProjectId == projectId &&
                           t.Date.Date == date.Date &&
                           !t.IsDeleted);

            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<decimal> GetTotalHoursByUserAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Timesheets
                .Where(t => t.UserId == userId &&
                           t.Date >= fromDate &&
                           t.Date <= toDate &&
                           !t.IsDeleted &&
                           t.Status == TimesheetStatus.Approved)
                .SumAsync(t => t.HoursWorked);
        }

        public async Task<decimal> GetTotalHoursByProjectAsync(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Timesheets
                .Where(t => t.ProjectId == projectId &&
                           t.Date >= fromDate &&
                           t.Date <= toDate &&
                           !t.IsDeleted &&
                           t.Status == TimesheetStatus.Approved)
                .SumAsync(t => t.HoursWorked);
        }

        public async Task<List<Timesheet>> GetPendingApprovalsAsync(Guid? projectId = null)
        {
            var query = _context.Timesheets
                .Where(t => t.Status == TimesheetStatus.Submitted && !t.IsDeleted);

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            return await query
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        public async Task<bool> SubmitTimesheetsAsync(List<Guid> timesheetIds, Guid submittedBy)
        {
            var timesheets = await _context.Timesheets
                .Where(t => timesheetIds.Contains(t.Id) && !t.IsDeleted)
                .ToListAsync();

            foreach (var timesheet in timesheets)
            {
                if (timesheet.Status != TimesheetStatus.Draft)
                    throw new InvalidOperationException($"Timesheet {timesheet.Id} is already submitted or approved");

                timesheet.Status = TimesheetStatus.Submitted;
                timesheet.SubmittedAt = DateTime.UtcNow;
                timesheet.SubmittedById = submittedBy;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveTimesheetsAsync(List<Guid> timesheetIds, Guid approvedBy, string notes)
        {
            var timesheets = await _context.Timesheets
                .Where(t => timesheetIds.Contains(t.Id) && !t.IsDeleted)
                .ToListAsync();

            foreach (var timesheet in timesheets)
            {
                if (timesheet.Status != TimesheetStatus.Submitted)
                    throw new InvalidOperationException($"Timesheet {timesheet.Id} is not in submitted status");

                timesheet.Status = TimesheetStatus.Approved;
                timesheet.ApprovedAt = DateTime.UtcNow;
                timesheet.ApprovedById = approvedBy;
                timesheet.ApprovalNote = notes ?? string.Empty;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTimesheetsAsync(List<Guid> timesheetIds, Guid rejectedBy, string reason)
        {
            var timesheets = await _context.Timesheets
                .Where(t => timesheetIds.Contains(t.Id) && !t.IsDeleted)
                .ToListAsync();

            foreach (var timesheet in timesheets)
            {
                if (timesheet.Status != TimesheetStatus.Submitted)
                    throw new InvalidOperationException($"Timesheet {timesheet.Id} is not in submitted status");

                timesheet.Status = TimesheetStatus.Rejected;
                timesheet.RejectedAt = DateTime.UtcNow;
                timesheet.RejectedById = rejectedBy;
                timesheet.RejectionReason = reason;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<Guid, decimal>> GetTimesheetSummaryByProjectAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Timesheets
                .Where(t => t.UserId == userId &&
                           t.Date >= fromDate &&
                           t.Date <= toDate &&
                           !t.IsDeleted &&
                           t.Status == TimesheetStatus.Approved)
                .GroupBy(t => t.ProjectId)
                .Select(g => new { ProjectId = g.Key, TotalHours = g.Sum(t => t.HoursWorked) })
                .ToDictionaryAsync(x => x.ProjectId, x => x.TotalHours);
        }
    }
}