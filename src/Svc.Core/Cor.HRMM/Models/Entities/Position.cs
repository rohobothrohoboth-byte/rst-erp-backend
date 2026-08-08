namespace Cor.HRMM.Models.Entities;

public class Position: BaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int NoOfPosition { get; set; } = default!;
    public string IsVacant { get; set; } = default!; //YesNo
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public PositionReq? PositionReq { get; set; }
    public PositionExp? PositionExp { get; set; }
    public ICollection<PositionEducation> PositionEducations { get; set; } = new List<PositionEducation>();
    public ICollection<PositionBenefit> PositionBenefits { get; set; } = new List<PositionBenefit>();

    public Guid? JobGradeId { get; set; }
    public JobGrade? JobGrade { get; set; }
}








