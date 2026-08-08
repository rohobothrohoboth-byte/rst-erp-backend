// Models/DTOs/AssetDto.cs
using System;
using System.Collections.Generic;

namespace Cor.Finance.Models.DTOs
{
    public class AssetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? Location { get; set; }
        public decimal AcquisitionCost { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal? SalvageValue { get; set; }
        public int? UsefulLife { get; set; }
        public decimal? DepreciationRate { get; set; }
        public string? AssetType { get; set; }
        public string? AssetCategory { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid? AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? CurrentValue { get; set; }
        public decimal? AccumulatedDepreciation { get; set; }
        public DateTime? LastDepreciationDate { get; set; }
        public string? WarrantyInfo { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime? DateMod { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? UpdatedByUserName { get; set; }
    }

    public class CreateAssetDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? Location { get; set; }
        public decimal AcquisitionCost { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal? SalvageValue { get; set; }
        public int? UsefulLife { get; set; }
        public decimal? DepreciationRate { get; set; }
        public string? AssetType { get; set; }
        public string? AssetCategory { get; set; }
        public string? Status { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? WarrantyInfo { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAssetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? Location { get; set; }
        public decimal AcquisitionCost { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal? SalvageValue { get; set; }
        public int? UsefulLife { get; set; }
        public decimal? DepreciationRate { get; set; }
        public string? AssetType { get; set; }
        public string? AssetCategory { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? CurrentValue { get; set; }
        public decimal? AccumulatedDepreciation { get; set; }
        public DateTime? LastDepreciationDate { get; set; }
        public string? WarrantyInfo { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class AssetFilterDto
    {
        public string? Status { get; set; }
        public string? AssetType { get; set; }
        public string? AssetCategory { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? AssignedTo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinCost { get; set; }
        public decimal? MaxCost { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } = "AcquisitionDate";
        public string? SortDirection { get; set; } = "DESC";
    }
}