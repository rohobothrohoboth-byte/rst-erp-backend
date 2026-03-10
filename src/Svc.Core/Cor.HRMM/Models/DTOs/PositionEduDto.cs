namespace Cor.HRMM.Models.DTOs;

public class PositionEduListDto : BaseDto
{
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQualification
    public string EducationLevel { get; set; } = default!; // enum.EducationLevel
    public string EducationQual { get; set; } = default!;
    public string EducationLevelStr { get; set; } = default!;
}

public class PositionEduAddDto
{
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQualification
    public string EducationLevel { get; set; } = default!; // enum.EducationLevel
}

public class PositionEduModDto
{
    public Guid Id { get; set; }
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQualification
    public string EducationLevel { get; set; } = default!; // enum.EducationLevel
    public string RowVersion { get; set; } = default!;
}