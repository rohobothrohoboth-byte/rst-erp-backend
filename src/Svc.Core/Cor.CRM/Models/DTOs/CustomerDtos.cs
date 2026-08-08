// Cor.CRM/Models/DTOs/CustomerDto.cs

namespace Cor.CRM.Models.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public int EmployeeCount { get; set; }
     public bool IsActive { get; set; }
    public string? Website { get; set; }
    public string? Tags { get; set; }
    public decimal? LifetimeValue { get; set; }
    public int TotalOrders { get; set; }
    public int ContactCount { get; set; }
    public int OpportunityCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Type { get; set; }
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public int? EmployeeCount { get; set; }
    public string? Website { get; set; }
    public string? Tags { get; set; }
}

public class UpdateCustomerDto
{
    public string? Name { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public int? EmployeeCount { get; set; }
    public string? Website { get; set; }
    public string? Tags { get; set; }
    public bool? IsActive { get; set; }
}
public class CustomerFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Industry { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}