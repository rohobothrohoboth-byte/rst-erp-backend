// Handlers/IssueHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.IssueCommands;
using Cor.ProjectManagement.Queries.IssueQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, ProjectIssueDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateIssueCommandHandler> _logger;

        public CreateIssueCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateIssueCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectIssueDto> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var issue = new ProjectIssue
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    Type = request.Type,
                    Priority = request.Priority,
                    Status = IssueStatus.Open,
                    ReportedByName = request.ReportedByName ?? "System",
                    ReportedAt = DateTime.UtcNow,
                    AssignedToId = request.AssignedToId,
                    AssignedToName = request.AssignedToName ?? string.Empty,
                    DueDate = request.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectIssues.AddAsync(issue, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Issue created successfully with ID: {IssueId}", issue.Id);
                return _mapper.Map<ProjectIssueDto>(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue");
                throw;
            }
        }
    }

    public class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand, ProjectIssueDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateIssueCommandHandler> _logger;

        public UpdateIssueCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateIssueCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectIssueDto> Handle(UpdateIssueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var issue = await _context.ProjectIssues
                    .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

                if (issue == null)
                    throw new Exception($"Issue with ID {request.Id} not found");

                if (issue.Status == IssueStatus.Resolved || issue.Status == IssueStatus.Closed)
                    throw new Exception("Cannot update a resolved or closed issue");

                if (!string.IsNullOrEmpty(request.Title))
                    issue.Title = request.Title;

                if (!string.IsNullOrEmpty(request.Description))
                    issue.Description = request.Description;

                if (request.Type.HasValue)
                    issue.Type = request.Type.Value;

                if (request.Priority.HasValue)
                    issue.Priority = request.Priority.Value;

                if (request.Status.HasValue)
                    issue.Status = request.Status.Value;

                if (request.AssignedToId.HasValue)
                    issue.AssignedToId = request.AssignedToId.Value;

                if (!string.IsNullOrEmpty(request.AssignedToName))
                    issue.AssignedToName = request.AssignedToName;

                if (request.DueDate.HasValue)
                    issue.DueDate = request.DueDate.Value;

                if (!string.IsNullOrEmpty(request.Resolution))
                    issue.Resolution = request.Resolution;

                if (!string.IsNullOrEmpty(request.RootCause))
                    issue.RootCause = request.RootCause;

                if (!string.IsNullOrEmpty(request.Impact))
                    issue.Impact = request.Impact;

                issue.UpdatedAt = DateTime.UtcNow;
                issue.UpdatedBy = request.UpdatedBy ?? "System";
                issue.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Issue updated successfully with ID: {IssueId}", issue.Id);
                return _mapper.Map<ProjectIssueDto>(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue with ID: {IssueId}", request.Id);
                throw;
            }
        }
    }

    public class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteIssueCommandHandler> _logger;

        public DeleteIssueCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteIssueCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteIssueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var issue = await _context.ProjectIssues
                    .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

                if (issue == null)
                    throw new Exception($"Issue with ID {request.Id} not found");

                issue.IsDeleted = true;
                issue.DeletedAt = DateTime.UtcNow;
                issue.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Issue deleted successfully with ID: {IssueId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting issue with ID: {IssueId}", request.Id);
                throw;
            }
        }
    }

    public class ResolveIssueCommandHandler : IRequestHandler<ResolveIssueCommand, ProjectIssueDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ResolveIssueCommandHandler> _logger;

        public ResolveIssueCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ResolveIssueCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectIssueDto> Handle(ResolveIssueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var issue = await _context.ProjectIssues
                    .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

                if (issue == null)
                    throw new Exception($"Issue with ID {request.Id} not found");

                if (issue.Status == IssueStatus.Resolved || issue.Status == IssueStatus.Closed)
                    throw new Exception("Issue is already resolved or closed");

                issue.Status = IssueStatus.Resolved;
                issue.Resolution = request.Resolution;
                issue.RootCause = request.RootCause ?? issue.RootCause;
                issue.ResolvedAt = DateTime.UtcNow;
                issue.ResolvedById = Guid.TryParse(request.ResolvedBy, out var id) ? id : null;
                issue.ResolvedByName = request.ResolvedBy ?? "System";
                issue.UpdatedAt = DateTime.UtcNow;
                issue.UpdatedBy = request.ResolvedBy ?? "System";
                issue.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Issue resolved successfully with ID: {IssueId}", issue.Id);
                return _mapper.Map<ProjectIssueDto>(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving issue with ID: {IssueId}", request.Id);
                throw;
            }
        }
    }

    public class UpdateIssueStatusCommandHandler : IRequestHandler<UpdateIssueStatusCommand, ProjectIssueDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateIssueStatusCommandHandler> _logger;

        public UpdateIssueStatusCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateIssueStatusCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectIssueDto> Handle(UpdateIssueStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var issue = await _context.ProjectIssues
                    .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

                if (issue == null)
                    throw new Exception($"Issue with ID {request.Id} not found");

                if (issue.Status == IssueStatus.Resolved || issue.Status == IssueStatus.Closed)
                    throw new Exception("Cannot change status of resolved or closed issue");

                issue.Status = request.Status;
                issue.UpdatedAt = DateTime.UtcNow;
                issue.UpdatedBy = request.UpdatedBy ?? "System";
                issue.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Issue status updated to {Status} with ID: {IssueId}", request.Status, issue.Id);
                return _mapper.Map<ProjectIssueDto>(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue status with ID: {IssueId}", request.Id);
                throw;
            }
        }
    }

    // Query Handlers
    public class GetIssueByIdQueryHandler : IRequestHandler<GetIssueByIdQuery, ProjectIssueDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetIssueByIdQueryHandler> _logger;

        public GetIssueByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetIssueByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectIssueDto> Handle(GetIssueByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var issue = await _context.ProjectIssues
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

                if (issue == null)
                    throw new Exception($"Issue with ID {request.Id} not found");

                return _mapper.Map<ProjectIssueDto>(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue with ID: {IssueId}", request.Id);
                throw;
            }
        }
    }

    public class GetIssuesByProjectQueryHandler : IRequestHandler<GetIssuesByProjectQuery, List<ProjectIssueDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetIssuesByProjectQueryHandler> _logger;

        public GetIssuesByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetIssuesByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectIssueDto>> Handle(GetIssuesByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectIssues
                    .AsNoTracking()
                    .Where(i => i.ProjectId == request.ProjectId && !i.IsDeleted);

                if (request.Type.HasValue)
                    query = query.Where(i => i.Type == request.Type.Value);

                if (request.Priority.HasValue)
                    query = query.Where(i => i.Priority == request.Priority.Value);

                if (request.Status.HasValue)
                    query = query.Where(i => i.Status == request.Status.Value);

                if (request.AssignedToId.HasValue)
                    query = query.Where(i => i.AssignedToId == request.AssignedToId.Value);

                var items = await query
                    .OrderByDescending(i => i.Priority)
                    .ThenByDescending(i => i.CreatedAt)
                    .Select(i => _mapper.Map<ProjectIssueDto>(i))
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issues for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetIssueSummaryQueryHandler : IRequestHandler<GetIssueSummaryQuery, IssueSummaryDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetIssueSummaryQueryHandler> _logger;

        public GetIssueSummaryQueryHandler(
            ProjectDbContext context,
            ILogger<GetIssueSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IssueSummaryDto> Handle(GetIssueSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var issues = await _context.ProjectIssues
                    .AsNoTracking()
                    .Where(i => i.ProjectId == request.ProjectId && !i.IsDeleted)
                    .ToListAsync(cancellationToken);

                return new IssueSummaryDto
                {
                    ProjectId = request.ProjectId,
                    TotalIssues = issues.Count,
                    OpenIssues = issues.Count(i => i.Status == IssueStatus.Open || i.Status == IssueStatus.InProgress),
                    ResolvedIssues = issues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed),
                    IssuesByPriority = issues.GroupBy(i => i.Priority).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    IssuesByStatus = issues.GroupBy(i => i.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    IssuesByType = issues.GroupBy(i => i.Type).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    AverageResolutionTime = issues
                        .Where(i => i.ResolvedAt.HasValue)
                        .Select(i => (i.ResolvedAt.Value - i.ReportedAt).TotalHours)
                        .DefaultIfEmpty(0)
                        .Average()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue summary for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}