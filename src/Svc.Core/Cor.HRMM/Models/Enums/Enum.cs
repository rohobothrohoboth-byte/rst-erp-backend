using System.ComponentModel.DataAnnotations;

namespace Cor.HRMM.Models.Enums;

public enum Gender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female
}

public enum PositionGender
{
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female,
    [Display(Name = "Both")]
    Both
}

public enum YesNo
{
    [Display(Name = "Yes")]
    Yes,
    [Display(Name = "No")]
    No
}

public enum WorkOption
{
    [Display(Name = "Morning")]
    Morning,
    [Display(Name = "Afternoon")]
    Afternoon,
    [Display(Name = "Both")]
    Both,
    [Display(Name = "None")]
    None
}

public enum Per
{
    [Display(Name = "Day")]
    Day,
    [Display(Name = "Month")]
    Month,
    [Display(Name = "Year")]
    Year
}
