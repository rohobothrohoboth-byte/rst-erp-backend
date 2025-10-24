using System.ComponentModel.DataAnnotations;

namespace Cor.Module.Models.Enums;

public enum YesNo
{
    [Display(Name = "Yes")]
    Yes,
    [Display(Name = "No")]
    No
}

public enum BranchType
{
    [Display(Name = "Head Office")]
    HeadOff,
    [Display(Name = "Regional")]
    RegOff,
    [Display(Name = "Local")]
    LocOff,
    [Display(Name = "Virtual")]
    VirOff
}

public enum BranchStat
{
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Inactive")]
    InAct,
    [Display(Name = "Under construction")]
    UndCon
}

public enum DeptStat
{
    [Display(Name = "Active")]
    Active,
    [Display(Name = "Inactive")]
    InAct
}

public enum Quarter
{
    [Display(Name = "1st Quarter")]
    Q1,
    [Display(Name = "2nd Quarter")]
    Q2,
    [Display(Name = "3rd Quarter")]
    Q3,
    [Display(Name = "4th Quarter")]
    Q4
}