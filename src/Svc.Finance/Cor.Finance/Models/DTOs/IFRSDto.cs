// Models/DTOs/IFRSDto.cs
namespace Cor.Finance.Models.DTOs;

public class IFRSReportDto
{
    public Guid Id { get; set; }
    public string Standard { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Period { get; set; }
    public DateTime? ReportDate { get; set; }
    public string? Status { get; set; }
    public string? Format { get; set; }
    public string? GeneratedBy { get; set; }
    public string? FilePath { get; set; }
    public string? FileSize { get; set; }
    public string? Summary { get; set; }
    public List<IFRSMetricDto>? Metrics { get; set; }
    public DateTime DateAdd { get; set; }
}

public class IFRSMetricDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? PreviousValue { get; set; }
    public decimal? Change { get; set; }
    public decimal? ChangePercentage { get; set; }
    public string? Status { get; set; } // Positive, Negative, Neutral
    public string? Unit { get; set; }
}

public class GenerateIFRSReportDto
{
    public string Standard { get; set; } = string.Empty;
    public string? Period { get; set; }
    public string? Format { get; set; } // PDF, Excel, HTML
    public bool IncludeMetrics { get; set; } = true;
    public bool IncludeNotes { get; set; } = true;
}

public class ScheduleIFRSReportDto
{
    public string Standard { get; set; } = string.Empty;
    public string? Period { get; set; }
    public string Frequency { get; set; } = "Monthly";
    public List<string>? Recipients { get; set; }
    public string? Format { get; set; } = "PDF";
}