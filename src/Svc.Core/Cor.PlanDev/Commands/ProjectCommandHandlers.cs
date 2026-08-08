using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Models.Entities;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;
using Cor.PlanDev.Queries;

namespace Cor.PlanDev.Commands;

public class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<CreateProjectCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly ILoggerFactory _loggerFactory;

    public CreateProjectCommandHandler(
        PlanDevDbContext context,
        ILogger<CreateProjectCommandHandler> logger,
        ICacheService cache,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _loggerFactory = loggerFactory;
    }

    private async Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var queryLogger = _loggerFactory.CreateLogger<GetProjectByIdQueryHandler>();
        var queryHandler = new GetProjectByIdQueryHandler(_context, queryLogger, _cache);
        return await queryHandler.Handle(new GetProjectByIdQuery { Id = id }, cancellationToken);
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new project: {ProjectName}", request.CreateDto.Name);

            var code = await GenerateProjectCodeAsync(cancellationToken);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = request.CreateDto.Name,
                Description = request.CreateDto.Description,
                StartDate = request.CreateDto.StartDate,
                EndDate = request.CreateDto.EndDate,
                Status = "Planning",
                Priority = request.CreateDto.Priority,
                Budget = request.CreateDto.Budget,
                ActualCost = 0,
                Progress = 0,
                ProjectType = request.CreateDto.ProjectType,
                Department = request.CreateDto.Department,
                ManagerId = request.CreateDto.ManagerId,
                ManagerName = request.CreateDto.ManagerName,
                SponsorId = request.CreateDto.SponsorId,
                SponsorName = request.CreateDto.SponsorName,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false,
                CreatedByUserId = request.CreateDto.ManagerId,
                CreatedByUserName = request.CreateDto.ManagerName
            };

            project.UpdateRowVersion();

            await _context.Projects.AddAsync(project, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(CacheKeys.ProjectsAll, cancellationToken);

            _logger.LogInformation("✅ Project created successfully: {ProjectCode} - {ProjectName}", code, project.Name);

            return await GetProjectByIdAsync(project.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            throw;
        }
    }

    private async Task<string> GenerateProjectCodeAsync(CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _context.Projects
            .Where(p => p.Code.StartsWith($"PRJ-{year}"))
            .CountAsync(ct) + 1;

        return $"PRJ-{year}-{count:D4}";
    }
}

public class UpdateProjectCommandHandler
    : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly ILoggerFactory _loggerFactory;

    public UpdateProjectCommandHandler(
        PlanDevDbContext context,
        ILogger<UpdateProjectCommandHandler> logger,
        ICacheService cache,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _loggerFactory = loggerFactory;
    }

    private async Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var queryLogger = _loggerFactory.CreateLogger<GetProjectByIdQueryHandler>();
        var queryHandler = new GetProjectByIdQueryHandler(_context, queryLogger, _cache);
        return await queryHandler.Handle(new GetProjectByIdQuery { Id = id }, cancellationToken);
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating project: {ProjectId}", request.UpdateDto.Id);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == request.UpdateDto.Id && !p.IsDeleted, cancellationToken);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID '{request.UpdateDto.Id}' not found");

            if (!string.IsNullOrEmpty(request.UpdateDto.RowVersion) &&
                request.UpdateDto.RowVersion != project.RowVersion)
            {
                throw new DbUpdateConcurrencyException("The project was modified by another user");
            }

            if (!string.IsNullOrEmpty(request.UpdateDto.Name)) project.Name = request.UpdateDto.Name;
            if (!string.IsNullOrEmpty(request.UpdateDto.Description)) project.Description = request.UpdateDto.Description;
            if (request.UpdateDto.StartDate.HasValue) project.StartDate = request.UpdateDto.StartDate.Value;
            if (request.UpdateDto.EndDate.HasValue) project.EndDate = request.UpdateDto.EndDate.Value;
            if (!string.IsNullOrEmpty(request.UpdateDto.Status)) project.Status = request.UpdateDto.Status;
            if (!string.IsNullOrEmpty(request.UpdateDto.Priority)) project.Priority = request.UpdateDto.Priority;
            if (request.UpdateDto.Budget.HasValue) project.Budget = request.UpdateDto.Budget.Value;
            if (!string.IsNullOrEmpty(request.UpdateDto.ProjectType)) project.ProjectType = request.UpdateDto.ProjectType;
            if (!string.IsNullOrEmpty(request.UpdateDto.Department)) project.Department = request.UpdateDto.Department;
            if (request.UpdateDto.ManagerId.HasValue) project.ManagerId = request.UpdateDto.ManagerId.Value;
            if (!string.IsNullOrEmpty(request.UpdateDto.ManagerName)) project.ManagerName = request.UpdateDto.ManagerName;
            if (request.UpdateDto.SponsorId.HasValue) project.SponsorId = request.UpdateDto.SponsorId.Value;
            if (!string.IsNullOrEmpty(request.UpdateDto.SponsorName)) project.SponsorName = request.UpdateDto.SponsorName;

            project.DateMod = DateTime.UtcNow;
            project.UpdatedByUserId = request.UpdateDto.ManagerId;
            project.UpdatedByUserName = request.UpdateDto.ManagerName;
            project.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(CacheKeys.ProjectsAll, cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.ProjectById, project.Id), cancellationToken);

            _logger.LogInformation("✅ Project updated successfully: {ProjectCode}", project.Code);

            return await GetProjectByIdAsync(project.Id, cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict while updating project {ProjectId}", request.UpdateDto.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project");
            throw;
        }
    }
}

