// Models/DTOs/ProjectBudgetDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectBudgetDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public BudgetCategory Category { get; set; }
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
        public string ApprovedByName { get; set; } = string.Empty;
        public decimal UtilizationPercentage => PlannedAmount > 0 ? (ActualAmount / PlannedAmount) * 100 : 0;
    }

    public class ProjectBudgetCreateDto
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public BudgetCategory Category { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public decimal PlannedAmount { get; set; }

        public DateTime? PlannedDate { get; set; }

        public string? VendorId { get; set; }
        public string? VendorName { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? CreatedBy { get; set; }
    }

    public class ProjectBudgetUpdateDto
    {
        public decimal? PlannedAmount { get; set; }
        public decimal? ActualAmount { get; set; }
        public decimal? CommittedAmount { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public bool? IsApproved { get; set; }
        public string? Description { get; set; }
        public string? UpdatedBy { get; set; }
    }



      public class BudgetSummaryDto
        {
            public Guid ProjectId { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public decimal TotalBudget { get; set; }
            public decimal TotalActual { get; set; }
            public decimal TotalCommitted { get; set; }
            public decimal TotalRemaining { get; set; }
            public decimal ApprovedBudget { get; set; }
            public decimal PendingApproval { get; set; }
            public double UtilizationPercentage { get; set; }

            // ✅ Use BudgetCategory enum as key
            public Dictionary<BudgetCategory, decimal> BudgetByCategory { get; set; } = new();
            public Dictionary<BudgetCategory, decimal> ActualByCategory { get; set; } = new();
        }
     public class BudgetUtilizationDto
        {
            public Guid ProjectId { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public decimal TotalBudget { get; set; }
            public decimal TotalUtilized { get; set; }
            public decimal TotalCommitted { get; set; }
            public double OverallUtilization { get; set; }
              public decimal ApprovedBudget { get; set; }
                    public decimal PendingApproval { get; set; }
            public List<MonthlyBudgetData> MonthlyData { get; set; } = new();
            public List<CategoryUtilizationData> CategoryUtilization { get; set; } = new();
        }
         public class MonthlyBudgetData
            {
                public string Month { get; set; } = string.Empty;
                public decimal Planned { get; set; }
                public decimal Actual { get; set; }
                public double Utilization { get; set; }
            }

            public class CategoryUtilizationData
            {
                public string Category { get; set; } = string.Empty;
                public decimal Planned { get; set; }
                public decimal Actual { get; set; }
                public double Utilization { get; set; }
            }
}