namespace Cor.HRMM.Models.DTOs;

public class PositionBenefitListDto : BaseDto
{
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position
    public string Position { get; set; } = default!;
    public string PositionAm { get; set; } = default!;
    public string BenefitName { get; set; } = default!;
    public string PerStr { get; set; } = default!;
    public string Benefit { get; set; } = default!;
}

public class PositionBenefitAddDto
{
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position
}

public class PositionBenefitModDto
{
    public Guid Id { get; set; }
    public Guid BenefitSettingId { get; set; } = default!; //BenefitSetting
    public Guid PositionId { get; set; } = default!; //Position
    public string RowVersion { get; set; } = default!;
}