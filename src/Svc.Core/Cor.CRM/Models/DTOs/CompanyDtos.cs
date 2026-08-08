// Cor.CRM/Models/DTOs/CompanyDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? Size { get; set; }
    public string? Status { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Description { get; set; }
    public int? FoundedYear { get; set; }
    public decimal? Revenue { get; set; }
    public int? EmployeeCount { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public int ContactCount { get; set; }
    public int LeadCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? Size { get; set; }
    public string? Status { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Description { get; set; }
    public int? FoundedYear { get; set; }
    public decimal? Revenue { get; set; }
    public int? EmployeeCount { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
}

public class UpdateCompanyDto
{
    public string? Name { get; set; }
    public string? LegalName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? Size { get; set; }
    public string? Status { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Description { get; set; }
    public int? FoundedYear { get; set; }
    public decimal? Revenue { get; set; }
    public int? EmployeeCount { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public bool? IsActive { get; set; }
}

public class CompanyFilterDto
{
    public string? Search { get; set; }
    public string? Industry { get; set; }
    public string? Status { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class CompanyStatsDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public Dictionary<string, int> ByIndustry { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
}