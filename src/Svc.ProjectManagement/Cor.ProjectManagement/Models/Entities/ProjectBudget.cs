// Models/Entities/ProjectBudget.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.ProjectManagement.Models.Entities
{
    public enum BudgetCategory
    {
        Labor = 1,
        Materials = 2,
        Equipment = 3,
        Software = 4,
        Travel = 5,
        Training = 6,
        Consulting = 7,
        Overhead = 8,
        Contingency = 9,
        Other = 10
    }

    public class ProjectBudget : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public BudgetCategory Category { get; set; }

        [MaxLength(200)]
        public string CategoryName { get; set; } = string.Empty;

        public decimal PlannedAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }

        public string? VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;

        public string? PurchaseOrderId { get; set; }
        public string? InvoiceId { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string? CustomFields { get; set; }

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
    }
}