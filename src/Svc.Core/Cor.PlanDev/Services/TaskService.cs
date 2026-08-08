using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Persistence;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.PlanDev.Services;

public class TaskService : ITaskService
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<TaskService> _logger;
    private readonly ICacheService _cache;

    public TaskService(
        PlanDevDbContext context,
        ILogger<TaskService> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<TaskDto>> GetTasksByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        // Implementation
        return new List<TaskDto>();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Implementation
        return null;
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken ct = default)
    {
        // Implementation
        return new TaskDto();
    }

    public async Task<TaskDto> UpdateTaskAsync(UpdateTaskDto dto, CancellationToken ct = default)
    {
        // Implementation
        return new TaskDto();
    }

    public async Task<bool> DeleteTaskAsync(Guid id, CancellationToken ct = default)
    {
        // Implementation
        return true;
    }
}