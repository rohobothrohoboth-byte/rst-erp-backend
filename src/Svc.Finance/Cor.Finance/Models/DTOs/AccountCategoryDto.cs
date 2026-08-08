// Models/DTOs/AccountCategoryDto.cs
using System;

namespace Cor.Finance.Models.DTOs;

// ============================================================
// ACCOUNT CATEGORY DTOs
// ============================================================

public class AccountCategoryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddAccountCategoryDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? ParentId { get; set; }
}

public class EditAccountCategoryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
}

// ============================================================
// ACCOUNT CATEGORY HIERARCHY DTO
// ============================================================

public class AccountCategoryHierarchyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<AccountCategoryHierarchyDto> Children { get; set; } = new();
}

// ============================================================
// CATEGORY USAGE DTO
// ============================================================

public class CategoryUsageDto
{
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int AccountCount { get; set; }
    public bool CanDelete { get; set; }
    public string? Reason { get; set; }
}

// ============================================================
// BULK DELETE DTO
// ============================================================

public class BulkDeleteDto
{
    public List<Guid> Ids { get; set; } = new();
}

// ============================================================
// CAN DELETE RESPONSE DTO
// ============================================================

public class CanDeleteResponse
{
    public bool CanDelete { get; set; }
    public string? Reason { get; set; }
    public int AccountCount { get; set; }
}

// ============================================================
// BULK DELETE RESULT DTO
// ============================================================

public class BulkDeleteResultDto
{
    public int DeletedCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkDeleteErrorDto> Errors { get; set; } = new(); // Changed to List<BulkDeleteErrorDto>
}

public class BulkDeleteErrorDto
{
    public Guid Id { get; set; }
    public string? Error { get; set; }
}

// ============================================================
// TOGGLE ACTIVE RESULT DTO
// ============================================================

public class ToggleActiveResultDto
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}