public class DeleteProjectCommandHandler
    : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteProjectCommandHandler(
        PlanDevDbContext context,
        ILogger<DeleteProjectCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting project: {ProjectId}", request.Id);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

            if (project == null)
                return false;

            project.IsDeleted = true;
            project.DateMod = DateTime.UtcNow;
            project.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(CacheKeys.ProjectsAll, cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.ProjectById, request.Id), cancellationToken);

            _logger.LogInformation("✅ Project deleted successfully: {ProjectCode}", project.Code);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project");
            throw;
        }
    }
}

public class UpdateProjectProgressCommandHandler
    : IRequestHandler<UpdateProjectProgressCommand, ProjectDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<UpdateProjectProgressCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly ILoggerFactory _loggerFactory;

    public UpdateProjectProgressCommandHandler(
        PlanDevDbContext context,
        ILogger<UpdateProjectProgressCommandHandler> logger,
        ICacheService cache,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _loggerFactory = loggerFactory;
    }

    private async Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var queryLogger = _loggerFactory.CreateLogger<GetProjectByIdQueryHandler>();
        var queryHandler = new GetProjectByIdQueryHandler(_context, queryLogger, _cache);
        return await queryHandler.Handle(new GetProjectByIdQuery { Id = id }, cancellationToken);
    }

    public async Task<ProjectDto> Handle(UpdateProjectProgressCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating project progress: {ProjectId} to {Progress}%",
                request.ProgressDto.Id, request.ProgressDto.Progress);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == request.ProgressDto.Id && !p.IsDeleted, cancellationToken);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID '{request.ProgressDto.Id}' not found");

            if (request.ProgressDto.Progress < 0 || request.ProgressDto.Progress > 100)
                throw new ArgumentException("Progress must be between 0 and 100");

            project.Progress = request.ProgressDto.Progress;
            project.DateMod = DateTime.UtcNow;
            project.UpdateRowVersion();

            if (project.Progress >= 100)
            {
                project.Status = "Completed";
                project.CompletionDate = DateTime.UtcNow;
            }
            else if (project.Status == "Planning" && project.Progress > 0)
            {
                project.Status = "Active";
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(CacheKeys.ProjectsAll, cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.ProjectById, project.Id), cancellationToken);

            _logger.LogInformation("✅ Project progress updated: {ProjectCode} - {Progress}%",
                project.Code, project.Progress);

            return await GetProjectByIdAsync(project.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project progress");
            throw;
        }
    }
}

public class CompleteProjectCommandHandler
    : IRequestHandler<CompleteProjectCommand, ProjectDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<CompleteProjectCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly ILoggerFactory _loggerFactory;

    public CompleteProjectCommandHandler(
        PlanDevDbContext context,
        ILogger<CompleteProjectCommandHandler> logger,
        ICacheService cache,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _loggerFactory = loggerFactory;
    }

    private async Task<ProjectDto> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var queryLogger = _loggerFactory.CreateLogger<GetProjectByIdQueryHandler>();
        var queryHandler = new GetProjectByIdQueryHandler(_context, queryLogger, _cache);
        return await queryHandler.Handle(new GetProjectByIdQuery { Id = id }, cancellationToken);
    }

    public async Task<ProjectDto> Handle(CompleteProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Completing project: {ProjectId}", request.Id);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

            if (project == null)
                throw new KeyNotFoundException($"Project with ID '{request.Id}' not found");

            if (project.Status == "Completed")
            {
                _logger.LogInformation("Project already completed: {ProjectCode}", project.Code);
                return await GetProjectByIdAsync(project.Id, cancellationToken);
            }

            project.Status = "Completed";
            project.Progress = 100;
            project.CompletionDate = DateTime.UtcNow;
            project.DateMod = DateTime.UtcNow;
            project.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(CacheKeys.ProjectsAll, cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.ProjectById, project.Id), cancellationToken);

            _logger.LogInformation("✅ Project completed successfully: {ProjectCode}", project.Code);

            return await GetProjectByIdAsync(project.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing project");
            throw;
        }
    }
}