// Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;

namespace Cor.ProjectManagement.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ProjectDbContext _context;

        public TaskRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectTask?> GetByIdAsync(Guid id)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.ParentTask)
                .Include(t => t.SubTasks.Where(s => !s.IsDeleted))
                .Include(t => t.Comments.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<IQueryable<ProjectTask>> GetQueryableAsync()
        {
            return await Task.FromResult(_context.ProjectTasks
                .AsNoTracking()
                .Where(t => !t.IsDeleted));
        }

        public async Task<PaginatedResponse<ProjectTask>> GetByProjectAsync(Guid projectId, TaskFilterDto filter)
        {
            var query = _context.ProjectTasks
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId && !t.IsDeleted && t.ParentTaskId == null);

            // Apply filters
            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.AssigneeId.HasValue)
                query = query.Where(t => t.AssigneeId == filter.AssigneeId.Value);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

            // Apply sorting
            query = filter.OrderBy?.ToLower() switch
            {
                "title" => filter.Descending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "status" => filter.Descending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                "priority" => filter.Descending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "duedate" => filter.Descending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "completion" => filter.Descending ? query.OrderByDescending(t => t.CompletionPercentage) : query.OrderBy(t => t.CompletionPercentage),
                "assignee" => filter.Descending ? query.OrderByDescending(t => t.AssigneeName) : query.OrderBy(t => t.AssigneeName),
                _ => filter.Descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponse<ProjectTask>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<PaginatedResponse<ProjectTask>> GetByAssigneeAsync(Guid assigneeId, TaskFilterDto filter)
        {
            var query = _context.ProjectTasks
                .AsNoTracking()
                .Where(t => t.AssigneeId == assigneeId && !t.IsDeleted);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

            query = query.OrderBy(t => t.DueDate ?? t.StartDate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponse<ProjectTask>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<ProjectTask> AddAsync(ProjectTask task)
        {
            await _context.ProjectTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<ProjectTask> UpdateAsync(ProjectTask task)
        {
            _context.ProjectTasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task = await GetByIdAsync(id);
            if (task == null) return false;

            var hasSubtasks = await _context.ProjectTasks
                .AnyAsync(t => t.ParentTaskId == id && !t.IsDeleted);

            if (hasSubtasks)
                throw new InvalidOperationException("Cannot delete task with subtasks. Delete subtasks first.");

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProjectTask>> GetTaskTreeAsync(Guid projectId)
        {
            var allTasks = await _context.ProjectTasks
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .OrderBy(t => t.Order)
                .ToListAsync();

            var taskDict = allTasks.ToDictionary(t => t.Id, t => t);
            var rootTasks = new List<ProjectTask>();

            foreach (var task in allTasks)
            {
                if (task.ParentTaskId.HasValue && taskDict.TryGetValue(task.ParentTaskId.Value, out var parent))
                {
                    parent.SubTasks ??= new List<ProjectTask>();
                    parent.SubTasks.Add(task);
                }
                else
                {
                    rootTasks.Add(task);
                }
            }

            return rootTasks;
        }

        public async Task<int> GetNextOrderAsync(Guid projectId)
        {
            var maxOrder = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .MaxAsync(t => (int?)t.Order) ?? 0;
            return maxOrder + 1;
        }

        public async Task<bool> HasSubtasksAsync(Guid taskId)
        {
            return await _context.ProjectTasks
                .AnyAsync(t => t.ParentTaskId == taskId && !t.IsDeleted);
        }

        public async Task<double> GetProjectCompletionAsync(Guid projectId)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();

            if (!tasks.Any()) return 0;

            return tasks.Average(t => t.CompletionPercentage);
        }

        public async Task<List<ProjectTask>> GetOverdueTasksAsync(Guid projectId)
        {
            return await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId &&
                           !t.IsDeleted &&
                           t.DueDate < DateTime.UtcNow &&
                           t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Completed &&
                           t.Status != Cor.ProjectManagement.Models.Entities.TaskStatus.Cancelled)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<int> GetTaskCountByStatusAsync(Guid projectId, Cor.ProjectManagement.Models.Entities.TaskStatus status)
        {
            return await _context.ProjectTasks
                .CountAsync(t => t.ProjectId == projectId && !t.IsDeleted && t.Status == status);
        }
    }
}