namespace Recruit.Domain.Entities;

public class JobDec : BaseEntity
{
    public string Desc { get; set; } = default!;
    public string KeyRespo { get; set; } = default!;
    public string ReqQual { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;  
    public string PreGender { get; set; } = default!; // enum.PositionGender
    public string EmpNature { get; set; } = default!; // enum.EmpNature
    public string WorkArr { get; set; } = default!; // enum.WorkArrangement
}