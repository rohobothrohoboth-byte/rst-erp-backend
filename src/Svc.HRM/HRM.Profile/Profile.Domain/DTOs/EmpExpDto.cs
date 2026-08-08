using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public sealed class EmpExpListDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string Company { get; set; } = default!;
    public string PosTitle { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string Respo { get; set; } = "";
    public string Status { get; set; } = default!;

    // ✅ These must be DateTime, not string
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // ✅ These are computed from the DateTime properties above
    public string DateStart => StartDate.ToString("MMMM dd, yyyy");
    public string DateEnd => EndDate?.ToString("MMMM dd, yyyy") ?? "";
}

public sealed class EmpExpAddDto
{
    [JsonIgnore]
    public Guid EmpId { get; set; } = default!;

    public string Company { get; set; } = default!;
    public string PosTitle { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Respo { get; set; } = "";
}

public sealed class EmpExpModDto
{
    public Guid Id { get; set; }
    public string Company { get; set; } = default!;
    public string PosTitle { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Respo { get; set; } = "";
     public string Status { get; set; } = string.Empty;
    public string RowVersion { get; set; } = default!;
}