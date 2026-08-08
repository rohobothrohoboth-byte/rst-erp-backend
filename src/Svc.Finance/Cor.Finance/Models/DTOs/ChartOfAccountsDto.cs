// Models/DTOs/ChartOfAccountsDto.cs
using System;

namespace Cor.Finance.Models.DTOs;

// ============================================================
// MAIN CHART OF ACCOUNTS DTO
// ============================================================

// ============================================================
// MAIN CHART OF ACCOUNTS DTO - FIXED ✅
// ============================================================

public class ChartOfAccountsDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string? AccountSubType { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; }
    public decimal? OpeningBalance { get; set; }
    public decimal? CurrentBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }

    // ✅ ADD THIS
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }  // For display

    // Asset specific fields
    public int? UsefulLife { get; set; }
    public decimal? SalvageValue { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? AssignedTo { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}

// ============================================================
// ADD CHART OF ACCOUNTS DTO
// ============================================================

public class AddChartOfAccountsDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string? AccountSubType { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; } = 1;
    public decimal? OpeningBalance { get; set; }
    public decimal? CurrentBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentId { get; set; }
    public Guid? PeriodId { get; set; }

    // ✅ ADD THIS - CategoryId is required for linking to AccountCategories
    public Guid? CategoryId { get; set; }

    // Asset specific fields
    public int? UsefulLife { get; set; }
    public decimal? SalvageValue { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? AssignedTo { get; set; }
    public Guid? DepartmentId { get; set; }
}

// ============================================================
// EDIT CHART OF ACCOUNTS DTO
// ============================================================

// ============================================================
// EDIT CHART OF ACCOUNTS DTO - FIXED ✅
// ============================================================

public class EditChartOfAccountsDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string? AccountSubType { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; }
    public decimal? OpeningBalance { get; set; }
    public decimal? CurrentBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public string? RowVersion { get; set; }
    public Guid? PeriodId { get; set; }

    // ✅ ADD THIS
    public Guid? CategoryId { get; set; }

    // Asset specific fields
    public int? UsefulLife { get; set; }
    public decimal? SalvageValue { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? AssignedTo { get; set; }
    public Guid? DepartmentId { get; set; }
}

// ============================================================
// CHART OF ACCOUNTS HIERARCHY DTO
// ============================================================

public class ChartOfAccountsHierarchyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? AccountType { get; set; }
    public string? AccountSubType { get; set; }
    public string? Description { get; set; }
    public int? Level { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentId { get; set; }
    public decimal? OpeningBalance { get; set; }
    public decimal? CurrentBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }

    // ✅ ADD THESE
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public List<ChartOfAccountsHierarchyDto> Children { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

// ============================================================
// CHART OF ACCOUNTS USAGE DTO
// ============================================================

public class ChartOfAccountsUsageDto
{
    public Guid ChartOfAccountsId { get; set; }
    public string? AccountId { get; set; } //
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCode { get; set; }
    public int JournalLineCount { get; set; }
    public int TransactionCount { get; set; }
    public int JournalEntryCount { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanBeDeleted { get; set; }
    public bool HasChildren { get; set; }
    public string? Reason { get; set; }
}

// ============================================================
// CAN DELETE RESULT DTO
// ============================================================

public class CanDeleteResultDto
{
    public bool CanDelete { get; set; }
    public string? Reason { get; set; }
    public int ChartOfAccountsCount { get; set; }
}

public class CanDeleteResponseDto
{
    public bool CanDelete { get; set; }
    public string? Reason { get; set; }
    public int Count { get; set; }
}

// ============================================================
// TOGGLE ACTIVE RESULT DTO
// ============================================================


// ============================================================
// BULK DELETE DTOs
// ============================================================



// ============================================================
// PAGINATED RESPONSE DTO
// ============================================================

