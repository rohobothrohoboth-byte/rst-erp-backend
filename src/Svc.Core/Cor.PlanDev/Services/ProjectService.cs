using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.PlanDev.Services;

public class ProjectService : IProjectService
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<ProjectService> _logger;
    private readonly ICacheService _cache;

    public ProjectService(
        PlanDevDbContext context,
        ILogger<ProjectService> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProjectDto>> GetAllProjectsAsync(CancellationToken ct = default)
    {
        // Implementation
        return new List<ProjectDto>();
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Implementation
        return null;
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, CancellationToken ct = default)
    {
        // Implementation
        return new ProjectDto();
    }

    public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto, CancellationToken ct = default)
    {
        // Implementation
        return new ProjectDto();
    }

    public async Task<bool> DeleteProjectAsync(Guid id, CancellationToken ct = default)
    {
        // Implementation
        return true;
    }
}