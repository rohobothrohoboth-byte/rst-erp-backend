namespace Cor.Inventory.Models.DTOs;

public class MaterialAssignmentDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime IssuedDate { get; set; }
    public string Status { get; set; } = "Issued";
    public DateTime? ReturnedDate { get; set; }
    public Guid? RequestId { get; set; }
    public string? Note { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateMaterialAssignmentDto
{
    public Guid EmployeeId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Note { get; set; }
}
