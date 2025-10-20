namespace Cor.HRMM.Models.Entities;

public class BenefitSetting : BaseEntity
{
    public string Name { get; set; } = default!;
    public double BenefitValue { get; set; } = default!;
    public string Per { get; set; } = default!;
}