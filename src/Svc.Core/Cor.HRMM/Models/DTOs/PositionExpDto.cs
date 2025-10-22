namespace Cor.HRMM.Models.DTOs;

public class PositionExpListDto : BaseDto
{
    public Guid PositionId { get; set; } = default!; // Position
    public int SamePosExp { get; set; } = 0;
    public int OtherPosExp { get; set; } = 0;
    public int MinAge { get; set; } = default!;
    public int MaxAge { get; set; } = default!;
}

public class PositionExpAddDto
{
    public int SamePosExp { get; set; } = 0;
    public int OtherPosExp { get; set; } = 0;
    public int MinAge { get; set; } = default!;
    public int MaxAge { get; set; } = default!;
    public Guid PositionId { get; set; } = default!; // Position
}

public class PositionExpModDto
{
    public Guid Id { get; set; }
    public int SamePosExp { get; set; } = 0;
    public int OtherPosExp { get; set; } = 0;
    public int MinAge { get; set; } = default!;
    public int MaxAge { get; set; } = default!;
    public Guid PositionId { get; set; } = default!; // Position
    public string RowVersion { get; set; } = default!;
}