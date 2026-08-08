// Cor.CRM/Handlers/TicketHandlers.cs
using Cor.CRM.Commands;
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Cor.CRM.Persistence;
using Cor.CRM.Queries;
using Dapper;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cor.CRM.Handlers;

// ============================================================
// TICKET ALL HANDLER
// ============================================================

public class TicketAllHandler : IRequestHandler<TicketAllQry, List<TicketDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TicketAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<TicketDto>> Handle(TicketAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    t.""Id"", t.""Title"", t.""Description"",
                    t.""Status"", t.""Priority"", t.""Source"",
                    t.""CustomerId"", c.""Name"" as CustomerName,
                    t.""AssignedToUserId"", t.""AssignedToUserName"",
                    t.""AssignedDate"", t.""ResolvedDate"", t.""ClosedDate"", t.""DueDate"",
                    t.""EstimatedHours"", t.""ActualHours"",
                    t.""Resolution"", t.""Category"", t.""SubCategory"",
                    t.""IsEscalated"", t.""EscalationLevel"", t.""SatisfactionScore"",
                    t.""CreatedAt"", t.""UpdatedAt"",
                    t.""CreatedByUserId"", t.""CreatedByUserName"",
                    t.""UpdatedByUserId"", t.""UpdatedByUserName"",
                    (SELECT COUNT(*) FROM ""TicketComments"" WHERE ""TicketId"" = t.""Id"" AND ""IsDeleted"" = false) as CommentCount,
                    (SELECT COUNT(*) FROM ""TicketAttachments"" WHERE ""TicketId"" = t.""Id"" AND ""IsDeleted"" = false) as AttachmentCount
                FROM ""Tickets"" t
                LEFT JOIN ""Customers"" c ON t.""CustomerId"" = c.""Id""
                WHERE t.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND t.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<TicketStatus>(request.Status));
            }

            if (!string.IsNullOrEmpty(request.Priority))
            {
                sql += " AND t.\"Priority\" = @Priority";
                parameters.Add("@Priority", Enum.Parse<TicketPriority>(request.Priority));
            }

            if (!string.IsNullOrEmpty(request.Source))
            {
                sql += " AND t.\"Source\" = @Source";
                parameters.Add("@Source", Enum.Parse<TicketSource>(request.Source));
            }

            if (request.CustomerId.HasValue)
            {
                sql += " AND t.\"CustomerId\" = @CustomerId";
                parameters.Add("@CustomerId", request.CustomerId.Value);
            }

            if (request.AssignedToUserId.HasValue)
            {
                sql += " AND t.\"AssignedToUserId\" = @AssignedToUserId";
                parameters.Add("@AssignedToUserId", request.AssignedToUserId.Value);
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                sql += " AND t.\"Category\" = @Category";
                parameters.Add("@Category", request.Category);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND t.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND t.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY t.\"CreatedAt\" DESC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var tickets = await _dapper.QueryAsync<TicketDto>(sql, parameters, ct);
            return tickets.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all tickets");
            throw;
        }
    }
}

// ============================================================
// TICKET BY ID HANDLER
// ============================================================

public class TicketByIdHandler : IRequestHandler<TicketByIdQry, TicketDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TicketByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<TicketDto?> Handle(TicketByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    t.""Id"", t.""Title"", t.""Description"",
                    t.""Status"", t.""Priority"", t.""Source"",
                    t.""CustomerId"", c.""Name"" as CustomerName,
                    t.""AssignedToUserId"", t.""AssignedToUserName"",
                    t.""AssignedDate"", t.""ResolvedDate"", t.""ClosedDate"", t.""DueDate"",
                    t.""EstimatedHours"", t.""ActualHours"",
                    t.""Resolution"", t.""Category"", t.""SubCategory"",
                    t.""IsEscalated"", t.""EscalationLevel"", t.""SatisfactionScore"",
                    t.""CreatedAt"", t.""UpdatedAt"",
                    t.""CreatedByUserId"", t.""CreatedByUserName"",
                    t.""UpdatedByUserId"", t.""UpdatedByUserName"",
                    (SELECT COUNT(*) FROM ""TicketComments"" WHERE ""TicketId"" = t.""Id"" AND ""IsDeleted"" = false) as CommentCount,
                    (SELECT COUNT(*) FROM ""TicketAttachments"" WHERE ""TicketId"" = t.""Id"" AND ""IsDeleted"" = false) as AttachmentCount
                FROM ""Tickets"" t
                LEFT JOIN ""Customers"" c ON t.""CustomerId"" = c.""Id""
                WHERE t.""Id"" = @Id AND t.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var ticket = await _dapper.QueryFirstOrDefaultAsync<TicketDto>(sql, parameters, ct);
            return ticket;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ticket by ID: {TicketId}", request.Id);
            throw;
        }
    }
}

