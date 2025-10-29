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
    [Display(Name = "Permanent")]
    Per,
    [Display(Name = "Contract")]
    Con
}

public enum MaritalStat
{
    [Display(Name = "Not Married")]
    NotMar,
    [Display(Name = "Single")]
    Sin,
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