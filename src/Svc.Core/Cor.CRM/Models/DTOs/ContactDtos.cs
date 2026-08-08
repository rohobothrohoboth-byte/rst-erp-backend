// Cor.CRM/Models/DTOs/ContactDto.cs

namespace Cor.CRM.Models.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Title { get; set; }
    public string? Department { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsDecisionMaker { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    // ? Add missing properties
    public bool AcceptsEmail { get; set; }
    public bool AcceptsSMS { get; set; }
    public bool AcceptsCalls { get; set; }
    public bool AcceptsMarketing { get; set; }
    public string? PreferredContactMethod { get; set; }
    public DateTime? LastContactDate { get; set; }
    public int ContactCount { get; set; }
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateContactDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Title { get; set; }
    public string? Department { get; set; }
    public Guid? CustomerId { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsDecisionMaker { get; set; }
    public string? Notes { get; set; }
    // ? Add missing properties
    public bool AcceptsEmail { get; set; } = true;
    public bool AcceptsSMS { get; set; } = true;
    public bool AcceptsCalls { get; set; } = true;
    public bool AcceptsMarketing { get; set; } = true;
    public string? PreferredContactMethod { get; set; }
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
}

public class UpdateContactDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Title { get; set; }
    public string? Department { get; set; }
    public Guid? CustomerId { get; set; }
    public bool? IsPrimary { get; set; }
    public bool? IsDecisionMaker { get; set; }
    public string? Notes { get; set; }
    // ? Add missing properties
    public bool? AcceptsEmail { get; set; }
    public bool? AcceptsSMS { get; set; }
    public bool? AcceptsCalls { get; set; }
    public bool? AcceptsMarketing { get; set; }
    public string? PreferredContactMethod { get; set; }
    public string? LinkedIn { get; set; }
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
    public bool? IsActive { get; set; }
}