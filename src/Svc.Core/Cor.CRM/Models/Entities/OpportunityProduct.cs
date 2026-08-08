using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities
{
    public class OpportunityProduct  : BaseEntity
    {
        public Guid OpportunityId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        
        [ForeignKey("OpportunityId")]
        public virtual Opportunity Opportunity { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
