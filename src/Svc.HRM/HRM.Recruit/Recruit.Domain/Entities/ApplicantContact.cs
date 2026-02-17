namespace Recruit.Domain.Entities;

public class ApplicantContact : BaseEntity
{
    public string Phone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AlternatePhone { get; set; }
}