using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Services;

public interface ITaskService
{
    Task<List<TaskDto>> GetTasksByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken ct = default);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken ct = default);
    Task<TaskDto> UpdateTaskAsync(UpdateTaskDto dto, CancellationToken ct = default);
    Task<bool> DeleteTaskAsync(Guid id, CancellationToken ct = default);
}