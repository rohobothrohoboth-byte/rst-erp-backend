// Models/DTOs/BudgetCodeDto.cs
using System;

namespace Cor.Finance.Models.DTOs;

public class BudgetCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string BudgetType { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddBudgetCodeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string BudgetType { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public bool IsActive { get; set; } = true;
}

public class EditBudgetCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public string BudgetType { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public bool IsActive { get; set; }
}