// Handlers/ChangeHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.ChangeCommands;
using Cor.ProjectManagement.Queries.ChangeQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class CreateChangeCommandHandler : IRequestHandler<CreateChangeCommand, ProjectChangeDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateChangeCommandHandler> _logger;

        public CreateChangeCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateChangeCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectChangeDto> Handle(CreateChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var change = new ProjectChange
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    Type = request.Type,
                    Priority = request.Priority,
                    Status = ChangeStatus.Submitted,
                    CurrentState = request.CurrentState,
                    ProposedState = request.ProposedState,
                    Justification = request.Justification,
                    ImpactAnalysis = request.ImpactAnalysis,
                    CostImpact = request.CostImpact,
                    ScheduleImpact = request.ScheduleImpact,
                    RequestedByName = request.RequestedByName ?? "System",
                    RequestedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectChanges.AddAsync(change, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Change created successfully with ID: {ChangeId}", change.Id);
                return _mapper.Map<ProjectChangeDto>(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating change");
                throw;
            }
        }
    }

    public class UpdateChangeCommandHandler : IRequestHandler<UpdateChangeCommand, ProjectChangeDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateChangeCommandHandler> _logger;

        public UpdateChangeCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateChangeCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectChangeDto> Handle(UpdateChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var change = await _context.ProjectChanges
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (change == null)
                    throw new Exception($"Change with ID {request.Id} not found");

                if (change.Status == ChangeStatus.Approved || change.Status == ChangeStatus.Rejected || change.Status == ChangeStatus.Implemented)
                    throw new Exception("Cannot update an approved, rejected, or implemented change");

                if (!string.IsNullOrEmpty(request.Title))
                    change.Title = request.Title;

                if (!string.IsNullOrEmpty(request.Description))
                    change.Description = request.Description;

                if (request.Type.HasValue)
                    change.Type = request.Type.Value;

                if (request.Priority.HasValue)
                    change.Priority = request.Priority.Value;

                if (request.Status.HasValue)
                    change.Status = request.Status.Value;

                if (!string.IsNullOrEmpty(request.CurrentState))
                    change.CurrentState = request.CurrentState;

                if (!string.IsNullOrEmpty(request.ProposedState))
                    change.ProposedState = request.ProposedState;

                if (!string.IsNullOrEmpty(request.Justification))
                    change.Justification = request.Justification;

                if (!string.IsNullOrEmpty(request.ImpactAnalysis))
                    change.ImpactAnalysis = request.ImpactAnalysis;

                if (request.CostImpact.HasValue)
                    change.CostImpact = request.CostImpact.Value;

                if (request.ScheduleImpact.HasValue)
                    change.ScheduleImpact = request.ScheduleImpact.Value;

                if (!string.IsNullOrEmpty(request.ReviewNotes))
                    change.ReviewNotes = request.ReviewNotes;

                change.UpdatedAt = DateTime.UtcNow;
                change.UpdatedBy = request.UpdatedBy ?? "System";
                change.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Change updated successfully with ID: {ChangeId}", change.Id);
                return _mapper.Map<ProjectChangeDto>(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating change with ID: {ChangeId}", request.Id);
                throw;
            }
        }
    }

    public class DeleteChangeCommandHandler : IRequestHandler<DeleteChangeCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteChangeCommandHandler> _logger;

        public DeleteChangeCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteChangeCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var change = await _context.ProjectChanges
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (change == null)
                    throw new Exception($"Change with ID {request.Id} not found");

                if (change.Status == ChangeStatus.Implemented)
                    throw new Exception("Cannot delete an implemented change");

                change.IsDeleted = true;
                change.DeletedAt = DateTime.UtcNow;
                change.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Change deleted successfully with ID: {ChangeId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting change with ID: {ChangeId}", request.Id);
                throw;
            }
        }
    }

    public class ApproveChangeCommandHandler : IRequestHandler<ApproveChangeCommand, ProjectChangeDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ApproveChangeCommandHandler> _logger;

        public ApproveChangeCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ApproveChangeCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectChangeDto> Handle(ApproveChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var change = await _context.ProjectChanges
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (change == null)
                    throw new Exception($"Change with ID {request.Id} not found");

                if (change.Status != ChangeStatus.UnderReview)
                    throw new Exception("Change must be under review to approve");

                change.Status = ChangeStatus.Approved;
                change.ApprovedById = Guid.TryParse(request.ApprovedBy, out var id) ? id : null;
                change.ApprovedByName = request.ApprovedBy ?? "System";
                change.ApprovedAt = DateTime.UtcNow;
                change.ApprovalNotes = request.Notes ?? string.Empty;
                change.UpdatedAt = DateTime.UtcNow;
                change.UpdatedBy = request.ApprovedBy ?? "System";
                change.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Change approved successfully with ID: {ChangeId}", change.Id);
                return _mapper.Map<ProjectChangeDto>(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving change with ID: {ChangeId}", request.Id);
                throw;
            }
        }
    }

    public class RejectChangeCommandHandler : IRequestHandler<RejectChangeCommand, ProjectChangeDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RejectChangeCommandHandler> _logger;

        public RejectChangeCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<RejectChangeCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectChangeDto> Handle(RejectChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var change = await _context.ProjectChanges
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (change == null)
                    throw new Exception($"Change with ID {request.Id} not found");

                if (change.Status != ChangeStatus.UnderReview)
                    throw new Exception("Change must be under review to reject");

                change.Status = ChangeStatus.Rejected;
                change.ReviewedAt = DateTime.UtcNow;
                change.ReviewNotes = request.Reason;
                change.UpdatedAt = DateTime.UtcNow;
                change.UpdatedBy = request.RejectedBy ?? "System";
                change.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Change rejected successfully with ID: {ChangeId}", change.Id);
                return _mapper.Map<ProjectChangeDto>(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting change with ID: {ChangeId}", request.Id);
                throw;
            }
        }
    }

    public class ImplementChangeCommandHandler : IRequestHandler<ImplementChangeCommand, ProjectChangeDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ImplementChangeCommandHandler> _logger;

        public ImplementChangeCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ImplementChangeCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectChangeDto> Handle(ImplementChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var change = await _context.ProjectChanges
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (change == null)
                    throw new Exception($"Change with ID {request.Id} not found");

                if (change.Status != ChangeStatus.Approved)
                    throw new Exception("Change must be approved to implement");

                change.Status = ChangeStatus.Implemented;
                change.ImplementedById = Guid.TryParse(request.ImplementedBy, out var id) ? id : null;
                change.ImplementedByName = request.ImplementedBy ?? "System";
                change.ImplementedAt = DateTime.UtcNow;
                change.UpdatedAt = DateTime.UtcNow;
                change.UpdatedBy = request.ImplementedBy ?? "System";
                change.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Change implemented successfully with ID: {ChangeId}", change.Id);
                return _mapper.Map<ProjectChangeDto>(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error implementing change with ID: {ChangeId}", request.Id);
                throw;
            }
        }
    }

    // Query Handlers
    public class GetChangeByIdQueryHandler : IRequestHandler<GetChangeByIdQuery, ProjectChangeDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetChangeByIdQueryHandler> _logger;

        public GetChangeByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetChangeByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectChangeDto> Handle(GetChangeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var change = await _context.ProjectChanges
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

                if (change == null)
                    throw new Exception($"Change with ID {request.Id} not found");

                return _mapper.Map<ProjectChangeDto>(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting change with ID: {ChangeId}", request.Id);
                throw;
            }
        }
    }

    public class GetChangesByProjectQueryHandler : IRequestHandler<GetChangesByProjectQuery, List<ProjectChangeDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetChangesByProjectQueryHandler> _logger;

        public GetChangesByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetChangesByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectChangeDto>> Handle(GetChangesByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectChanges
                    .AsNoTracking()
                    .Where(c => c.ProjectId == request.ProjectId && !c.IsDeleted);

                if (request.Type.HasValue)
                    query = query.Where(c => c.Type == request.Type.Value);

                if (request.Priority.HasValue)
                    query = query.Where(c => c.Priority == request.Priority.Value);

                if (request.Status.HasValue)
                    query = query.Where(c => c.Status == request.Status.Value);

                var items = await query
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => _mapper.Map<ProjectChangeDto>(c))
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting changes for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetChangeSummaryQueryHandler : IRequestHandler<GetChangeSummaryQuery, ChangeSummaryDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetChangeSummaryQueryHandler> _logger;

        public GetChangeSummaryQueryHandler(
            ProjectDbContext context,
            ILogger<GetChangeSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChangeSummaryDto> Handle(GetChangeSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var changes = await _context.ProjectChanges
                    .AsNoTracking()
                    .Where(c => c.ProjectId == request.ProjectId && !c.IsDeleted)
                    .ToListAsync(cancellationToken);

                return new ChangeSummaryDto
                {
                    ProjectId = request.ProjectId,
                    TotalChanges = changes.Count,
                    ApprovedChanges = changes.Count(c => c.Status == ChangeStatus.Approved),
                    RejectedChanges = changes.Count(c => c.Status == ChangeStatus.Rejected),
                    ImplementedChanges = changes.Count(c => c.Status == ChangeStatus.Implemented),
                    PendingChanges = changes.Count(c => c.Status == ChangeStatus.Submitted || c.Status == ChangeStatus.UnderReview),
                    TotalCostImpact = changes.Where(c => c.Status == ChangeStatus.Implemented).Sum(c => c.CostImpact),
                    TotalScheduleImpact = changes.Where(c => c.Status == ChangeStatus.Implemented).Sum(c => c.ScheduleImpact),
                    ChangesByType = changes.GroupBy(c => c.Type).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    ChangesByStatus = changes.GroupBy(c => c.Status).ToDictionary(g => g.Key.ToString(), g => g.Count())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting change summary for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}