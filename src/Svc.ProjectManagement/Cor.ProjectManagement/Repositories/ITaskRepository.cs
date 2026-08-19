// Repositories/ITaskRepository.cs
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Repositories
{
    public interface ITaskRepository
    {
        Task<ProjectTask?> GetByIdAsync(Guid id);
        Task<IQueryable<ProjectTask>> GetQueryableAsync();
        Task<PaginatedResponse<ProjectTask>> GetByProjectAsync(Guid projectId, TaskFilterDto filter);
        Task<PaginatedResponse<ProjectTask>> GetByAssigneeAsync(Guid assigneeId, TaskFilterDto filter);
        Task<ProjectTask> AddAsync(ProjectTask task);
        Task<ProjectTask> UpdateAsync(ProjectTask task);
        Task<bool> DeleteAsync(Guid id);
        Task<List<ProjectTask>> GetTaskTreeAsync(Guid projectId);
        Task<int> GetNextOrderAsync(Guid projectId);
        Task<bool> HasSubtasksAsync(Guid taskId);
        Task<double> GetProjectCompletionAsync(Guid projectId);
        Task<List<ProjectTask>> GetOverdueTasksAsync(Guid projectId);
        Task<int> GetTaskCountByStatusAsync(Guid projectId, Cor.ProjectManagement.Models.Entities.TaskStatus status);
    }
}