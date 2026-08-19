// Services/ITaskService.cs
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Services
{
    public interface ITaskService
    {
        Task<ProjectTaskDto> GetTaskByIdAsync(Guid id);
        Task<PaginatedResponse<ProjectTaskDto>> GetTasksByProjectAsync(Guid projectId, TaskFilterDto filter);
        Task<ProjectTaskDto> CreateTaskAsync(TaskCreateDto dto);
        Task<ProjectTaskDto> UpdateTaskAsync(Guid id, TaskUpdateDto dto);
        Task<bool> DeleteTaskAsync(Guid id);
        Task<ProjectTaskDto> AssignTaskAsync(TaskAssignmentDto dto);
        Task<ProjectTaskDto> UpdateTaskStatusAsync(Guid id, TaskStatusUpdateDto dto);
    }

    public class TaskService : ITaskService
    {
        public Task<ProjectTaskDto> GetTaskByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<PaginatedResponse<ProjectTaskDto>> GetTasksByProjectAsync(Guid projectId, TaskFilterDto filter) => throw new NotImplementedException();
        public Task<ProjectTaskDto> CreateTaskAsync(TaskCreateDto dto) => throw new NotImplementedException();
        public Task<ProjectTaskDto> UpdateTaskAsync(Guid id, TaskUpdateDto dto) => throw new NotImplementedException();
        public Task<bool> DeleteTaskAsync(Guid id) => throw new NotImplementedException();
        public Task<ProjectTaskDto> AssignTaskAsync(TaskAssignmentDto dto) => throw new NotImplementedException();
        public Task<ProjectTaskDto> UpdateTaskStatusAsync(Guid id, TaskStatusUpdateDto dto) => throw new NotImplementedException();
    }
}