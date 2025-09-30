namespace Cor.HRMM.Models.DTOs;

public class PositionReqListDto : BaseDto
{
    public Guid ProfessionTypeId { get; set; } = default!; //Lup.ProfessionType
    public Guid PositionId { get; set; } = default!; // Position
    public string Gender { get; set; } = default!; //enum.PositionGender
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption
    public decimal WorkingHours { get; set; } = default!;
    public string ProfessionType { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string PositionAm { get; set; } = default!;
}

public class PositionReqAddDto
{
    public string Gender { get; set; } = default!; //enum.PositionGender
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption
    public decimal WorkingHours { get; set; } = default!;
    public Guid ProfessionTypeId { get; set; } = default!; //Lup.ProfessionType
    public Guid PositionId { get; set; } = default!; // Position
}

public class PositionReqModDto
{
    public Guid Id { get; set; }
    public string Gender { get; set; } = default!; //enum.PositionGender
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption
    public decimal WorkingHours { get; set; } = default!;
    public Guid ProfessionTypeId { get; set; } = default!; //Lup.ProfessionType
    public Guid PositionId { get; set; } = default!; // Position
    public string RowVersion { get; set; } = default!;
}