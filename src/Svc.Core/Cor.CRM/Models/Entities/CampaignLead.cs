using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities
{
    public class CampaignLead
    {
        public Guid CampaignId { get; set; }
        public Guid LeadId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public bool IsConverted { get; set; } = false;
        
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; } = null!;
        
        [ForeignKey("LeadId")]
        public virtual Lead Lead { get; set; } = null!;
    }
}
