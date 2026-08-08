// Models/DTOs/CostCenterDto.cs
using System;

namespace Cor.Finance.Models.DTOs;

public class CostCenterDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? BudgetHolder { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddCostCenterDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? DepartmentId { get; set; }
    public string? BudgetHolder { get; set; }
    public Guid? ParentId { get; set; }
}

public class EditCostCenterDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? BudgetHolder { get; set; }
    public Guid? ParentId { get; set; }
}
public class InternalOrderDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? ResponsiblePerson { get; set; }
    public string? ProjectManager { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public Guid? PeriodId { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddInternalOrderDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public decimal BudgetAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? ResponsiblePerson { get; set; }
    public string? ProjectManager { get; set; }
    public Guid? CostCenterId { get; set; }
    public Guid? PeriodId { get; set; }
}

public class EditInternalOrderDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? ResponsiblePerson { get; set; }
    public string? ProjectManager { get; set; }
    public Guid? CostCenterId { get; set; }
    public Guid? PeriodId { get; set; }
    public string? RowVersion { get; set; }
}