// Recruit.Domain/DTOs/PublicDto.cs

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http; // ✅ Add this for IFormFile

namespace Recruit.Domain.DTOs;

/// <summary>
/// DTO for external applicant registration
/// </summary>
public class ExternalApplicantRegistrationDto
{
    // Personal Information
    [JsonRequired]
    public string FirstName { get; set; } = default!;

    public string? FirstNameAm { get; set; }

    [JsonRequired]
    public string MiddleName { get; set; } = default!;

    public string? MiddleNameAm { get; set; }

    [JsonRequired]
    public string LastName { get; set; } = default!;

    public string? LastNameAm { get; set; }

    [JsonRequired]
    public string Gender { get; set; } = default!; // enum.Gender

    [JsonRequired]
    public string Nationality { get; set; } = default!;

    // Contact Information
    [JsonRequired]
    public string Email { get; set; } = default!;

    [JsonRequired]
    public string Phone { get; set; } = default!;

    public string? AlternatePhone { get; set; }
    public string? PoBox { get; set; }
    public string? Fax { get; set; }

    // Address Information
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? Zone { get; set; }
    public string? Woreda { get; set; }
    public string? Subcity { get; set; }
    public string? Kebele { get; set; }
    public string? HouseNo { get; set; }
    public string? AddressType { get; set; }
}

/// <summary>
/// DTO for external applicant registration response
/// </summary>
public class ExternalApplicantRegistrationResponseDto
{
    public Guid ApplicantId { get; set; }
    public Guid PersonId { get; set; }
    public Guid ContactId { get; set; }
    public Guid? AddressId { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Message { get; set; } = default!;
}

/// <summary>
/// DTO for external job application
/// </summary>
public class JobAppExtAddDto
{
    [JsonRequired]
    public Guid ApplicantId { get; set; }

    [JsonRequired]
    public Guid JobPostingId { get; set; }

    public string CoverLetter { get; set; } = default!;

    public IFormFile? File { get; set; } = default!; // ✅ Now works with using Microsoft.AspNetCore.Http
}

/// <summary>
/// DTO for applicant login/authentication
/// </summary>
public class ExternalApplicantLoginDto
{
    [JsonRequired]
    public string Email { get; set; } = default!;

    [JsonRequired]
    public string Phone { get; set; } = default!;
}

/// <summary>
/// DTO for checking if applicant exists
/// </summary>
public class ApplicantExistsDto
{
    public bool Exists { get; set; }
    public Guid? ApplicantId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// DTO for external applicant profile update
/// </summary>
public class ExternalApplicantUpdateDto
{
    public Guid ApplicantId { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? Subcity { get; set; }
}

/// <summary>
/// DTO for withdrawing an application
/// </summary>
public class WithdrawApplicationDto
{
    public string? Reason { get; set; }
}