// Handlers/AuditLogHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Queries.AuditLogQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, ProjectAuditLogDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAuditLogByIdQueryHandler> _logger;

        public GetAuditLogByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetAuditLogByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectAuditLogDto> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var log = await _context.ProjectAuditLogs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

                if (log == null)
                    throw new Exception($"Audit log with ID {request.Id} not found");

                return _mapper.Map<ProjectAuditLogDto>(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log with ID: {AuditLogId}", request.Id);
                throw;
            }
        }
    }

    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PaginatedResponse<ProjectAuditLogDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAuditLogsQueryHandler> _logger;

        public GetAuditLogsQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetAuditLogsQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ProjectAuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectAuditLogs.AsNoTracking();

                if (request.ProjectId.HasValue)
                    query = query.Where(l => l.ProjectId == request.ProjectId.Value);

                if (request.Action.HasValue)
                    query = query.Where(l => l.Action == request.Action.Value);

                if (!string.IsNullOrEmpty(request.EntityType))
                    query = query.Where(l => l.EntityType == request.EntityType);

                if (!string.IsNullOrEmpty(request.EntityId))
                    query = query.Where(l => l.EntityId == request.EntityId);

                if (request.UserId.HasValue)
                    query = query.Where(l => l.UserId == request.UserId.Value);

                if (request.FromDate.HasValue)
                    query = query.Where(l => l.CreatedAt >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(l => l.CreatedAt <= request.ToDate.Value);

                // Apply sorting
                query = request.OrderBy?.ToLower() switch
                {
                    "action" => request.Descending ? query.OrderByDescending(l => l.Action) : query.OrderBy(l => l.Action),
                    "username" => request.Descending ? query.OrderByDescending(l => l.UserName) : query.OrderBy(l => l.UserName),
                    "entitytype" => request.Descending ? query.OrderByDescending(l => l.EntityType) : query.OrderBy(l => l.EntityType),
                    _ => request.Descending ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt)
                };

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(l => _mapper.Map<ProjectAuditLogDto>(l))
                    .ToListAsync(cancellationToken);

                return new PaginatedResponse<ProjectAuditLogDto>
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
                _logger.LogError(ex, "Error getting audit logs");
                throw;
            }
        }
    }

    public class GetAuditLogSummaryQueryHandler : IRequestHandler<GetAuditLogSummaryQuery, AuditLogSummaryDto>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<GetAuditLogSummaryQueryHandler> _logger;

        public GetAuditLogSummaryQueryHandler(
            ProjectDbContext context,
            ILogger<GetAuditLogSummaryQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AuditLogSummaryDto> Handle(GetAuditLogSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectAuditLogs
                    .AsNoTracking()
                    .Where(l => l.ProjectId == request.ProjectId);

                if (request.FromDate.HasValue)
                    query = query.Where(l => l.CreatedAt >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(l => l.CreatedAt <= request.ToDate.Value);

                var logs = await query.ToListAsync(cancellationToken);

                return new AuditLogSummaryDto
                {
                    ProjectId = request.ProjectId,
                    TotalLogs = logs.Count,
                    LogsByAction = logs.GroupBy(l => l.Action).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    LogsByUser = logs.GroupBy(l => l.UserName).ToDictionary(g => g.Key, g => g.Count()),
                    LogsByEntity = logs.GroupBy(l => l.EntityType).ToDictionary(g => g.Key, g => g.Count()),
                    LastActivity = logs.Any() ? logs.Max(l => l.CreatedAt) : null,
                    MostActiveUser = logs.GroupBy(l => l.UserName)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log summary for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }
}