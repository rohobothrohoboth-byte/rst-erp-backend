using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities
{
    public class CampaignCustomer
    {
        public Guid CampaignId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; } = null!;
        
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }
}
