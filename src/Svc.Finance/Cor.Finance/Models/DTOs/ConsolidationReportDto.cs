// Models/DTOs/ConsolidationReportDto.cs
namespace Cor.Finance.Models.DTOs;

public class ConsolidationReportDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Period { get; set; }
    public Guid? ConsolidationGroupId { get; set; }
    public string? ConsolidationGroupName { get; set; }
    public string? Format { get; set; }
    public string? Status { get; set; }
    public DateTime? GeneratedDate { get; set; }
    public string? GeneratedBy { get; set; }
    public string? FileSize { get; set; }
    public string? FilePath { get; set; }
    public string? Summary { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal NetIncome { get; set; }
    public int Adjustments { get; set; }
    public int Eliminations { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
}

public class GenerateConsolidationReportDto
{
    public Guid? ConsolidationGroupId { get; set; }
    public string? Period { get; set; }
    public string? Format { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IncludeEliminations { get; set; }
    public bool IncludeAdjustments { get; set; }
}