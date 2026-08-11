namespace Cor.Inventory.Models.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}
