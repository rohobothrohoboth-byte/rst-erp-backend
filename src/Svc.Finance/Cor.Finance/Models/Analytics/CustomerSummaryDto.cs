// CustomerSummaryDto.cs
namespace Cor.Finance.Models.DTOs
{
    public class CustomerSummaryDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
        public decimal AverageInvoice { get; set; }
    }


}