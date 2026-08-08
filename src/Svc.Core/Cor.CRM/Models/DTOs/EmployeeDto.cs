// Cor.CRM/Models/DTOs/EmployeeDto.cs

namespace Cor.CRM.Models.DTOs;

public class EmployeeAssignmentDto
{
    public Guid Id { get; set; }
    public Guid? AppUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Code { get; set; }

    public string DisplayName => $"{FirstName} {LastName}".Trim();
}