using System;

namespace Cor.CRM.Models.DTOs
{
    public class NoteDto : BaseDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid? LeadId { get; set; }
        public string? LeadName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? OpportunityId { get; set; }
        public string? OpportunityName { get; set; }
        public bool IsPinned { get; set; }
        public bool IsPrivate { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
    }

    public class CreateNoteDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OpportunityId { get; set; }
        public bool IsPinned { get; set; }
        public bool IsPrivate { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
    }

    public class UpdateNoteDto
    {
        public string? Content { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsPrivate { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
    }
}
