// Models/DTOs/ProjectDocumentDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectDocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? PhaseId { get; set; }
        public string? PhaseName { get; set; }
        public Guid? TaskId { get; set; }
        public string? TaskName { get; set; }
        public DocumentType Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int RevisionNumber { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string LastModifiedByName { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
        public bool IsConfidential { get; set; }
        public bool IsArchived { get; set; }
        public string Tags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }

    public class ProjectDocumentCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? PhaseId { get; set; }
        public Guid? TaskId { get; set; }

        [Required]
        public DocumentType Type { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string? FilePath { get; set; }
        public string? FileSize { get; set; }
        public string? FileType { get; set; }

        public bool IsConfidential { get; set; }
        public string Tags { get; set; } = string.Empty;

        public string? CreatedBy { get; set; }
    }

    public class ProjectDocumentUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsArchived { get; set; }
        public string? Tags { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DocumentUploadResultDto
    {
        public Guid DocumentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public string UploadUrl { get; set; } = string.Empty;
        public bool UploadSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}