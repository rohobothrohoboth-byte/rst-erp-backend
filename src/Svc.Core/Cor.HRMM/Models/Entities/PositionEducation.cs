namespace Cor.HRMM.Models.Entities;

public class PositionEducation : BaseEntity
{
    public Guid PositionId { get; set; } = default!; // Position
    public Guid EducationQualId { get; set; } = default!; // EducationQual
    public Guid EducationLevelId { get; set; } = default!; // lup.EducationLevel

    //******************************************//

    public Position Position { get; set; } = null!;
    public EducationQual EducationQual { get; set; } = null!;
}
