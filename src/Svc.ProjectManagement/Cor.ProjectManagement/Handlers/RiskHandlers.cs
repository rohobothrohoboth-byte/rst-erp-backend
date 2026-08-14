// Handlers/RiskHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.RiskCommands;
using Cor.ProjectManagement.Queries.RiskQueries;

namespace Cor.ProjectManagement.Handlers
{
    // ============ COMMAND HANDLERS ============

    public class CreateRiskCommandHandler : IRequestHandler<CreateRiskCommand, ProjectRiskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateRiskCommandHandler> _logger;

        public CreateRiskCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<CreateRiskCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectRiskDto> Handle(CreateRiskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                var risk = new ProjectRisk
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    Impact = request.Impact,
                    Probability = request.Probability,
                    Severity = CalculateSeverity(request.Impact, request.Probability),
                    RiskScore = CalculateRiskScore(request.Impact, request.Probability),
                    MitigationStrategy = request.MitigationStrategy,
                    ContingencyPlan = request.ContingencyPlan,
                    Status = RiskStatus.Identified,
                    IdentifiedByName = request.IdentifiedByName ?? "System",
                    IdentifiedAt = DateTime.UtcNow,
                    AssignedToId = request.AssignedToId,
                    AssignedToName = request.AssignedToName ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectRisks.AddAsync(risk, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Risk created successfully with ID: {RiskId}", risk.Id);
                return _mapper.Map<ProjectRiskDto>(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating risk");
                throw;
            }
        }

        private RiskSeverity CalculateSeverity(RiskImpact impact, RiskProbability probability)
        {
            var severityMatrix = new Dictionary<(RiskImpact, RiskProbability), RiskSeverity>
            {
                {(RiskImpact.VeryLow, RiskProbability.Rare), RiskSeverity.VeryLow},
                {(RiskImpact.VeryLow, RiskProbability.Unlikely), RiskSeverity.VeryLow},
                {(RiskImpact.VeryLow, RiskProbability.Possible), RiskSeverity.Low},
                {(RiskImpact.VeryLow, RiskProbability.Likely), RiskSeverity.Low},
                {(RiskImpact.VeryLow, RiskProbability.AlmostCertain), RiskSeverity.Medium},

                {(RiskImpact.Low, RiskProbability.Rare), RiskSeverity.VeryLow},
                {(RiskImpact.Low, RiskProbability.Unlikely), RiskSeverity.Low},
                {(RiskImpact.Low, RiskProbability.Possible), RiskSeverity.Low},
                {(RiskImpact.Low, RiskProbability.Likely), RiskSeverity.Medium},
                {(RiskImpact.Low, RiskProbability.AlmostCertain), RiskSeverity.Medium},

                {(RiskImpact.Medium, RiskProbability.Rare), RiskSeverity.Low},
                {(RiskImpact.Medium, RiskProbability.Unlikely), RiskSeverity.Low},
                {(RiskImpact.Medium, RiskProbability.Possible), RiskSeverity.Medium},
                {(RiskImpact.Medium, RiskProbability.Likely), RiskSeverity.Medium},
                {(RiskImpact.Medium, RiskProbability.AlmostCertain), RiskSeverity.High},

                {(RiskImpact.High, RiskProbability.Rare), RiskSeverity.Low},
                {(RiskImpact.High, RiskProbability.Unlikely), RiskSeverity.Medium},
                {(RiskImpact.High, RiskProbability.Possible), RiskSeverity.Medium},
                {(RiskImpact.High, RiskProbability.Likely), RiskSeverity.High},
                {(RiskImpact.High, RiskProbability.AlmostCertain), RiskSeverity.Critical},

                {(RiskImpact.Critical, RiskProbability.Rare), RiskSeverity.Medium},
                {(RiskImpact.Critical, RiskProbability.Unlikely), RiskSeverity.Medium},
                {(RiskImpact.Critical, RiskProbability.Possible), RiskSeverity.High},
                {(RiskImpact.Critical, RiskProbability.Likely), RiskSeverity.Critical},
                {(RiskImpact.Critical, RiskProbability.AlmostCertain), RiskSeverity.Critical}
            };

            return severityMatrix.TryGetValue((impact, probability), out var severity)
                ? severity
                : RiskSeverity.Medium;
        }

        private int CalculateRiskScore(RiskImpact impact, RiskProbability probability)
        {
            return ((int)impact) * ((int)probability);
        }
    }

