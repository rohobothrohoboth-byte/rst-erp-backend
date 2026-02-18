namespace Recruit.Domain.Entities;

public class EvaluationType : BaseEntity
{
    public string Name { get; set; } = default!;
    public double MaxScore { get; set; }
}