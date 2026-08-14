// Services/ITimesheetService.cs
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Services
{
    public interface ITimesheetService
    {
        Task<TimesheetDto> GetTimesheetByIdAsync(Guid id);
        Task<PaginatedResponse<TimesheetDto>> GetTimesheetsByUserAsync(Guid userId, TimesheetFilterDto filter);
        Task<TimesheetDto> CreateTimesheetAsync(TimesheetCreateDto dto);
        Task<TimesheetDto> UpdateTimesheetAsync(Guid id, TimesheetUpdateDto dto);
        Task<bool> DeleteTimesheetAsync(Guid id);
        Task<List<TimesheetDto>> SubmitTimesheetsAsync(SubmitTimesheetDto dto);
        Task<List<TimesheetDto>> ApproveTimesheetsAsync(ApproveTimesheetDto dto);
        Task<List<TimesheetDto>> RejectTimesheetsAsync(RejectTimesheetDto dto);
    }

    public class TimesheetService : ITimesheetService
    {
        public Task<TimesheetDto> GetTimesheetByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<PaginatedResponse<TimesheetDto>> GetTimesheetsByUserAsync(Guid userId, TimesheetFilterDto filter) => throw new NotImplementedException();
        public Task<TimesheetDto> CreateTimesheetAsync(TimesheetCreateDto dto) => throw new NotImplementedException();
        public Task<TimesheetDto> UpdateTimesheetAsync(Guid id, TimesheetUpdateDto dto) => throw new NotImplementedException();
        public Task<bool> DeleteTimesheetAsync(Guid id) => throw new NotImplementedException();
        public Task<List<TimesheetDto>> SubmitTimesheetsAsync(SubmitTimesheetDto dto) => throw new NotImplementedException();
        public Task<List<TimesheetDto>> ApproveTimesheetsAsync(ApproveTimesheetDto dto) => throw new NotImplementedException();
        public Task<List<TimesheetDto>> RejectTimesheetsAsync(RejectTimesheetDto dto) => throw new NotImplementedException();
    }
}