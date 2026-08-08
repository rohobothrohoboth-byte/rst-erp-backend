namespace Cor.HRMM.Models.Entities;

public class PositionBenefit : BaseEntity
{
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position

    //******************************************//
    public BenefitSetting BenefitSetting { get; set; } = null!;
    public Position Position { get; set; } = null!;
}
