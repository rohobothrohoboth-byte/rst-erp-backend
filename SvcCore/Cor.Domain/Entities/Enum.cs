using System.ComponentModel.DataAnnotations;

namespace Cor.Domain.Entities
{
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
}