// Services/ProjectService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Repositories;

namespace Cor.ProjectManagement.Services
{
    public interface IProjectService
    {
        Task<ProjectDto> GetProjectByIdAsync(Guid id);
        Task<PaginatedResponse<ProjectDto>> GetProjectsAsync(ProjectFilterDto filter);
        Task<ProjectDto> CreateProjectAsync(ProjectCreateDto dto);
        Task<ProjectDto> UpdateProjectAsync(Guid id, ProjectUpdateDto dto);
        Task<bool> DeleteProjectAsync(Guid id);
        Task<ProjectDashboardDto> GetDashboardAsync();
        Task<ProjectGanttDto> GetProjectGanttAsync(Guid projectId);
        Task<ProjectTimelineDto> GetProjectTimelineAsync(Guid projectId);
        Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId);
    }

    public class ProjectService : IProjectService
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<ProjectService> _logger;
        private readonly IIntegrationService _integrationService;

        public ProjectService(
            ProjectDbContext context,
            ILogger<ProjectService> logger,
            IIntegrationService integrationService)
        {
            _context = context;
            _logger = logger;
            _integrationService = integrationService;
        }

        public async Task<ProjectDto> GetProjectByIdAsync(Guid id)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Phases)
                    .Include(p => p.Tasks)
                    .Include(p => p.Milestones)
                    .Include(p => p.Resources)
                    .Include(p => p.Budgets)
                    .Include(p => p.Risks)
                    .Include(p => p.Issues)
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (project == null)
                   throw new Exception($"Project with ID {id} not found");

                return MapToDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with ID {ProjectId}", id);
                throw;
            }
        }

        public async Task<PaginatedResponse<ProjectDto>> GetProjectsAsync(ProjectFilterDto filter)
        {
            try
            {
                var query = _context.Projects
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    query = query.Where(p =>
                        p.Name.Contains(filter.Search) ||
                        p.Code.Contains(filter.Search) ||
                        p.Description.Contains(filter.Search));
                }

                if (filter.Status.HasValue)
                    query = query.Where(p => p.Status == filter.Status.Value);

                if (filter.Type.HasValue)
                    query = query.Where(p => p.Type == filter.Type.Value);

                if (filter.ProjectManagerId.HasValue)
                    query = query.Where(p => p.ProjectManagerId == filter.ProjectManagerId.Value);

                if (filter.StartDateFrom.HasValue)
                    query = query.Where(p => p.StartDate >= filter.StartDateFrom.Value);

                if (filter.StartDateTo.HasValue)
                    query = query.Where(p => p.StartDate <= filter.StartDateTo.Value);

                if (filter.EndDateFrom.HasValue)
                    query = query.Where(p => p.EndDate >= filter.EndDateFrom.Value);

                if (filter.EndDateTo.HasValue)
                    query = query.Where(p => p.EndDate <= filter.EndDateTo.Value);

                if (filter.CustomerId.HasValue)
                    query = query.Where(p => p.CustomerId == filter.CustomerId.Value.ToString());

                // Apply sorting
                query = filter.OrderBy?.ToLower() switch
                {
                    "name" => filter.Descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "code" => filter.Descending ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
                    "startdate" => filter.Descending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                    "enddate" => filter.Descending ? query.OrderByDescending(p => p.EndDate) : query.OrderBy(p => p.EndDate),
                    "budget" => filter.Descending ? query.OrderByDescending(p => p.Budget) : query.OrderBy(p => p.Budget),
                    "status" => filter.Descending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
                    _ => filter.Descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(p => MapToDto(p))
                    .ToListAsync();

                return new PaginatedResponse<ProjectDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects with filter {@Filter}", filter);
                throw;
            }
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectCreateDto dto)
        {
            try
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Code = GenerateProjectCode(dto.Name),
                    Description = dto.Description,
                    Type = dto.Type,
                    Status = ProjectStatus.Draft,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Budget = dto.Budget,
                    ProjectManagerId = dto.ProjectManagerId,
                    ProjectManagerName = dto.ProjectManagerName ?? string.Empty,
                    DepartmentId = dto.DepartmentId,
                    DepartmentName = dto.DepartmentName ?? string.Empty,
                    CustomerId = dto.CustomerId?.ToString(),
                    CustomerName = dto.CustomerName ?? string.Empty,
                    Priority = dto.Priority,
                    Tags = dto.Tags ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy ?? "System"
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                // Notify stakeholders
                await _integrationService.NotifyProjectCreatedAsync(project);

                return MapToDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                throw;
            }
        }

        public async Task<ProjectDto> UpdateProjectAsync(Guid id, ProjectUpdateDto dto)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (project == null)
                   throw new Exception($"Project with ID {id} not found");

                // Update properties
                if (!string.IsNullOrEmpty(dto.Name))
                    project.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Description))
                    project.Description = dto.Description;

                if (dto.Status.HasValue)
                    project.Status = dto.Status.Value;

                if (dto.StartDate.HasValue)
                    project.StartDate = dto.StartDate.Value;

                if (dto.EndDate.HasValue)
                    project.EndDate = dto.EndDate.Value;

                if (dto.Budget.HasValue)
                    project.Budget = dto.Budget.Value;

                if (dto.ProjectManagerId.HasValue)
                    project.ProjectManagerId = dto.ProjectManagerId.Value;

                if (dto.Priority.HasValue)
                    project.Priority = dto.Priority.Value;

                project.UpdatedAt = DateTime.UtcNow;
                project.UpdatedBy = dto.UpdatedBy ?? "System";
                project.Version += 1;

                await _context.SaveChangesAsync();

                // Notify stakeholders
                await _integrationService.NotifyProjectUpdatedAsync(project);

                return MapToDto(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID {ProjectId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (project == null)
                  throw new Exception($"Project with ID {id} not found");

                // Soft delete
                project.IsDeleted = true;
                project.DeletedAt = DateTime.UtcNow;
                project.DeletedBy = "System";

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID {ProjectId}", id);
                throw;
            }
        }

        public async Task<ProjectDashboardDto> GetDashboardAsync()
        {
            try
            {
                var totalProjects = await _context.Projects
                    .CountAsync(p => !p.IsDeleted);

                var projectsByStatus = await _context.Projects
                    .Where(p => !p.IsDeleted)
                    .GroupBy(p => p.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);

                var recentProjects = await _context.Projects
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .Select(p => MapToDto(p))
                    .ToListAsync();

                var upcomingMilestones = await _context.ProjectMilestones
                    .Where(m => !m.IsDeleted && m.DueDate >= DateTime.UtcNow && m.IsCompleted == false)
                    .OrderBy(m => m.DueDate)
                    .Take(10)
                    .Select(m => new MilestoneDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        DueDate = m.DueDate,
                        ProjectId = m.ProjectId,
                        ProjectName = m.Project.Name
                    })
                    .ToListAsync();

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
                        AssigneeName = t.AssigneeName
                    })
                    .ToListAsync();

                return new ProjectDashboardDto
                {
                    TotalProjects = totalProjects,
                    ProjectsByStatus = projectsByStatus,
                    RecentProjects = recentProjects,
                    UpcomingMilestones = upcomingMilestones,
                    OverdueTasks = overdueTasks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data");
                throw;
            }
        }

        public async Task<ProjectGanttDto> GetProjectGanttAsync(Guid projectId)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Phases)
                    .Include(p => p.Tasks)
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

                if (project == null)
                  throw new Exception($"Project with ID {projectId} not found");

                var tasks = project.Tasks.Select(t => new GanttTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    StartDate = t.StartDate,
                    EndDate = t.DueDate ?? t.StartDate.AddDays(7),
                    Completion = t.CompletionPercentage,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Assignee = t.AssigneeName
                }).ToList();

                return new ProjectGanttDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate ?? project.StartDate.AddMonths(3),
                    Tasks = tasks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Gantt data for project {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ProjectTimelineDto> GetProjectTimelineAsync(Guid projectId)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Milestones)
                    .Include(p => p.Phases)
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

                if (project == null)
                   throw new Exception($"Project with ID {projectId} not found");

                var milestones = project.Milestones
                    .Where(m => !m.IsDeleted)
                    .OrderBy(m => m.DueDate)
                    .Select(m => new TimelineMilestoneDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Date = m.DueDate,
                        IsCompleted = m.IsCompleted,
                        PhaseName = m.Phase?.Name ?? "General"
                    }).ToList();

                return new ProjectTimelineDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    Milestones = milestones
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting timeline for project {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ProjectStatisticsDto> GetProjectStatisticsAsync(Guid projectId)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Tasks)
                    .Include(p => p.Resources)
                    .Include(p => p.Budgets)
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);

                if (project == null)

                    throw new Exception($"Project with ID {projectId} not found");

                var totalTasks = project.Tasks.Count;
                var completedTasks = project.Tasks.Count(t => t.Status == Cor.ProjectManagement.Models.Entities.TaskStatus.Completed);
                var inProgressTasks = project.Tasks.Count(t => t.Status == Cor.ProjectManagement.Models.Entities.TaskStatus.InProgress);
                var overdueTasks = project.Tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Completed);

                var totalResources = project.Resources.Count;
                var allocatedResources = project.Resources.Count(r => r.Status == ResourceAllocationStatus.Allocated);

                var totalBudget = project.Budget;
                var actualCost = project.ActualCost;
                var budgetUtilization = totalBudget > 0 ? (actualCost / totalBudget) * 100 : 0;

                var tasksByStatus = project.Tasks
                    .GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new ProjectStatisticsDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.Name,
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    InProgressTasks = inProgressTasks,
                    OverdueTasks = overdueTasks,
                    TotalResources = totalResources,
                    AllocatedResources = allocatedResources,
                    TotalBudget = totalBudget,
                    ActualCost = actualCost,
                    BudgetUtilization = Math.Round(budgetUtilization, 2),
                    TasksByStatus = tasksByStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for project {ProjectId}", projectId);
                throw;
            }
        }

        private string GenerateProjectCode(string name)
        {
            var prefix = string.Concat(name.Split(' ')
                .Select(w => char.ToUpper(w[0])))
                .Substring(0, Math.Min(3, name.Length))
                .ToUpper();

            var count = _context.Projects.Count(p => p.Code.StartsWith(prefix)) + 1;
            return $"{prefix}-{DateTime.Now.Year}-{count:D3}";
        }

        private ProjectDto MapToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Code = project.Code,
                Description = project.Description,
                Status = project.Status,
                Type = project.Type,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ActualStartDate = project.ActualStartDate,
                ActualEndDate = project.ActualEndDate,
                ProjectManagerId = project.ProjectManagerId,
                ProjectManagerName = project.ProjectManagerName,
                DepartmentId = project.DepartmentId,
                DepartmentName = project.DepartmentName,
                Budget = project.Budget,
                ActualCost = project.ActualCost,
                TotalBilled = project.TotalBilled,
                Priority = project.Priority,
                CustomerId = project.CustomerId,
                CustomerName = project.CustomerName,
                VendorId = project.VendorId,
                VendorName = project.VendorName,
                CompletionPercentage = project.CompletionPercentage,
                Tags = project.Tags,
                CreatedAt = project.CreatedAt,
                CreatedBy = project.CreatedBy,
                UpdatedAt = project.UpdatedAt,
                UpdatedBy = project.UpdatedBy,
                TaskCount = project.Tasks?.Count ?? 0,
                MilestoneCount = project.Milestones?.Count ?? 0,
                ResourceCount = project.Resources?.Count ?? 0
            };
        }
    }
}