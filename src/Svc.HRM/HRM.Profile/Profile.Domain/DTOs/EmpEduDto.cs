using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public sealed class EmpEduListDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string EduLevel { get; set; } = default!;
    public string Institution { get; set; } = default!;
    public string FieldOfStudy { get; set; } = default!;
    public double? GPA { get; set; }
    public string Status { get; set; } = default!;

    // ✅ These must be DateTime, not string
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // ✅ These are computed from the DateTime properties above
    public string DateStart => StartDate.ToString("MMMM dd, yyyy");
    public string DateEnd => EndDate.ToString("MMMM dd, yyyy");
}

public sealed class EmpEduAddDto
{
    [JsonIgnore]
    public Guid EmpId { get; set; } = default!;

    public string EduLevel { get; set; } = default!; //enum.EducationLevel
    public string Institution { get; set; } = default!;
    public string FieldOfStudy { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double? GPA { get; set; }
}

public sealed class EmpEduModDto
{
    public Guid Id { get; set; }
    public string EduLevel { get; set; } = default!; //enum.EducationLevel
    public string Institution { get; set; } = default!;
    public string FieldOfStudy { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double? GPA { get; set; }
     public string Status { get; set; } = string.Empty;
    public string RowVersion { get; set; } = default!;
}