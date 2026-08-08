// Models/DTOs/ProfitCenterDto.cs
namespace Cor.Finance.Models.DTOs;

public class ProfitCenterDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? Manager { get; set; }
    public string? Region { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class AddProfitCenterDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Manager { get; set; }
    public string? Region { get; set; }
    public Guid? ParentId { get; set; }
}

public class EditProfitCenterDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? Manager { get; set; }
    public string? Region { get; set; }
    public Guid? ParentId { get; set; }
    public string? RowVersion { get; set; }
}