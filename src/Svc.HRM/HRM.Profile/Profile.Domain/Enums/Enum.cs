using System.ComponentModel.DataAnnotations;

namespace Profile.Domain.Enums;

public enum YesNo
{
    [Display(Name = "Yes")]
    Yes,
    [Display(Name = "No")]
    No
}

public enum Gender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female
}

public enum EmpType
{
    [Display(Name = "Replacement")]
    Rep,
    [Display(Name = "New Opening")]
    NewOp,
    [Display(Name = "Additional Required")]
    AddReq,
    [Display(Name = "Old Employee")]
    Old
}

public enum EmpNature
{
    [Display(Name = "Permanent / Full-time")]
    Per,
    [Display(Name = "Contract / Fixed-term")]
    Con,
    [Display(Name = "Probation")]
    Pro,
    [Display(Name = "Intern / Trainee")]
    Inte,
    [Display(Name = "Part-time / Casual")]
    Par
}

public enum WorkArrangement
{
    [Display(Name = "On-site")]
    OnSite,
    [Display(Name = "Remote")]
    Remote,
    [Display(Name = "Hybrid")]
    Hybrid,
    [Display(Name = "Shift-based")]
    ShiftB,
    [Display(Name = "Rotational / Roster-based")]
    Rota
}

public enum MaritalStat
{
    [Display(Name = "Single / Not Married")]
    NotMar,
    [Display(Name = "Married")]
    Mar,
    [Display(Name = "Widow/er")]
    Wid,
    [Display(Name = "Divorced")]
    Div,
    [Display(Name = "Not Mentioned")]
    NotMen
}

public enum AddressType
{
    [Display(Name = "Residence")]
    Res,
    [Display(Name = "Work Place")]
    Work
}