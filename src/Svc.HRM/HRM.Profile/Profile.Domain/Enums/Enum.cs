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