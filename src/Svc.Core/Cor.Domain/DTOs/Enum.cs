using System.ComponentModel.DataAnnotations;

namespace Shared.Api.Cor.DTOs;

public enum YesNoAm
{
    [Display(Name = "አዎ")]
    Yes,
    [Display(Name = "አይ")]
    No
}

public enum YesNo
{
    Yes,
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