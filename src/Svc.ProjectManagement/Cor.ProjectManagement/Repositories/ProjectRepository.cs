// Repositories/ProjectRepository.cs
using Microsoft.EntityFrameworkCore;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;

namespace Cor.ProjectManagement.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectDbContext _context;

        public ProjectRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.Phases.Where(ph => !ph.IsDeleted))
                .Include(p => p.Tasks.Where(t => !t.IsDeleted))
                .Include(p => p.Milestones.Where(m => !m.IsDeleted))
                .Include(p => p.Resources.Where(r => !r.IsDeleted))
                .Include(p => p.Budgets.Where(b => !b.IsDeleted))
                .Include(p => p.Risks.Where(r => !r.IsDeleted))
                .Include(p => p.Issues.Where(i => !i.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IQueryable<Project>> GetQueryableAsync()
        {
            return await Task.FromResult(_context.Projects
                .AsNoTracking()
                .Where(p => !p.IsDeleted));
        }

        public async Task<PaginatedResponse<Project>> GetPaginatedAsync(ProjectFilterDto filter)
        {
            var query = _context.Projects
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var search = filter.Search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Code.ToLower().Contains(search) ||
                    (p.Description != null && p.Description.ToLower().Contains(search)));
            }

            // Apply status filter
            if (filter.Status.HasValue)
                query = query.Where(p => p.Status == filter.Status.Value);

            // Apply type filter
            if (filter.Type.HasValue)
                query = query.Where(p => p.Type == filter.Type.Value);

            // Apply project manager filter
            if (filter.ProjectManagerId.HasValue)
                query = query.Where(p => p.ProjectManagerId == filter.ProjectManagerId.Value);

            // Apply date filters
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
                "priority" => filter.Descending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                "completion" => filter.Descending ? query.OrderByDescending(p => p.CompletionPercentage) : query.OrderBy(p => p.CompletionPercentage),
                _ => filter.Descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponse<Project>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<Project> AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var project = await GetByIdAsync(id);
            if (project == null) return false;

            // Check for dependencies
            var hasTasks = await _context.ProjectTasks.AnyAsync(t => t.ProjectId == id && !t.IsDeleted);
            var hasTimesheets = await _context.Timesheets.AnyAsync(t => t.ProjectId == id && !t.IsDeleted);
            var hasResources = await _context.ProjectResources.AnyAsync(r => r.ProjectId == id && !r.IsDeleted);

            if (hasTasks || hasTimesheets || hasResources)
                throw new InvalidOperationException("Cannot delete project with existing tasks, timesheets, or resources. Archive instead.");

            project.IsDeleted = true;
            project.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Projects.CountAsync(p => !p.IsDeleted);
        }

        public async Task<Dictionary<ProjectStatus, int>> GetProjectsByStatusAsync()
        {
            return await _context.Projects
                .Where(p => !p.IsDeleted)
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<List<Project>> GetRecentProjectsAsync(int count)
        {
            return await _context.Projects
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<string> GenerateProjectCodeAsync(string projectName)
        {
            var prefix = string.Concat(projectName.Split(' ')
                .Where(w => !string.IsNullOrEmpty(w))
                .Select(w => char.ToUpper(w[0])))
                .Substring(0, Math.Min(3, projectName.Length))
                .ToUpper();

            var count = await _context.Projects.CountAsync(p => p.Code.StartsWith(prefix)) + 1;
            return $"{prefix}-{DateTime.Now.Year}-{count:D3}";
        }
    }
}