// ============================================================
// TICKET STATS HANDLER
// ============================================================

public class TicketStatsHandler : IRequestHandler<TicketStatsQry, TicketStatsResponse>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TicketStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<TicketStatsResponse> Handle(TicketStatsQry request, CancellationToken ct)
    {
        try
        {
            var result = new TicketStatsResponse();

            // Get total counts
            var countSql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as New,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Open,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as InProgress,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Resolved,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Closed,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Reopened,
                    COUNT(CASE WHEN ""Status"" = 7 THEN 1 END) as OnHold,
                    COUNT(CASE WHEN ""Status"" != 4 AND ""Status"" != 5 AND ""DueDate"" < NOW() THEN 1 END) as Overdue,
                    AVG(CASE WHEN ""Status"" = 4 OR ""Status"" = 5 THEN
                        EXTRACT(EPOCH FROM (""ResolvedDate"" - ""CreatedAt""))/3600
                    END) as AverageResolutionHours
                FROM ""Tickets""
                WHERE ""IsDeleted"" = false
            ";

            var counts = await _dapper.QueryFirstOrDefaultAsync<TicketStatsResponse>(countSql, null, ct);
            if (counts != null)
            {
                result.Total = counts.Total;
                result.New = counts.New;
                result.Open = counts.Open;
                result.InProgress = counts.InProgress;
                result.Resolved = counts.Resolved;
                result.Closed = counts.Closed;
                result.Reopened = counts.Reopened;
                result.OnHold = counts.OnHold;
                result.Overdue = counts.Overdue;
                result.AverageResolutionHours = counts.AverageResolutionHours;
            }

            // Get counts by priority
            var byPrioritySql = @"
                SELECT
                    ""Priority"" as Key,
                    COUNT(*) as Value
                FROM ""Tickets""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Priority""
            ";

            var byPriority = await _dapper.QueryAsync<KeyValuePair<string, int>>(byPrioritySql, null, ct);
            result.ByPriority = byPriority.ToDictionary(x => x.Key, x => x.Value);

            // Get counts by source
            var bySourceSql = @"
                SELECT
                    ""Source"" as Key,
                    COUNT(*) as Value
                FROM ""Tickets""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Source""
            ";

            var bySource = await _dapper.QueryAsync<KeyValuePair<string, int>>(bySourceSql, null, ct);
            result.BySource = bySource.ToDictionary(x => x.Key, x => x.Value);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ticket stats");
            throw;
        }
    }
}

// ============================================================
// TICKET ADD HANDLER
// ============================================================

