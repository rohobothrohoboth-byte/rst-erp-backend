namespace Cor.Procurement.Models.DTOs;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? WarehouseType { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime? SyncedAt { get; set; }
    public Guid? SourceId { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}