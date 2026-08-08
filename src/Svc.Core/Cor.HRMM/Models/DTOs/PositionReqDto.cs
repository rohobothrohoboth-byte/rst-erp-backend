namespace Cor.HRMM.Models.DTOs;

public class PositionReqListDto : BaseDto
{
    public Guid PositionId { get; set; } = default!; // Position
    public string Gender { get; set; } = default!; //enum.PositionGender (0/1)
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption (0/1)
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption (0/1)
    public string ProfessionType { get; set; } = default!; //enum.ProfessionType (0/1)
    public double WorkingHours { get; set; } = default!;

    public string GenderStr { get; set; } = default!;
    public string SaturdayWorkOptionStr { get; set; } = default!;
    public string SundayWorkOptionStr { get; set; } = default!;
    public string ProfessionTypeStr { get; set; } = default!;
}

public class PositionReqAddDto
{
    public string Gender { get; set; } = default!; //enum.PositionGender (0/1)
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption (0/1)
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption (0/1)
    public double WorkingHours { get; set; } = default!;
    public string ProfessionType { get; set; } = default!; //enum.ProfessionType (0/1)
    public Guid PositionId { get; set; } = default!; // Position
}

public class PositionReqModDto
{
    public Guid Id { get; set; }
    public string Gender { get; set; } = default!; //enum.PositionGender (0/1)
    public string SaturdayWorkOption { get; set; } = default!; //enum.WorkOption (0/1)
    public string SundayWorkOption { get; set; } = default!; //enum.WorkOption (0/1)
    public double WorkingHours { get; set; } = default!;
    public string ProfessionType { get; set; } = default!; //enum.ProfessionType (0/1)
    public Guid PositionId { get; set; } = default!; // Position
    public string RowVersion { get; set; } = default!;
}
