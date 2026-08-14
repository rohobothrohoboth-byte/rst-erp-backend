// Repositories/ITimesheetRepository.cs
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Repositories
{
    public interface ITimesheetRepository
    {
        Task<Timesheet?> GetByIdAsync(Guid id);
        Task<IQueryable<Timesheet>> GetQueryableAsync();
        Task<PaginatedResponse<Timesheet>> GetByUserAsync(Guid userId, TimesheetFilterDto filter);
        Task<PaginatedResponse<Timesheet>> GetByProjectAsync(Guid projectId, TimesheetFilterDto filter);
        Task<Timesheet> AddAsync(Timesheet timesheet);
        Task<Timesheet> UpdateAsync(Timesheet timesheet);
        Task<bool> DeleteAsync(Guid id);
        Task<List<Timesheet>> GetByUserAndDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate);
        Task<bool> HasDuplicateEntryAsync(Guid userId, Guid projectId, DateTime date, Guid? excludeId = null);
        Task<decimal> GetTotalHoursByUserAsync(Guid userId, DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalHoursByProjectAsync(Guid projectId, DateTime fromDate, DateTime toDate);
        Task<List<Timesheet>> GetPendingApprovalsAsync(Guid? projectId = null);
        Task<bool> SubmitTimesheetsAsync(List<Guid> timesheetIds, Guid submittedBy);
        Task<bool> ApproveTimesheetsAsync(List<Guid> timesheetIds, Guid approvedBy, string notes);
        Task<bool> RejectTimesheetsAsync(List<Guid> timesheetIds, Guid rejectedBy, string reason);
        Task<Dictionary<Guid, decimal>> GetTimesheetSummaryByProjectAsync(Guid userId, DateTime fromDate, DateTime toDate);
    }
}