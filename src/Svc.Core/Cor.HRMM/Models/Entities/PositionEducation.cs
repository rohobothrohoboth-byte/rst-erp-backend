namespace Cor.HRMM.Models.Entities;

public class PositionEducation : BaseEntity
{
    public string EducationLevel { get; set; } = default!; // enum.EducationLevel
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQual

    //******************************************//

    public Position Position { get; set; } = null!;
    public EducationQual EducationQual { get; set; } = null!;
}

