using System;

namespace Cor.CRM.Models.DTOs
{
    public class PaymentDto : BaseDto
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public Guid? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public bool IsReconciled { get; set; }
    }

    public class CreatePaymentDto
    {
        public Guid? InvoiceId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePaymentDto
    {
        public string? Status { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
    }
}
