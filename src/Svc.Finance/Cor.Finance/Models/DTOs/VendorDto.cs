// Models/DTOs/VendorDto.cs
using System;
using System.Collections.Generic;

namespace Cor.Finance.Models.DTOs;

// ============================================================
// VENDOR DTOs
// ============================================================

public class ContactPersonDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
}

public class VendorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string VendorType { get; set; } = "Supplier";
    public string Status { get; set; } = "Active";
    public string? PaymentTerms { get; set; } = "Net 30";
    public string? Currency { get; set; } = "USD";
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? Website { get; set; }
    public ContactPersonDto? ContactPerson { get; set; }
    public decimal? Rating { get; set; }
    public decimal? TotalSpent { get; set; }
    public int? TotalTransactions { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class VendorCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? VendorType { get; set; } = "Supplier";
    public string? PaymentTerms { get; set; } = "Net 30";
    public string? Currency { get; set; } = "USD";
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? Website { get; set; }
    public ContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; } = true;
}

public class VendorUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? VendorType { get; set; }
    public string? Status { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Currency { get; set; }
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? Website { get; set; }
    public ContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RowVersion { get; set; }
}

// ============================================================
// VENDOR PORTAL USER DTOs
// ============================================================

public class PortalVendorUserDto
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Role { get; set; } // 'Admin' | 'Submitter' | 'Viewer'
    public string? Status { get; set; } // 'Active' | 'Inactive' | 'Pending'
    public DateTime? LastLogin { get; set; }
    public List<string>? Permissions { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddPortalVendorUserDto
{    public Guid VendorId { get; set; }
   public string? VendorName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Role { get; set; } = "Submitter";
    public string? Status { get; set; } = "Pending";
    public List<string>? Permissions { get; set; }
}

public class EditPortalVendorUserDto
{
    public Guid Id { get; set; }
   public string? VendorName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Status { get; set; }
    public List<string>? Permissions { get; set; }
    public string? RowVersion { get; set; }
}

// ============================================================
// PORTAL INVOICE DTOs
// ============================================================

public class PortalInvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public string? VendorName { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public string? PoNumber { get; set; }
    public string? GrnNumber { get; set; }
    public string? Notes { get; set; }
    public string? SubmittedBy { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? RejectionReason { get; set; }

    // ✅ ADD THESE MISSING PROPERTIES
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentReference { get; set; }

    public List<InvoiceTrackingDto> Tracking { get; set; } = new();
    public List<InvoiceAttachmentDto> Attachments { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}
public class VendorSummaryDto
{
    public int TotalVendors { get; set; }
    public int ActiveVendors { get; set; }
    public int InactiveVendors { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageRating { get; set; }
    public Dictionary<string, int> VendorsByType { get; set; } = new();
    public Dictionary<string, int> VendorsByStatus { get; set; } = new();

     public Guid VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
        public decimal AverageInvoice { get; set; }
}
public class AddPortalInvoiceDto
{
    public Guid VendorId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public string? PoNumber { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
    public List<Guid>? AttachmentIds { get; set; }
}

public class EditPortalInvoiceDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxAmount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public string? PoNumber { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public string? RowVersion { get; set; }
}

public class InvoiceTrackingDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public string? UpdatedBy { get; set; }
}

public class InvoiceAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}



// ============================================================
// PORTAL PAYMENT DTOs
// ============================================================

public class PortalPaymentDto
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // BankTransfer, CreditCard, Check, Cash, DigitalWallet
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed, Cancelled, Refunded
    public string? Remarks { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? ProcessedBy { get; set; }
    public string? TransactionId { get; set; }
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? FailureReason { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddPortalPaymentDto
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
}

public class EditPortalPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? Status { get; set; }
    public string? RowVersion { get; set; }
}

public class UpdatePaymentStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class PortalPaymentSummaryDto
{
    public decimal TotalPayments { get; set; }
    public decimal PendingPayments { get; set; }
    public decimal CompletedPayments { get; set; }
    public int PaymentCount { get; set; }
    public int PendingCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal AveragePaymentAmount { get; set; }
    public Dictionary<string, decimal> PaymentsByMethod { get; set; } = new();
    public Dictionary<string, int> PaymentsByStatus { get; set; } = new();
}

// ============================================================
// PORTAL NOTIFICATION DTOs
// ============================================================

public class PortalNotificationDto
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
   public string? VendorName { get; set; }
    public string? Type { get; set; } // Invoice_Approved, Invoice_Rejected, Payment_Scheduled, Payment_Made, Reminder, Portal_Update
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? Link { get; set; }
    public string? Priority { get; set; } // Low, Medium, High, Urgent
    public string? Category { get; set; } // Payment, Invoice, Vendor, System, Approval, Reminder, Alert
    public string? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class SendPortalNotificationDto
{
    public Guid VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; } = "Info";
    public string? Priority { get; set; } = "Medium";
    public string? Category { get; set; } = "System";
    public string? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public string? Link { get; set; }
}





public class PortalInvoiceTrackingDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<TrackingEventDto> Events { get; set; } = new();
    public List<InvoiceTrackingDto> History { get; set; } = new();
}

public class TrackingEventDto
{
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
}


// ============================================================
// REJECT / MARK READ DTOs
// ============================================================

public class RejectPortalInvoiceDto
{
    public string? Reason { get; set; }
}

public class MarkAllPortalNotificationsReadDto
{
    public Guid VendorId { get; set; }
}


