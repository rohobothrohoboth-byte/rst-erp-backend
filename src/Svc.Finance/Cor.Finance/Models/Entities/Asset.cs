// Models/Entities/Asset.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Finance.Models.Entities.Local;
namespace Cor.Finance.Models.Entities;


    [Table("Assets")]
    public class Asset : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(100)]
        public string? Manufacturer { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AcquisitionCost { get; set; }

        public DateTime AcquisitionDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalvageValue { get; set; }

        public int? UsefulLife { get; set; } // In months

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DepreciationRate { get; set; }

        [MaxLength(50)]
        public string? AssetType { get; set; } // Fixed, Current, Intangible

        [MaxLength(50)]
        public string? AssetCategory { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Disposed, UnderMaintenance

        public bool IsActive { get; set; } = true;

        public Guid? AssignedTo { get; set; } // EmployeeId

        public Guid? DepartmentId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? AccountId { get; set; } // Chart of Accounts reference

        public DateTime? PurchaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AccumulatedDepreciation { get; set; }

        public DateTime? LastDepreciationDate { get; set; }

        [MaxLength(500)]
        public string? WarrantyInfo { get; set; }

        public DateTime? WarrantyExpiryDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(AssignedTo))]
        public virtual LocalEmployee? AssignedEmployee { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual LocalDepartment? Department { get; set; }

        [ForeignKey(nameof(BranchId))]
        public virtual LocalBranch? Branch { get; set; }

        [ForeignKey(nameof(AccountId))]
        public virtual ChartOfAccounts? Account { get; set; }
    }
