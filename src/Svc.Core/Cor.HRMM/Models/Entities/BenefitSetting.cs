namespace Cor.HRMM.Models.Entities;

public class BenefitSetting : BaseEntity
{
    public string Name { get; set; } = default!;
    public decimal BenefitValue { get; set; } = default!;
}