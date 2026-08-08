using System;

namespace Cor.CRM.Models.DTOs
{
    public abstract class BaseDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
    }
}
