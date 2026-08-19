// Cor.Finance/Models/DTOs/AccountTypeDto.cs
using System;

namespace Cor.Finance.Models.DTOs;

// ============================================================
// ACCOUNT TYPE DTOs
// ============================================================

public class AccountTypeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string NormalBalance { get; set; } = "Debit";
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public int SubtypeCount { get; set; }
}

public class CreateAccountTypeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string NormalBalance { get; set; } = "Debit";
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}

public class UpdateAccountTypeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string NormalBalance { get; set; } = "Debit";
    public bool IsActive { get; set; }
    public int SortOrder { get; set; } = 0;
}

// ============================================================
// ACCOUNT SUBTYPE DTOs
// ============================================================

public class AccountSubtypeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public Guid AccountTypeId { get; set; }
    public string? AccountTypeName { get; set; }
    public string? AccountTypeCode { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateAccountSubtypeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public Guid AccountTypeId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}

public class UpdateAccountSubtypeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public Guid AccountTypeId { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; } = 0;
}

// ============================================================
// ACCOUNT TYPE WITH SUBTYPES DTO
// ============================================================

public class AccountTypeWithSubtypesDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string NormalBalance { get; set; } = "Debit";
    public bool IsActive { get; set; }
    public List<AccountSubtypeDto> Subtypes { get; set; } = new();
}

// ============================================================
// BULK DELETE DTO
// ============================================================

public class BulkDeleteAccountTypesDto
{
    public List<Guid> Ids { get; set; } = new();
}

// ============================================================
// TOGGLE ACTIVE RESULT DTO
// ============================================================

