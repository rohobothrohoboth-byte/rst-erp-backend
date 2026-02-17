namespace Recruit.Domain.Entities;

public class JobDec : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Desc { get; set; } = default!;
    public string Qualification { get; set; } = default!;
    public string KeySkills { get; set; } = default!;
    public string WorkLocation { get; set; } = default!;
    public string PreGender { get; set; } = default!; // enum.Gender
    public string ContractType { get; set; } = default!; // enum.EmpNature
}