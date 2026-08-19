// Models/DTOs/ProjectCommentDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectCommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? TaskId { get; set; }
        public string? TaskName { get; set; }
        public Guid? MilestoneId { get; set; }
        public string? MilestoneName { get; set; }
        public Guid? IssueId { get; set; }
        public string? IssueName { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime? EditedAt { get; set; }
        public string EditedByName { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<ProjectCommentDto> Replies { get; set; } = new List<ProjectCommentDto>();
        public int ReplyCount { get; set; }
    }

    public class ProjectCommentCreateDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public Guid ProjectId { get; set; }

        public Guid? TaskId { get; set; }
        public Guid? MilestoneId { get; set; }
        public Guid? IssueId { get; set; }
        public Guid? ParentCommentId { get; set; }

        public string? AuthorName { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class ProjectCommentUpdateDto
    {
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsResolved { get; set; }
        public string? UpdatedBy { get; set; }
    }
}