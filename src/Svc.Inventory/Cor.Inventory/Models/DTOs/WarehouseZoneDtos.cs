namespace Cor.Inventory.Models.DTOs;

public class WarehouseZoneDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? ZoneType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateWarehouseZoneDto
{
    public Guid WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? ZoneType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateWarehouseZoneDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? ZoneType { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public string? RowVersion { get; set; }
}

public class BinDto
{
    public Guid Id { get; set; }
    public Guid ZoneId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateBinDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; } = true;
}

public class WarehouseLayoutZoneDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? ZoneType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<BinDto> Bins { get; set; } = new();
}

public class WarehouseLayoutDto
{
    public Guid WarehouseId { get; set; }
    public List<WarehouseLayoutZoneDto> Zones { get; set; } = new();
}
