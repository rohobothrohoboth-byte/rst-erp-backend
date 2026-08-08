using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities
{
    public class LeadTask
    {
        public Guid LeadId { get; set; }
        public Guid TaskId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("LeadId")]
        public virtual Lead Lead { get; set; } = null!;
        
        [ForeignKey("TaskId")]
        public virtual Cor.CRM.Models.Entities.Task Task { get; set; } = null!;
    }
}
