// Queries/AuditLogQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.AuditLogQueries
{
    public class GetAuditLogByIdQuery : IRequest<ProjectAuditLogDto>
    {
        public Guid Id { get; set; }
    }

    public class GetAuditLogsQuery : IRequest<PaginatedResponse<ProjectAuditLogDto>>
    {
        public Guid? ProjectId { get; set; }
        public AuditAction? Action { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = true;
    }

    public class GetAuditLogSummaryQuery : IRequest<AuditLogSummaryDto>
    {
        public Guid ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}