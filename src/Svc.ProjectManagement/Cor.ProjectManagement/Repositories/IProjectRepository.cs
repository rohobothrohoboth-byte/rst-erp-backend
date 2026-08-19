// Repositories/IProjectRepository.cs
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id);
        Task<IQueryable<Project>> GetQueryableAsync();
        Task<PaginatedResponse<Project>> GetPaginatedAsync(ProjectFilterDto filter);
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetTotalCountAsync();
        Task<Dictionary<ProjectStatus, int>> GetProjectsByStatusAsync();
        Task<List<Project>> GetRecentProjectsAsync(int count);
        Task<bool> ExistsAsync(Guid id);
        Task<string> GenerateProjectCodeAsync(string projectName);
    }
}