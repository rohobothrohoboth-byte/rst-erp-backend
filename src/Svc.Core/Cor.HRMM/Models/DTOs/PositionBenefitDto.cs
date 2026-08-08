namespace Cor.HRMM.Models.DTOs;

public class PosBenefitListDto : BaseDto
{
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position
    public string BenefitName { get; set; } = default!;
    public string PerStr { get; set; } = default!;
    public string Benefit { get; set; } = default!;
    public double BenefitValue { get; set; } = default!;
    public string Per { get; set; } = default!;
}

public class PosBenefitAddDto
{
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position
}

public class PosBenefitModDto
{
    public Guid Id { get; set; }
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position
    public string RowVersion { get; set; } = default!;
}
