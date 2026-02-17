namespace Recruit.Domain.DTOs;

public class CanContactListDto : BaseDto
{
    public string Phone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AlternatePhone { get; set; }
}

public class CanContactAddDto
{
    public string Phone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AlternatePhone { get; set; }
}

public class CanContactModDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AlternatePhone { get; set; }
    public string RowVersion { get; set; } = default!;
}