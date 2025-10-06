namespace Cor.HRMM.Models.DTOs;

public class PositionEduListDto : BaseDto
{
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQualification
    public Guid EducationLevelId { get; set; } = default!; // lup.EducationLevel
    public string Position { get; set; } = default!;
    public string PositionAm { get; set; } = default!;
    public string EducationQual { get; set; } = default!;
    public string EducationLevel { get; set; } = default!;
}

public class PositionEduAddDto
{
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQualification
    public Guid EducationLevelId { get; set; } = default!; // lup.EducationLevel
}

public class PositionEduModDto
{
    public Guid Id { get; set; }
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQualification
    public Guid EducationLevelId { get; set; } = default!; // lup.EducationLevel
    public string RowVersion { get; set; } = default!;
}