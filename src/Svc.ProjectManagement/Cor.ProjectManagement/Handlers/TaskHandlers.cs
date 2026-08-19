// Handlers/TaskHandlers.cs - FIXED
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.TaskCommands;
using Cor.ProjectManagement.Queries.TaskQueries;
using TaskStatus = Cor.ProjectManagement.Models.Entities.TaskStatus;
namespace Cor.ProjectManagement.Handlers
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ProjectTaskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTaskCommandHandler> _logger;

        public CreateTaskCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateTaskCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectTaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var task = new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    ParentTaskId = request.ParentTaskId,
                    PhaseId = request.PhaseId,
                    Status = TaskStatus.NotStarted,
                    Priority = request.Priority,
                    StartDate = request.StartDate,
                    DueDate = request.DueDate,
                    EstimatedHours = request.EstimatedHours,
                    EstimatedCost = request.EstimatedCost,
                    AssigneeId = request.AssigneeId,
                    AssigneeName = request.AssigneeName ?? string.Empty,
                    ReviewerId = request.ReviewerId,
                    ReviewerName = request.ReviewerName ?? string.Empty,
                    Tags = request.Tags ?? string.Empty,
                    Order = await GetNextOrder(request.ProjectId, cancellationToken),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectTasks.AddAsync(task, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectCompletion(task.ProjectId, cancellationToken);

                _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);
                return _mapper.Map<ProjectTaskDto>(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        private async Task<int> GetNextOrder(Guid projectId, CancellationToken cancellationToken)
        {
            var maxOrder = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .MaxAsync(t => (int?)t.Order, cancellationToken);
            return (maxOrder ?? 0) + 1;
        }

        private async Task UpdateProjectCompletion(Guid projectId, CancellationToken cancellationToken)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            if (tasks.Any())
            {
                var completion = tasks.Average(t => t.CompletionPercentage);
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

                if (project != null)
                {
                    project.CompletionPercentage = Math.Round(completion, 2);
                    project.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, ProjectTaskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTaskCommandHandler> _logger;

        public UpdateTaskCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateTaskCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectTaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _context.ProjectTasks
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

                if (task == null)
                    throw new Exception($"Task with ID {request.Id} not found");

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.Title))
                    task.Title = request.Title;
                if (!string.IsNullOrEmpty(request.Description))
                    task.Description = request.Description;

                // ✅ FIX: Check if Status has value before assigning
                if (request.Status.HasValue)
                    task.Status = request.Status.Value;

                if (request.Priority.HasValue)
                    task.Priority = request.Priority.Value;
                if (request.StartDate.HasValue)
                    task.StartDate = request.StartDate.Value;
                if (request.DueDate.HasValue)
                    task.DueDate = request.DueDate.Value;
                if (request.ActualStartDate.HasValue)
                    task.ActualStartDate = request.ActualStartDate.Value;
                if (request.ActualEndDate.HasValue)
                    task.ActualEndDate = request.ActualEndDate.Value;
                if (request.EstimatedHours.HasValue)
                    task.EstimatedHours = request.EstimatedHours.Value;
                if (request.ActualHours.HasValue)
                    task.ActualHours = request.ActualHours.Value;
                if (request.RemainingHours.HasValue)
                    task.RemainingHours = request.RemainingHours.Value;
                if (request.EstimatedCost.HasValue)
                    task.EstimatedCost = request.EstimatedCost.Value;
                if (request.ActualCost.HasValue)
                    task.ActualCost = request.ActualCost.Value;
                if (request.AssigneeId.HasValue)
                    task.AssigneeId = request.AssigneeId.Value;
                if (!string.IsNullOrEmpty(request.AssigneeName))
                    task.AssigneeName = request.AssigneeName;
                if (request.ReviewerId.HasValue)
                    task.ReviewerId = request.ReviewerId.Value;
                if (!string.IsNullOrEmpty(request.ReviewerName))
                    task.ReviewerName = request.ReviewerName;
                if (request.CompletionPercentage.HasValue)
                    task.CompletionPercentage = request.CompletionPercentage.Value;
                if (!string.IsNullOrEmpty(request.Tags))
                    task.Tags = request.Tags;

                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = request.UpdatedBy ?? "System";
                task.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectCompletion(task.ProjectId, cancellationToken);

                _logger.LogInformation("Task updated successfully with ID: {TaskId}", task.Id);
                return _mapper.Map<ProjectTaskDto>(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectCompletion(Guid projectId, CancellationToken cancellationToken)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            if (tasks.Any())
            {
                var completion = tasks.Average(t => t.CompletionPercentage);
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

                if (project != null)
                {
                    project.CompletionPercentage = Math.Round(completion, 2);
                    project.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;

        public DeleteTaskCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteTaskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _context.ProjectTasks
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

                if (task == null)
                    throw new Exception($"Task with ID {request.Id} not found");

                var hasSubtasks = await _context.ProjectTasks
                    .AnyAsync(t => t.ParentTaskId == request.Id && !t.IsDeleted, cancellationToken);

                if (hasSubtasks)
                    throw new Exception("Cannot delete task with subtasks. Delete subtasks first.");

                task.IsDeleted = true;
                task.DeletedAt = DateTime.UtcNow;
                task.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectCompletion(task.ProjectId, cancellationToken);

                _logger.LogInformation("Task deleted successfully with ID: {TaskId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {TaskId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectCompletion(Guid projectId, CancellationToken cancellationToken)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            if (tasks.Any())
            {
                var completion = tasks.Average(t => t.CompletionPercentage);
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

                if (project != null)
                {
                    project.CompletionPercentage = Math.Round(completion, 2);
                    project.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, ProjectTaskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AssignTaskCommandHandler> _logger;

        public AssignTaskCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<AssignTaskCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectTaskDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _context.ProjectTasks
                    .FirstOrDefaultAsync(t => t.Id == request.TaskId && !t.IsDeleted, cancellationToken);

                if (task == null)
                    throw new Exception($"Task with ID {request.TaskId} not found");

                task.AssigneeId = request.AssigneeId;
                task.AssigneeName = request.AssigneeName;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = request.AssignedBy ?? "System";
                task.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Task {TaskId} assigned to {AssigneeName}", task.Id, request.AssigneeName);
                return _mapper.Map<ProjectTaskDto>(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task with ID: {TaskId}", request.TaskId);
                throw;
            }
        }
    }

    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, ProjectTaskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTaskStatusCommandHandler> _logger;

        public UpdateTaskStatusCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateTaskStatusCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectTaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _context.ProjectTasks
                    .FirstOrDefaultAsync(t => t.Id == request.TaskId && !t.IsDeleted, cancellationToken);

                if (task == null)
                    throw new Exception($"Task with ID {request.TaskId} not found");

                task.Status = request.Status.Value;
                task.UpdatedAt = DateTime.UtcNow;
                task.UpdatedBy = request.UpdatedBy ?? "System";
                task.Version += 1;

                if (request.Status == TaskStatus.InProgress && !task.ActualStartDate.HasValue)
                    task.ActualStartDate = DateTime.UtcNow;
                else if (request.Status == TaskStatus.Completed)
                {
                    task.ActualEndDate = DateTime.UtcNow;
                    task.CompletionPercentage = 100;
                }

                await _context.SaveChangesAsync(cancellationToken);

                await UpdateProjectCompletion(task.ProjectId, cancellationToken);

                _logger.LogInformation("Task {TaskId} status changed to {Status}", task.Id, request.Status);
                return _mapper.Map<ProjectTaskDto>(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status with ID: {TaskId}", request.TaskId);
                throw;
            }
        }

        private async Task UpdateProjectCompletion(Guid projectId, CancellationToken cancellationToken)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            if (tasks.Any())
            {
                var completion = tasks.Average(t => t.CompletionPercentage);
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted, cancellationToken);

                if (project != null)
                {
                    project.CompletionPercentage = Math.Round(completion, 2);
                    project.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    // Query Handlers
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, ProjectTaskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTaskByIdQueryHandler> _logger;

        public GetTaskByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTaskByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectTaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _context.ProjectTasks
                    .AsNoTracking()
                    .Include(t => t.Project)
                    .Include(t => t.ParentTask)
                    .Include(t => t.SubTasks.Where(s => !s.IsDeleted))
                    .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

                if (task == null)
                    throw new Exception($"Task with ID {request.Id} not found");

                var taskDto = _mapper.Map<ProjectTaskDto>(task);
                taskDto.SubTaskCount = task.SubTasks?.Count ?? 0;
                taskDto.SubTasks = _mapper.Map<List<ProjectTaskDto>>(task.SubTasks ?? new List<ProjectTask>());

                return taskDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task with ID: {TaskId}", request.Id);
                throw;
            }
        }
    }

    public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, PaginatedResponse<ProjectTaskDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTasksByProjectQueryHandler> _logger;

        public GetTasksByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTasksByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectTaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectTasks
                    .AsNoTracking()
                    .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted && t.ParentTaskId == null);

                // ✅ FIX: Check HasValue before using Value
                if (request.Status.HasValue)
                    query = query.Where(t => t.Status == request.Status.Value);

                if (request.Priority.HasValue)
                    query = query.Where(t => t.Priority == request.Priority.Value);

                if (request.AssigneeId.HasValue)
                    query = query.Where(t => t.AssigneeId == request.AssigneeId.Value);

                if (request.DueDateFrom.HasValue)
                    query = query.Where(t => t.DueDate >= request.DueDateFrom.Value);

                if (request.DueDateTo.HasValue)
                    query = query.Where(t => t.DueDate <= request.DueDateTo.Value);

                // Apply sorting
                query = request.OrderBy?.ToLower() switch
                {
                    "title" => request.Descending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                    "status" => request.Descending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                    "priority" => request.Descending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                    "duedate" => request.Descending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                    "completion" => request.Descending ? query.OrderByDescending(t => t.CompletionPercentage) : query.OrderBy(t => t.CompletionPercentage),
                    _ => request.Descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
                };

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => _mapper.Map<ProjectTaskDto>(t))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectTaskDto>
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
                _logger.LogError(ex, "Error getting tasks for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetTasksByAssigneeQueryHandler : IRequestHandler<GetTasksByAssigneeQuery, PaginatedResponse<ProjectTaskDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTasksByAssigneeQueryHandler> _logger;

        public GetTasksByAssigneeQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTasksByAssigneeQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectTaskDto>> Handle(GetTasksByAssigneeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectTasks
                    .AsNoTracking()
                    .Where(t => t.AssigneeId == request.AssigneeId && !t.IsDeleted);

                // ✅ FIX: Check HasValue before using Value
                if (request.Status.HasValue)
                    query = query.Where(t => t.Status == request.Status.Value);

                if (request.DueDateFrom.HasValue)
                    query = query.Where(t => t.DueDate >= request.DueDateFrom.Value);

                if (request.DueDateTo.HasValue)
                    query = query.Where(t => t.DueDate <= request.DueDateTo.Value);

                query = query.OrderBy(t => t.DueDate ?? t.StartDate);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => _mapper.Map<ProjectTaskDto>(t))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectTaskDto>
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
                _logger.LogError(ex, "Error getting tasks for assignee ID: {AssigneeId}", request.AssigneeId);
                throw;
            }
        }
    }

    public class GetTaskTreeQueryHandler : IRequestHandler<GetTaskTreeQuery, List<ProjectTaskDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTaskTreeQueryHandler> _logger;

        public GetTaskTreeQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetTaskTreeQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectTaskDto>> Handle(GetTaskTreeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allTasks = await _context.ProjectTasks
                    .AsNoTracking()
                    .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted)
                    .OrderBy(t => t.Order)
                    .ToListAsync(cancellationToken);

                var taskDict = allTasks.ToDictionary(t => t.Id, t => _mapper.Map<ProjectTaskDto>(t));
                var rootTasks = new List<ProjectTaskDto>();

                foreach (var task in allTasks)
                {
                    var taskDto = taskDict[task.Id];
                    if (task.ParentTaskId.HasValue)
                    {
                        if (taskDict.TryGetValue(task.ParentTaskId.Value, out var parent))
                        {
                            parent.SubTasks ??= new List<ProjectTaskDto>();
                            parent.SubTasks.Add(taskDto);
                        }
                    }
                    else
                    {
                        rootTasks.Add(taskDto);
                    }
                }

                return rootTasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task tree for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}