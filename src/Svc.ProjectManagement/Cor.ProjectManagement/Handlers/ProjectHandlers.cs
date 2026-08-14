// Handlers/ProjectHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.ProjectCommands;
using Cor.ProjectManagement.Queries.ProjectQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProjectCommandHandler> _logger;

        public CreateProjectCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateProjectCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Code = GenerateProjectCode(request.Name),
                    Description = request.Description,
                    Type = request.Type,
                    Status = ProjectStatus.Draft,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Budget = request.Budget,
                    ProjectManagerId = request.ProjectManagerId,
                    ProjectManagerName = request.ProjectManagerName ?? string.Empty,
                    DepartmentId = request.DepartmentId,
                    DepartmentName = request.DepartmentName ?? string.Empty,
                    CustomerId = request.CustomerId?.ToString(),
                    CustomerName = request.CustomerName ?? string.Empty,
                    Priority = request.Priority,
                    Tags = request.Tags ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.Projects.AddAsync(project, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Project created successfully with ID: {ProjectId}", project.Id);
                return _mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                throw;
            }
        }

        private string GenerateProjectCode(string name)
        {
            var prefix = string.Concat(name.Split(' ')
                .Where(w => !string.IsNullOrEmpty(w))
                .Select(w => char.ToUpper(w[0])))
                .Substring(0, Math.Min(3, name.Length))
                .ToUpper();

            var count = _context.Projects.Count(p => p.Code.StartsWith(prefix)) + 1;
            return $"{prefix}-{DateTime.Now.Year}-{count:D3}";
        }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProjectCommandHandler> _logger;

        public UpdateProjectCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateProjectCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.Id} not found");

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.Name))
                    project.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Description))
                    project.Description = request.Description;
                if (request.Status.HasValue)
                    project.Status = request.Status.Value;
                if (request.StartDate.HasValue)
                    project.StartDate = request.StartDate.Value;
                if (request.EndDate.HasValue)
                    project.EndDate = request.EndDate.Value;
                if (request.ActualStartDate.HasValue)
                    project.ActualStartDate = request.ActualStartDate.Value;
                if (request.ActualEndDate.HasValue)
                    project.ActualEndDate = request.ActualEndDate.Value;
                if (request.Budget.HasValue)
                    project.Budget = request.Budget.Value;
                if (request.ActualCost.HasValue)
                    project.ActualCost = request.ActualCost.Value;
                if (request.TotalBilled.HasValue)
                    project.TotalBilled = request.TotalBilled.Value;
                if (request.ProjectManagerId.HasValue)
                    project.ProjectManagerId = request.ProjectManagerId.Value;
                if (!string.IsNullOrEmpty(request.ProjectManagerName))
                    project.ProjectManagerName = request.ProjectManagerName;
                if (request.Priority.HasValue)
                    project.Priority = request.Priority.Value;
                if (request.CompletionPercentage.HasValue)
                    project.CompletionPercentage = request.CompletionPercentage.Value;
                if (!string.IsNullOrEmpty(request.Tags))
                    project.Tags = request.Tags;

                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = request.UpdatedBy ?? "System";
                project.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Project updated successfully with ID: {ProjectId}", project.Id);
                return _mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID: {ProjectId}", request.Id);
                throw;
            }
        }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteProjectCommandHandler> _logger;

        public DeleteProjectCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteProjectCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.Id} not found");

                // Check for dependencies
                var hasTasks = await _context.ProjectTasks.AnyAsync(t => t.ProjectId == request.Id && !t.IsDeleted, cancellationToken);
                var hasTimesheets = await _context.Timesheets.AnyAsync(t => t.ProjectId == request.Id && !t.IsDeleted, cancellationToken);
                var hasResources = await _context.ProjectResources.AnyAsync(r => r.ProjectId == request.Id && !r.IsDeleted, cancellationToken);

                if (hasTasks || hasTimesheets || hasResources)
                    throw new Exception("Cannot delete project with existing tasks, timesheets, or resources. Archive instead.");

                // Soft delete
                project.IsDeleted = true;
                project.DeletedAt = DateTime.UtcNow;
                project.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Project deleted successfully with ID: {ProjectId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID: {ProjectId}", request.Id);
                throw;
            }
        }
    }

    public class ChangeProjectStatusCommandHandler : IRequestHandler<ChangeProjectStatusCommand, ProjectDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ChangeProjectStatusCommandHandler> _logger;

        public ChangeProjectStatusCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ChangeProjectStatusCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDto> Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.Id} not found");

                // Update status with validation
                project.Status = request.Status;
                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = request.UpdatedBy ?? "System";
                project.Version += 1;

                // Update actual dates if completing or starting
                if (request.Status == ProjectStatus.InProgress && !project.ActualStartDate.HasValue)
                    project.ActualStartDate = DateTime.UtcNow;
                else if (request.Status == ProjectStatus.Completed)
                    project.ActualEndDate = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Project status changed to {Status} for ID: {ProjectId}",
                    request.Status, project.Id);
                return _mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing project status for ID: {ProjectId}", request.Id);
                throw;
            }
        }
    }

    // Query Handlers
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProjectByIdQueryHandler> _logger;

        public GetProjectByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetProjectByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .AsNoTracking()
                    .Include(p => p.Phases.Where(ph => !ph.IsDeleted))
                    .Include(p => p.Tasks.Where(t => !t.IsDeleted))
                    .Include(p => p.Milestones.Where(m => !m.IsDeleted))
                    .Include(p => p.Resources.Where(r => !r.IsDeleted))
                    .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.Id} not found");

                var projectDto = _mapper.Map<ProjectDto>(project);

                // Populate counts
                projectDto.TaskCount = project.Tasks?.Count ?? 0;
                projectDto.MilestoneCount = project.Milestones?.Count ?? 0;
                projectDto.ResourceCount = project.Resources?.Count ?? 0;

                return projectDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with ID: {ProjectId}", request.Id);
                throw;
            }
        }
    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PaginatedResponse<ProjectDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProjectsQueryHandler> _logger;

        public GetProjectsQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetProjectsQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Projects
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(request.Search))
                {
                    var search = request.Search.ToLower();
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(search) ||
                        p.Code.ToLower().Contains(search) ||
                        p.Description.ToLower().Contains(search));
                }

                if (request.Status.HasValue)
                    query = query.Where(p => p.Status == request.Status.Value);

                if (request.Type.HasValue)
                    query = query.Where(p => p.Type == request.Type.Value);

                if (request.ProjectManagerId.HasValue)
                    query = query.Where(p => p.ProjectManagerId == request.ProjectManagerId.Value);

                if (request.StartDateFrom.HasValue)
                    query = query.Where(p => p.StartDate >= request.StartDateFrom.Value);

                if (request.StartDateTo.HasValue)
                    query = query.Where(p => p.StartDate <= request.StartDateTo.Value);

                if (request.EndDateFrom.HasValue)
                    query = query.Where(p => p.EndDate >= request.EndDateFrom.Value);

                if (request.EndDateTo.HasValue)
                    query = query.Where(p => p.EndDate <= request.EndDateTo.Value);

                if (request.CustomerId.HasValue)
                    query = query.Where(p => p.CustomerId == request.CustomerId.Value.ToString());

                // Apply sorting
                query = request.OrderBy?.ToLower() switch
                {
                    "name" => request.Descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "code" => request.Descending ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
                    "startdate" => request.Descending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                    "enddate" => request.Descending ? query.OrderByDescending(p => p.EndDate) : query.OrderBy(p => p.EndDate),
                    "budget" => request.Descending ? query.OrderByDescending(p => p.Budget) : query.OrderBy(p => p.Budget),
                    "status" => request.Descending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
                    "priority" => request.Descending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                    _ => request.Descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
                };

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code,
                        Description = p.Description,
                        Status = p.Status,
                        Type = p.Type,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        ActualStartDate = p.ActualStartDate,
                        ActualEndDate = p.ActualEndDate,
                        ProjectManagerId = p.ProjectManagerId,
                        ProjectManagerName = p.ProjectManagerName,
                        DepartmentId = p.DepartmentId,
                        DepartmentName = p.DepartmentName,
                        Budget = p.Budget,
                        ActualCost = p.ActualCost,
                        TotalBilled = p.TotalBilled,
                        Priority = p.Priority,
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        VendorId = p.VendorId,
                        VendorName = p.VendorName,
                        CompletionPercentage = p.CompletionPercentage,
                        Tags = p.Tags,
                        CreatedAt = p.CreatedAt,
                        CreatedBy = p.CreatedBy,
                        UpdatedAt = p.UpdatedAt,
                        UpdatedBy = p.UpdatedBy,
                        TaskCount = _context.ProjectTasks.Count(t => t.ProjectId == p.Id && !t.IsDeleted),
                        MilestoneCount = _context.ProjectMilestones.Count(m => m.ProjectId == p.Id && !m.IsDeleted),
                        ResourceCount = _context.ProjectResources.Count(r => r.ProjectId == p.Id && !r.IsDeleted)
                    })
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects with filter");
                throw;
            }
        }
    }

    public class GetProjectDashboardQueryHandler : IRequestHandler<GetProjectDashboardQuery, ProjectDashboardDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProjectDashboardQueryHandler> _logger;

        public GetProjectDashboardQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetProjectDashboardQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDashboardDto> Handle(GetProjectDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var totalProjects = await _context.Projects
                    .CountAsync(p => !p.IsDeleted, cancellationToken);

                var projectsByStatus = await _context.Projects
                    .Where(p => !p.IsDeleted)
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

                var recentProjects = await _context.Projects
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .Select(p => _mapper.Map<ProjectDto>(p))
                    .ToListAsync(cancellationToken);

                var upcomingMilestones = await _context.ProjectMilestones
                    .Where(m => !m.IsDeleted &&
                               m.DueDate >= DateTime.UtcNow &&
                               m.IsCompleted == false)
                    .OrderBy(m => m.DueDate)
                    .Take(10)
                    .Select(m => new MilestoneDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        DueDate = m.DueDate,
                        ProjectId = m.ProjectId,
                        ProjectName = m.Project.Name,
                        IsCompleted = m.IsCompleted
                    })
                    .ToListAsync(cancellationToken);

                var overdueTasks = await _context.ProjectTasks
                    .Where(t => !t.IsDeleted &&
                               t.DueDate < DateTime.UtcNow &&
                               t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Completed &&
                               t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Cancelled)
                    .OrderBy(t => t.DueDate)
                    .Take(10)
                    .Select(t => new TaskSummaryDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        DueDate = t.DueDate ?? DateTime.UtcNow,
                        ProjectId = t.ProjectId,
                        ProjectName = t.Project.Name,
                        AssigneeName = t.AssigneeName,
                        Status = t.Status
                    })
                    .ToListAsync(cancellationToken);

                return new ProjectDashboardDto
                {
                    TotalProjects = totalProjects,
                    ProjectsByStatus = projectsByStatus,
                    RecentProjects = recentProjects,
                    UpcomingMilestones = upcomingMilestones,
                    OverdueTasks = overdueTasks,
                    ResourceSummary = new ResourceSummaryDto
                    {
                        TotalResources = await _context.ProjectResources.CountAsync(r => !r.IsDeleted, cancellationToken),
                        AllocatedResources = await _context.ProjectResources
                            .CountAsync(r => !r.IsDeleted && r.Status == ResourceAllocationStatus.Allocated, cancellationToken),
                        AvailableResources = await _context.ProjectResources
                            .CountAsync(r => !r.IsDeleted && r.Status == ResourceAllocationStatus.Planned, cancellationToken)
                    },
                    BudgetSummary = new BudgetSummaryDto
                    {
                        TotalBudget = await _context.Projects.Where(p => !p.IsDeleted).SumAsync(p => p.Budget, cancellationToken),
                        TotalActual = await _context.Projects.Where(p => !p.IsDeleted).SumAsync(p => p.ActualCost, cancellationToken)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data");
                throw;
            }
        }
    }

    public class GetProjectStatisticsQueryHandler : IRequestHandler<GetProjectStatisticsQuery, ProjectStatisticsDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetProjectStatisticsQueryHandler> _logger;

        public GetProjectStatisticsQueryHandler(
            ProjectDbContext context,
            ILogger<GetProjectStatisticsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProjectStatisticsDto> Handle(GetProjectStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var tasks = await _context.ProjectTasks
                    .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted)
                    .ToListAsync(cancellationToken);

                var resources = await _context.ProjectResources
                    .Where(r => r.ProjectId == request.ProjectId && !r.IsDeleted)
                    .ToListAsync(cancellationToken);

                var risks = await _context.ProjectRisks
                    .Where(r => r.ProjectId == request.ProjectId && !r.IsDeleted)
                    .ToListAsync(cancellationToken);

                var issues = await _context.ProjectIssues
                    .Where(i => i.ProjectId == request.ProjectId && !i.IsDeleted)
                    .ToListAsync(cancellationToken);

                return new ProjectStatisticsDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = tasks.Count,
                    CompletedTasks = tasks.Count(t => t.Status == Cor.ProjectManagement.Models.Entities.TaskStatus.Completed),
                    InProgressTasks = tasks.Count(t => t.Status == Cor.ProjectManagement.Models.Entities.TaskStatus.InProgress),
                    OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Completed && t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Cancelled),
                    TotalResources = resources.Count,
                    AllocatedResources = resources.Count(r => r.Status == ResourceAllocationStatus.Allocated),
                    TotalBudget = project.Budget,
                    ActualCost = project.ActualCost,
                    BudgetUtilization = project.Budget > 0 ? (project.ActualCost / project.Budget) * 100 : 0,
                    TasksByStatus = tasks.GroupBy(t => t.Status).ToDictionary(g => g.Key, g => g.Count()),
                    RisksByStatus = risks.GroupBy(r => r.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    IssuesByStatus = issues.GroupBy(i => i.Status).ToDictionary(g => g.Key.ToString(), g => g.Count())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}