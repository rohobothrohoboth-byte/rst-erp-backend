using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Services;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllProjectsAsync(CancellationToken ct = default);
    Task<ProjectDto?> GetProjectByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, CancellationToken ct = default);
    Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto, CancellationToken ct = default);
    Task<bool> DeleteProjectAsync(Guid id, CancellationToken ct = default);
}