    public class UpdateRiskCommandHandler : IRequestHandler<UpdateRiskCommand, ProjectRiskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRiskCommandHandler> _logger;

        public UpdateRiskCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateRiskCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectRiskDto> Handle(UpdateRiskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var risk = await _context.ProjectRisks
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (risk == null)
                    throw new Exception($"Risk with ID {request.Id} not found");

                if (risk.Status == RiskStatus.Resolved || risk.Status == RiskStatus.Closed)
                    throw new Exception("Cannot update a resolved or closed risk");

                if (!string.IsNullOrEmpty(request.Title))
                    risk.Title = request.Title;

                if (!string.IsNullOrEmpty(request.Description))
                    risk.Description = request.Description;

                if (request.Impact.HasValue)
                    risk.Impact = request.Impact.Value;

                if (request.Probability.HasValue)
                    risk.Probability = request.Probability.Value;

                if (request.Impact.HasValue || request.Probability.HasValue)
                {
                    risk.Severity = CalculateSeverity(risk.Impact, risk.Probability);
                    risk.RiskScore = CalculateRiskScore(risk.Impact, risk.Probability);
                }

                if (!string.IsNullOrEmpty(request.MitigationStrategy))
                    risk.MitigationStrategy = request.MitigationStrategy;

                if (!string.IsNullOrEmpty(request.ContingencyPlan))
                    risk.ContingencyPlan = request.ContingencyPlan;

                if (request.Status.HasValue)
                    risk.Status = request.Status.Value;

                if (request.AssignedToId.HasValue)
                    risk.AssignedToId = request.AssignedToId.Value;

                if (!string.IsNullOrEmpty(request.AssignedToName))
                    risk.AssignedToName = request.AssignedToName;

                if (request.ReviewDate.HasValue)
                    risk.ReviewDate = request.ReviewDate.Value;

                if (!string.IsNullOrEmpty(request.ResolutionNotes))
                    risk.ResolutionNotes = request.ResolutionNotes;

                risk.UpdatedAt = DateTime.UtcNow;
                risk.UpdatedBy = request.UpdatedBy ?? "System";
                risk.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Risk updated successfully with ID: {RiskId}", risk.Id);
                return _mapper.Map<ProjectRiskDto>(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating risk with ID: {RiskId}", request.Id);
                throw;
            }
        }

        private RiskSeverity CalculateSeverity(RiskImpact impact, RiskProbability probability)
        {
            // Same as in CreateRiskCommandHandler
            var severityMatrix = new Dictionary<(RiskImpact, RiskProbability), RiskSeverity>
            {
                {(RiskImpact.VeryLow, RiskProbability.Rare), RiskSeverity.VeryLow},
                {(RiskImpact.VeryLow, RiskProbability.Unlikely), RiskSeverity.VeryLow},
                {(RiskImpact.VeryLow, RiskProbability.Possible), RiskSeverity.Low},
                {(RiskImpact.VeryLow, RiskProbability.Likely), RiskSeverity.Low},
                {(RiskImpact.VeryLow, RiskProbability.AlmostCertain), RiskSeverity.Medium},

                {(RiskImpact.Low, RiskProbability.Rare), RiskSeverity.VeryLow},
                {(RiskImpact.Low, RiskProbability.Unlikely), RiskSeverity.Low},
                {(RiskImpact.Low, RiskProbability.Possible), RiskSeverity.Low},
                {(RiskImpact.Low, RiskProbability.Likely), RiskSeverity.Medium},
                {(RiskImpact.Low, RiskProbability.AlmostCertain), RiskSeverity.Medium},

                {(RiskImpact.Medium, RiskProbability.Rare), RiskSeverity.Low},
                {(RiskImpact.Medium, RiskProbability.Unlikely), RiskSeverity.Low},
                {(RiskImpact.Medium, RiskProbability.Possible), RiskSeverity.Medium},
                {(RiskImpact.Medium, RiskProbability.Likely), RiskSeverity.Medium},
                {(RiskImpact.Medium, RiskProbability.AlmostCertain), RiskSeverity.High},

                {(RiskImpact.High, RiskProbability.Rare), RiskSeverity.Low},
                {(RiskImpact.High, RiskProbability.Unlikely), RiskSeverity.Medium},
                {(RiskImpact.High, RiskProbability.Possible), RiskSeverity.Medium},
                {(RiskImpact.High, RiskProbability.Likely), RiskSeverity.High},
                {(RiskImpact.High, RiskProbability.AlmostCertain), RiskSeverity.Critical},

                {(RiskImpact.Critical, RiskProbability.Rare), RiskSeverity.Medium},
                {(RiskImpact.Critical, RiskProbability.Unlikely), RiskSeverity.Medium},
                {(RiskImpact.Critical, RiskProbability.Possible), RiskSeverity.High},
                {(RiskImpact.Critical, RiskProbability.Likely), RiskSeverity.Critical},
                {(RiskImpact.Critical, RiskProbability.AlmostCertain), RiskSeverity.Critical}
            };

            return severityMatrix.TryGetValue((impact, probability), out var severity)
                ? severity
                : RiskSeverity.Medium;
        }

