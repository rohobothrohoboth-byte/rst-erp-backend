using System.ComponentModel.DataAnnotations;

namespace Module.Domain.Enums;

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