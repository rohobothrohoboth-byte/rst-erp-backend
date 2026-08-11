namespace Cor.Inventory.Models.DTOs;

public class UnitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateUnitDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}