        private int CalculateRiskScore(RiskImpact impact, RiskProbability probability)
        {
            return ((int)impact) * ((int)probability);
        }
    }

    public class DeleteRiskCommandHandler : IRequestHandler<DeleteRiskCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteRiskCommandHandler> _logger;

        public DeleteRiskCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteRiskCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRiskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var risk = await _context.ProjectRisks
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (risk == null)
                    throw new Exception($"Risk with ID {request.Id} not found");

                risk.IsDeleted = true;
                risk.DeletedAt = DateTime.UtcNow;
                risk.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Risk deleted successfully with ID: {RiskId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting risk with ID: {RiskId}", request.Id);
                throw;
            }
        }
    }

    public class ResolveRiskCommandHandler : IRequestHandler<ResolveRiskCommand, ProjectRiskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ResolveRiskCommandHandler> _logger;

        public ResolveRiskCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ResolveRiskCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectRiskDto> Handle(ResolveRiskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var risk = await _context.ProjectRisks
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (risk == null)
                    throw new Exception($"Risk with ID {request.Id} not found");

                if (risk.Status == RiskStatus.Resolved || risk.Status == RiskStatus.Closed)
                    throw new Exception("Risk is already resolved or closed");

                risk.Status = RiskStatus.Resolved;
                risk.ResolutionNotes = request.ResolutionNotes;
                risk.ResolvedAt = DateTime.UtcNow;
                risk.ResolvedById = Guid.TryParse(request.ResolvedBy, out var id) ? id : null;
                risk.ResolvedByName = request.ResolvedBy ?? "System";
                risk.UpdatedAt = DateTime.UtcNow;
                risk.UpdatedBy = request.ResolvedBy ?? "System";
                risk.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Risk resolved successfully with ID: {RiskId}", risk.Id);
                return _mapper.Map<ProjectRiskDto>(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving risk with ID: {RiskId}", request.Id);
                throw;
            }
        }
    }

    public class UpdateRiskStatusCommandHandler : IRequestHandler<UpdateRiskStatusCommand, ProjectRiskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRiskStatusCommandHandler> _logger;

        public UpdateRiskStatusCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateRiskStatusCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectRiskDto> Handle(UpdateRiskStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var risk = await _context.ProjectRisks
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (risk == null)
                    throw new Exception($"Risk with ID {request.Id} not found");

                if (risk.Status == RiskStatus.Resolved || risk.Status == RiskStatus.Closed)
                    throw new Exception("Cannot change status of resolved or closed risk");

                risk.Status = request.Status;
                risk.UpdatedAt = DateTime.UtcNow;
                risk.UpdatedBy = request.UpdatedBy ?? "System";
                risk.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Risk status updated to {Status} with ID: {RiskId}", request.Status, risk.Id);
                return _mapper.Map<ProjectRiskDto>(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating risk status with ID: {RiskId}", request.Id);
                throw;
            }
        }
    }

    // ============ QUERY HANDLERS ============

    public class GetRiskByIdQueryHandler : IRequestHandler<GetRiskByIdQuery, ProjectRiskDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRiskByIdQueryHandler> _logger;

        public GetRiskByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetRiskByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectRiskDto> Handle(GetRiskByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var risk = await _context.ProjectRisks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

                if (risk == null)
                    throw new Exception($"Risk with ID {request.Id} not found");

                return _mapper.Map<ProjectRiskDto>(risk);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk with ID: {RiskId}", request.Id);
                throw;
            }
        }
    }

    public class GetRisksByProjectQueryHandler : IRequestHandler<GetRisksByProjectQuery, List<ProjectRiskDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRisksByProjectQueryHandler> _logger;

        public GetRisksByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetRisksByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectRiskDto>> Handle(GetRisksByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectRisks
                    .AsNoTracking()
                    .Where(r => r.ProjectId == request.ProjectId && !r.IsDeleted);

                if (request.Status.HasValue)
                    query = query.Where(r => r.Status == request.Status.Value);

                if (request.Severity.HasValue)
                    query = query.Where(r => r.Severity == request.Severity.Value);

                if (request.AssignedToId.HasValue)
                    query = query.Where(r => r.AssignedToId == request.AssignedToId.Value);

                var items = await query
                    .OrderByDescending(r => r.RiskScore)
                    .Select(r => _mapper.Map<ProjectRiskDto>(r))
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risks for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetRiskHeatmapQueryHandler : IRequestHandler<GetRiskHeatmapQuery, RiskHeatmapDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetRiskHeatmapQueryHandler> _logger;

        public GetRiskHeatmapQueryHandler(
            ProjectDbContext context,
            ILogger<GetRiskHeatmapQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RiskHeatmapDto> Handle(GetRiskHeatmapQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var risks = await _context.ProjectRisks
                    .AsNoTracking()
                    .Where(r => r.ProjectId == request.ProjectId && !r.IsDeleted && r.Status != RiskStatus.Resolved)
                    .ToListAsync(cancellationToken);

                var heatmapData = new Dictionary<string, Dictionary<string, int>>();

                foreach (var impact in Enum.GetValues(typeof(RiskImpact)).Cast<RiskImpact>())
                {
                    heatmapData[impact.ToString()] = new Dictionary<string, int>();
                    foreach (var probability in Enum.GetValues(typeof(RiskProbability)).Cast<RiskProbability>())
                    {
                        var count = risks.Count(r => r.Impact == impact && r.Probability == probability);
                        heatmapData[impact.ToString()][probability.ToString()] = count;
                    }
                }

                return new RiskHeatmapDto
                {
                    ProjectId = request.ProjectId,
                    HeatmapData = heatmapData,
                    TotalRisks = risks.Count,
                    HighRiskCount = risks.Count(r => r.Severity == RiskSeverity.Critical || r.Severity == RiskSeverity.High)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk heatmap for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetRiskSummaryQueryHandler : IRequestHandler<GetRiskSummaryQuery, RiskSummaryDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetRiskSummaryQueryHandler> _logger;

        public GetRiskSummaryQueryHandler(
            ProjectDbContext context,
            ILogger<GetRiskSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RiskSummaryDto> Handle(GetRiskSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var risks = await _context.ProjectRisks
                    .AsNoTracking()
                    .Where(r => r.ProjectId == request.ProjectId && !r.IsDeleted)
                    .ToListAsync(cancellationToken);

                return new RiskSummaryDto
                {
                    ProjectId = request.ProjectId,
                    TotalRisks = risks.Count,
                    OpenRisks = risks.Count(r => r.Status == RiskStatus.Identified ||
                                                 r.Status == RiskStatus.Analyzing ||
                                                 r.Status == RiskStatus.Mitigating ||
                                                 r.Status == RiskStatus.Monitored),
                    ResolvedRisks = risks.Count(r => r.Status == RiskStatus.Resolved || r.Status == RiskStatus.Closed),
                    AcceptedRisks = risks.Count(r => r.Status == RiskStatus.Accepted),
                    CriticalRisks = risks.Count(r => r.Severity == RiskSeverity.Critical || r.Severity == RiskSeverity.High),
                    RisksByStatus = risks.GroupBy(r => r.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    RisksBySeverity = risks.GroupBy(r => r.Severity).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    AverageRiskScore = risks.Any() ? risks.Average(r => r.RiskScore) : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk summary for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}