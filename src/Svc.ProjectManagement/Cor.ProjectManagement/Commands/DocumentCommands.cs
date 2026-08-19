// Commands/DocumentCommands.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Commands.DocumentCommands
{
    public class UploadDocumentCommand : IRequest<ProjectDocumentDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public Guid? PhaseId { get; set; }
        public Guid? TaskId { get; set; }
        public DocumentType Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public string? FileSize { get; set; }
        public string? FileType { get; set; }
        public bool IsConfidential { get; set; }
        public string Tags { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }

    public class UpdateDocumentCommand : IRequest<ProjectDocumentDto>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsArchived { get; set; }
        public string? Tags { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteDocumentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class ApproveDocumentCommand : IRequest<ProjectDocumentDto>
    {
        public Guid Id { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class ArchiveDocumentCommand : IRequest<ProjectDocumentDto>
    {
        public Guid Id { get; set; }
        public string? ArchivedBy { get; set; }
    }
}