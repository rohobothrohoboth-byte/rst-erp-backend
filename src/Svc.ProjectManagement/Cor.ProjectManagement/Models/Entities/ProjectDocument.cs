// Models/Entities/ProjectDocument.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum DocumentType
    {
        Plan = 1,
        Specification = 2,
        Design = 3,
        Contract = 4,
        Report = 5,
        Meeting = 6,
        Presentation = 7,
        Spreadsheet = 8,
        Image = 9,
        Video = 10,
        Other = 11
    }

    public class ProjectDocument : BaseEntity
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

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public string FileSize { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;

        public string Version { get; set; } = "1.0";
        public int RevisionNumber { get; set; } = 1;

        public Guid? UploadedById { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }

        public Guid? LastModifiedById { get; set; }
        public string LastModifiedByName { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }

        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;

        public bool IsConfidential { get; set; }
        public bool IsArchived { get; set; }

        public string Tags { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual ProjectPhase? Phase { get; set; }
        public virtual ProjectTask? Task { get; set; }
    }
}