public class TicketAddHandler : IRequestHandler<TicketAddCmd, TicketDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TicketAddHandler(IUnitOfWork uow, ILogService logger, IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return Guid.TryParse(userId, out var id) ? id : null;
    }

    private string? GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("userName")?.Value;
    }

    public async Task<TicketDto> Handle(TicketAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();

            var ticket = new Ticket
            {
                Id = Guid.CreateVersion7(),
                Title = request.Dto.Title,
                Description = request.Dto.Description,
                Status = !string.IsNullOrEmpty(request.Dto.Status)
                    ? Enum.Parse<TicketStatus>(request.Dto.Status)
                    : TicketStatus.New,
                Priority = !string.IsNullOrEmpty(request.Dto.Priority)
                    ? Enum.Parse<TicketPriority>(request.Dto.Priority)
                    : TicketPriority.Medium,
                Source = !string.IsNullOrEmpty(request.Dto.Source)
                    ? Enum.Parse<TicketSource>(request.Dto.Source)
                    : TicketSource.Email,
                CustomerId = request.Dto.CustomerId,
                AssignedToUserId = request.Dto.AssignedToUserId,
                DueDate = request.Dto.DueDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.DueDate.Value)
                    : null,
                Category = request.Dto.Category,
                SubCategory = request.Dto.SubCategory,
                CreatedByUserId = userId,
                CreatedByUserName = userName,
                UpdatedByUserId = userId,
                UpdatedByUserName = userName,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            // If assigned, set assigned date
            if (ticket.AssignedToUserId.HasValue)
            {
                ticket.AssignedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            }

            await _uow.Add(ticket, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Ticket created successfully: {Title}", ticket.Title);

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                Source = ticket.Source.ToString(),
                CustomerId = ticket.CustomerId,
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToUserName = ticket.AssignedToUserName,
                AssignedDate = ticket.AssignedDate,
                DueDate = ticket.DueDate,
                Category = ticket.Category,
                SubCategory = ticket.SubCategory,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                CreatedByUserId = ticket.CreatedByUserId,
                CreatedByUserName = ticket.CreatedByUserName,
                UpdatedByUserId = ticket.UpdatedByUserId,
                UpdatedByUserName = ticket.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to create ticket");
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// Add these to TicketHandlers.cs

// ============================================================
// TICKET UPDATE HANDLER
// ============================================================

public class TicketUpdateHandler : IRequestHandler<TicketUpdateCmd, TicketDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TicketUpdateHandler(IUnitOfWork uow, ILogService logger, IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return Guid.TryParse(userId, out var id) ? id : null;
    }

    private string? GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("userName")?.Value;
    }

    public async Task<TicketDto> Handle(TicketUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var ticket = await _uow.Set<Ticket>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (ticket == null)
                throw new DomainException($"Ticket with id [{request.Id}] NOT FOUND.");

            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();

            if (!string.IsNullOrEmpty(request.Dto.Title))
                ticket.Title = request.Dto.Title;

            if (request.Dto.Description != null)
                ticket.Description = request.Dto.Description;

            if (!string.IsNullOrEmpty(request.Dto.Status))
                ticket.Status = Enum.Parse<TicketStatus>(request.Dto.Status);

            if (!string.IsNullOrEmpty(request.Dto.Priority))
                ticket.Priority = Enum.Parse<TicketPriority>(request.Dto.Priority);

            if (!string.IsNullOrEmpty(request.Dto.Source))
                ticket.Source = Enum.Parse<TicketSource>(request.Dto.Source);

            if (request.Dto.AssignedToUserId.HasValue)
            {
                ticket.AssignedToUserId = request.Dto.AssignedToUserId;
                if (!ticket.AssignedDate.HasValue)
                    ticket.AssignedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            }

            if (request.Dto.DueDate.HasValue)
                ticket.DueDate = DateTimeHelper.EnsureUtc(request.Dto.DueDate.Value);

            if (request.Dto.Category != null)
                ticket.Category = request.Dto.Category;

            if (request.Dto.SubCategory != null)
                ticket.SubCategory = request.Dto.SubCategory;

            if (request.Dto.Resolution != null)
                ticket.Resolution = request.Dto.Resolution;

            ticket.UpdatedByUserId = userId;
            ticket.UpdatedByUserName = userName;
            ticket.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(ticket);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Ticket updated: {Title}", ticket.Title);

            return MapToDto(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to update ticket: {TicketId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }

    private TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.ToString(),
            Source = ticket.Source.ToString(),
            CustomerId = ticket.CustomerId,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserName = ticket.AssignedToUserName,
            AssignedDate = ticket.AssignedDate,
            ResolvedDate = ticket.ResolvedDate,
            ClosedDate = ticket.ClosedDate,
            DueDate = ticket.DueDate,
            EstimatedHours = ticket.EstimatedHours,
            ActualHours = ticket.ActualHours,
            Resolution = ticket.Resolution,
            Category = ticket.Category,
            SubCategory = ticket.SubCategory,
            IsEscalated = ticket.IsEscalated,
            EscalationLevel = ticket.EscalationLevel,
            SatisfactionScore = ticket.SatisfactionScore,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            CreatedByUserId = ticket.CreatedByUserId,
            CreatedByUserName = ticket.CreatedByUserName,
            UpdatedByUserId = ticket.UpdatedByUserId,
            UpdatedByUserName = ticket.UpdatedByUserName
        };
    }
}

// Similar handlers for UpdateStatus, Assign, Resolve, Close, Delete...

// Similar handlers for Update, UpdateStatus, Assign, Resolve, Close, Delete...
// I'll provide the complete version if needed