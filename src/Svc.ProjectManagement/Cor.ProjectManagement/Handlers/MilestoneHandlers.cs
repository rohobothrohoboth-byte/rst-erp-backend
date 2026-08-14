// Handlers/MilestoneHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.MilestoneCommands;
using Cor.ProjectManagement.Queries.MilestoneQueries;

namespace Cor.ProjectManagement.Handlers
{
    // ============ COMMAND HANDLERS ============

    public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, ProjectMilestoneDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateMilestoneCommandHandler> _logger;

        public CreateMilestoneCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateMilestoneCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectMilestoneDto> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate project exists
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                // Validate phase exists if provided
                if (request.PhaseId.HasValue)
                {
                    var phase = await _context.ProjectPhases
                        .FirstOrDefaultAsync(p => p.Id == request.PhaseId.Value &&
                                                   p.ProjectId == request.ProjectId &&
                                                   !p.IsDeleted, cancellationToken);

                    if (phase == null)
                        throw new Exception($"Phase with ID {request.PhaseId} not found in project");
                }

                var milestone = new ProjectMilestone
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    PhaseId = request.PhaseId,
                    DueDate = request.DueDate,
                    IsCompleted = false,
                    CompletionPercentage = 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectMilestones.AddAsync(milestone, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Milestone created successfully with ID: {MilestoneId}", milestone.Id);
                return _mapper.Map<ProjectMilestoneDto>(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating milestone");
                throw;
            }
        }
    }

    public class UpdateMilestoneCommandHandler : IRequestHandler<UpdateMilestoneCommand, ProjectMilestoneDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMilestoneCommandHandler> _logger;

        public UpdateMilestoneCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateMilestoneCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectMilestoneDto> Handle(UpdateMilestoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var milestone = await _context.ProjectMilestones
                    .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

                if (milestone == null)
                    throw new Exception($"Milestone with ID {request.Id} not found");

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.Title))
                    milestone.Title = request.Title;

                if (!string.IsNullOrEmpty(request.Description))
                    milestone.Description = request.Description;

                if (request.DueDate.HasValue)
                    milestone.DueDate = request.DueDate.Value;

                if (request.ActualDate.HasValue)
                    milestone.ActualDate = request.ActualDate.Value;

                if (request.IsCompleted.HasValue)
                {
                    milestone.IsCompleted = request.IsCompleted.Value;
                    if (request.IsCompleted.Value && !milestone.CompletedAt.HasValue)
                    {
                        milestone.CompletedAt = DateTime.UtcNow;
                        milestone.CompletionPercentage = 100;
                    }
                }

                if (request.CompletionPercentage.HasValue)
                    milestone.CompletionPercentage = request.CompletionPercentage.Value;

                milestone.UpdatedAt = DateTime.UtcNow;
                milestone.UpdatedBy = request.UpdatedBy ?? "System";
                milestone.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                // Update project progress if milestone completed
                if (milestone.IsCompleted)
                {
                    await UpdateProjectProgress(milestone.ProjectId, cancellationToken);
                }

                _logger.LogInformation("Milestone updated successfully with ID: {MilestoneId}", milestone.Id);
                return _mapper.Map<ProjectMilestoneDto>(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating milestone with ID: {MilestoneId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectProgress(Guid projectId, CancellationToken cancellationToken)
        {
            var milestones = await _context.ProjectMilestones
                .Where(m => m.ProjectId == projectId && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            if (milestones.Any())
            {
                var completion = milestones.Average(m => m.CompletionPercentage);
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

    public class DeleteMilestoneCommandHandler : IRequestHandler<DeleteMilestoneCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteMilestoneCommandHandler> _logger;

        public DeleteMilestoneCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteMilestoneCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteMilestoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var milestone = await _context.ProjectMilestones
                    .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

                if (milestone == null)
                    throw new Exception($"Milestone with ID {request.Id} not found");

                // Check if milestone has dependencies
                var hasComments = await _context.ProjectComments
                    .AnyAsync(c => c.MilestoneId == request.Id && !c.IsDeleted, cancellationToken);

                if (hasComments)
                    throw new Exception("Cannot delete milestone with existing comments. Delete comments first.");

                milestone.IsDeleted = true;
                milestone.DeletedAt = DateTime.UtcNow;
                milestone.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                // Update project progress
                await UpdateProjectProgress(milestone.ProjectId, cancellationToken);

                _logger.LogInformation("Milestone deleted successfully with ID: {MilestoneId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting milestone with ID: {MilestoneId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectProgress(Guid projectId, CancellationToken cancellationToken)
        {
            var milestones = await _context.ProjectMilestones
                .Where(m => m.ProjectId == projectId && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            if (milestones.Any())
            {
                var completion = milestones.Average(m => m.CompletionPercentage);
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

    public class CompleteMilestoneCommandHandler : IRequestHandler<CompleteMilestoneCommand, ProjectMilestoneDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CompleteMilestoneCommandHandler> _logger;

        public CompleteMilestoneCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CompleteMilestoneCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectMilestoneDto> Handle(CompleteMilestoneCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var milestone = await _context.ProjectMilestones
                    .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

                if (milestone == null)
                    throw new Exception($"Milestone with ID {request.Id} not found");

                if (milestone.IsCompleted)
                    throw new Exception("Milestone is already completed");

                milestone.IsCompleted = true;
                milestone.CompletedAt = DateTime.UtcNow;
                milestone.CompletedById = Guid.TryParse(request.CompletedBy, out var id) ? id : null;
                milestone.CompletedByName = request.CompletedBy ?? "System";
                milestone.CompletionPercentage = 100;
                milestone.UpdatedAt = DateTime.UtcNow;
                milestone.UpdatedBy = request.CompletedBy ?? "System";
                milestone.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                // Update project progress
                await UpdateProjectProgress(milestone.ProjectId, cancellationToken);

                _logger.LogInformation("Milestone completed successfully with ID: {MilestoneId}", milestone.Id);
                return _mapper.Map<ProjectMilestoneDto>(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing milestone with ID: {MilestoneId}", request.Id);
                throw;
            }
        }

        private async Task UpdateProjectProgress(Guid projectId, CancellationToken cancellationToken)
        {
            var milestones = await _context.ProjectMilestones
                .Where(m => m.ProjectId == projectId && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            if (milestones.Any())
            {
                var completion = milestones.Average(m => m.CompletionPercentage);
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

    // ============ QUERY HANDLERS ============

    public class GetMilestoneByIdQueryHandler : IRequestHandler<GetMilestoneByIdQuery, ProjectMilestoneDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMilestoneByIdQueryHandler> _logger;

        public GetMilestoneByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetMilestoneByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectMilestoneDto> Handle(GetMilestoneByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var milestone = await _context.ProjectMilestones
                    .AsNoTracking()
                    .Include(m => m.Project)
                    .Include(m => m.Phase)
                    .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

                if (milestone == null)
                    throw new Exception($"Milestone with ID {request.Id} not found");

                return _mapper.Map<ProjectMilestoneDto>(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting milestone with ID: {MilestoneId}", request.Id);
                throw;
            }
        }
    }

    public class GetMilestonesByProjectQueryHandler : IRequestHandler<GetMilestonesByProjectQuery, PaginatedResponse<ProjectMilestoneDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMilestonesByProjectQueryHandler> _logger;

        public GetMilestonesByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetMilestonesByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectMilestoneDto>> Handle(GetMilestonesByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectMilestones
                    .AsNoTracking()
                    .Where(m => m.ProjectId == request.ProjectId && !m.IsDeleted);

                if (request.IsCompleted.HasValue)
                    query = query.Where(m => m.IsCompleted == request.IsCompleted.Value);

                if (request.DueDateFrom.HasValue)
                    query = query.Where(m => m.DueDate >= request.DueDateFrom.Value);

                if (request.DueDateTo.HasValue)
                    query = query.Where(m => m.DueDate <= request.DueDateTo.Value);

                if (request.PhaseId.HasValue)
                    query = query.Where(m => m.PhaseId == request.PhaseId.Value);

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .OrderBy(m => m.DueDate)
                    .Select(m => _mapper.Map<ProjectMilestoneDto>(m))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectMilestoneDto>
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
                _logger.LogError(ex, "Error getting milestones for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetUpcomingMilestonesQueryHandler : IRequestHandler<GetUpcomingMilestonesQuery, List<ProjectMilestoneDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUpcomingMilestonesQueryHandler> _logger;

        public GetUpcomingMilestonesQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetUpcomingMilestonesQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectMilestoneDto>> Handle(GetUpcomingMilestonesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var thresholdDate = DateTime.UtcNow.AddDays(request.DaysThreshold);

                var milestones = await _context.ProjectMilestones
                    .AsNoTracking()
                    .Where(m => m.ProjectId == request.ProjectId &&
                               !m.IsDeleted &&
                               !m.IsCompleted &&
                               m.DueDate >= DateTime.UtcNow &&
                               m.DueDate <= thresholdDate)
                    .OrderBy(m => m.DueDate)
                    .Select(m => _mapper.Map<ProjectMilestoneDto>(m))
                    .ToListAsync(cancellationToken);

                return milestones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming milestones for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}