// Models/DTOs/ProjectAuditLogDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectAuditLogDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public AuditAction Action { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserRole { get; set; }
        public string? UserDepartment { get; set; }
        public string? ClientIp { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }

    public class AuditLogFilterDto
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
}