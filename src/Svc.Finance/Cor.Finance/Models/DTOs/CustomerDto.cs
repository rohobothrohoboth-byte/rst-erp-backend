// Models/DTOs/CustomerDto.cs
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Cor.Finance.Models.DTOs;

public class CustomerContactPersonDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
}

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string CustomerType { get; set; } = "Company";
    public string Status { get; set; } = "Active";
    public string? PaymentTerms { get; set; } = "Net 30";
    public string? Currency { get; set; } = "USD";
    public decimal? CreditLimit { get; set; }
    public string? SalesRep { get; set; }
    public CustomerContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class CustomerCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string CustomerType { get; set; } = "Company";
    public string Status { get; set; } = "Active";
    public string? PaymentTerms { get; set; } = "Net 30";
    public string? Currency { get; set; } = "USD";
    public decimal? CreditLimit { get; set; }
    public string? SalesRep { get; set; }
    public CustomerContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CustomerUpdateDto
{
    public Guid Id { get; set; } // ✅ Make required (not nullable)
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string CustomerType { get; set; } = "Company";
    public string Status { get; set; } = "Active";
    public string? PaymentTerms { get; set; } = "Net 30";
    public string? Currency { get; set; } = "USD";
    public decimal? CreditLimit { get; set; }
    public string? SalesRep { get; set; }
    public CustomerContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RowVersion { get; set; }
}

// ✅ ADD THIS - For backward compatibility with controller
public class CustomerCreateUpdateDto
{
    public Guid? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAm { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? TaxId { get; set; }
    public string CustomerType { get; set; } = "Company";
    public string Status { get; set; } = "Active";
    public string? PaymentTerms { get; set; } = "Net 30";
    public string? Currency { get; set; } = "USD";
    public decimal? CreditLimit { get; set; }
    public string? SalesRep { get; set; }
    public CustomerContactPersonDto? ContactPerson { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RowVersion { get; set; }
}

public class BulkOperationResultDto
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkOperationErrorDto> Errors { get; set; } = new();
}

public class BulkOperationErrorDto
{
    public int RowIndex { get; set; }
    public string? Code { get; set; }
    public string? ErrorMessage { get; set; }
}