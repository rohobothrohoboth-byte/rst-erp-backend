namespace Recruit.Domain.Entities;

public class EvaluationType : BaseEntity
{
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}