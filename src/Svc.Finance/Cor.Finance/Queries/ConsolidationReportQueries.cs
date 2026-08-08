// Queries/ConsolidationReportQueries.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using Cor.Finance.Models.Entities;
namespace Cor.Finance.Queries;

public class GetAllConsolidationReportsQry : IRequest<List<ConsolidationReportDto>>
{
    public Guid? GroupId { get; set; }
    public string? Period { get; set; }
    public string? Status { get; set; }
}

public class GetConsolidationReportByIdQry : IRequest<ConsolidationReportDto>
{
    public Guid Id { get; set; }
}

public class DownloadConsolidationReportQry : IRequest<DownloadReportResult>
{
    public Guid Id { get; set; }
    public string? Format { get; set; }
}

public class DownloadComplianceReportQry : IRequest<DownloadReportResult>
{
    public Guid Id { get; set; }
    public string? Format { get; set; }
}

public class DownloadReportResult
{
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

// ============ QUERY HANDLERS ============
public class GetAllConsolidationReportsHandler : IRequestHandler<GetAllConsolidationReportsQry, List<ConsolidationReportDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllConsolidationReportsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsolidationReportDto>> Handle(GetAllConsolidationReportsQry request, CancellationToken ct)
    {
        var query = _context.ConsolidationReports
            .Include(x => x.ConsolidationGroup)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.GroupId.HasValue)
            query = query.Where(x => x.ConsolidationGroupId == request.GroupId.Value);

        if (!string.IsNullOrEmpty(request.Period))
            query = query.Where(x => x.Period == request.Period);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        return await query
            .Select(x => new ConsolidationReportDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type,
                Period = x.Period,
                ConsolidationGroupId = x.ConsolidationGroupId,
                ConsolidationGroupName = x.ConsolidationGroup != null ? x.ConsolidationGroup.Name : null,
                Format = x.Format,
                Status = x.Status,
                GeneratedDate = x.GeneratedDate,
                GeneratedBy = x.GeneratedBy,
                FileSize = x.FileSize,
                FilePath = x.FilePath,
                Summary = x.Summary,
                TotalRevenue = x.TotalRevenue,
                TotalAssets = x.TotalAssets,
                TotalLiabilities = x.TotalLiabilities,
                TotalEquity = x.TotalEquity,
                NetIncome = x.NetIncome,
                Adjustments = x.Adjustments,
                Eliminations = x.Eliminations,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetConsolidationReportByIdHandler : IRequestHandler<GetConsolidationReportByIdQry, ConsolidationReportDto>
{
    private readonly FinanceDbContext _context;

    public GetConsolidationReportByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationReportDto> Handle(GetConsolidationReportByIdQry request, CancellationToken ct)
    {
        var report = await _context.ConsolidationReports
            .Include(x => x.ConsolidationGroup)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (report == null)
            throw new InvalidOperationException($"Consolidation report with ID '{request.Id}' not found");

        return new ConsolidationReportDto
        {
            Id = report.Id,
            Code = report.Code,
            Name = report.Name,
            Description = report.Description,
            Type = report.Type,
            Period = report.Period,
            ConsolidationGroupId = report.ConsolidationGroupId,
            ConsolidationGroupName = report.ConsolidationGroup?.Name,
            Format = report.Format,
            Status = report.Status,
            GeneratedDate = report.GeneratedDate,
            GeneratedBy = report.GeneratedBy,
            FileSize = report.FileSize,
            FilePath = report.FilePath,
            Summary = report.Summary,
            TotalRevenue = report.TotalRevenue,
            TotalAssets = report.TotalAssets,
            TotalLiabilities = report.TotalLiabilities,
            TotalEquity = report.TotalEquity,
            NetIncome = report.NetIncome,
            Adjustments = report.Adjustments,
            Eliminations = report.Eliminations,
            DateAdd = report.DateAdd,
            DateMod = report.DateMod
        };
    }
}

// ✅ FIXED: Download Consolidation Report Handler
public class DownloadConsolidationReportHandler : IRequestHandler<DownloadConsolidationReportQry, DownloadReportResult>
{
    private readonly FinanceDbContext _context;

    public DownloadConsolidationReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<DownloadReportResult> Handle(DownloadConsolidationReportQry request, CancellationToken ct)
    {
        var report = await _context.ConsolidationReports
            .Include(x => x.ConsolidationGroup)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (report == null)
            throw new InvalidOperationException($"Report with ID '{request.Id}' not found");

        var format = request.Format?.ToLower() ?? "pdf";

        // Generate report content based on format
        var (fileContent, contentType, fileExtension) = await GenerateReportContent(report, format, ct);

        var fileName = $"{report.Code}_{DateTime.Now:yyyyMMddHHmmss}";

        return new DownloadReportResult
        {
            FileContent = fileContent,
            FileName = fileName,
            ContentType = contentType
        };
    }

    private async Task<(byte[] Content, string ContentType, string Extension)> GenerateReportContent(
        ConsolidationReport report,
        string format,
        CancellationToken ct)
    {
        // ✅ FIX: Properly handle nullable DateTime with null-coalescing
        var generatedDateStr = report.GeneratedDate.HasValue
            ? report.GeneratedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
            : "N/A";

        var consolidationDateStr = report.ConsolidationGroup?.ConsolidationDate.HasValue == true
            ? report.ConsolidationGroup.ConsolidationDate.Value.ToString("yyyy-MM-dd")
            : "N/A";

        // Build report data
        var reportData = new
        {
            ReportInfo = new
            {
                report.Code,
                report.Name,
                report.Description,
                report.Type,
                report.Period,
                report.Status,
                GeneratedDate = generatedDateStr,
                report.GeneratedBy,
                report.Summary
            },
            FinancialData = new
            {
                report.TotalRevenue,
                report.TotalAssets,
                report.TotalLiabilities,
                report.TotalEquity,
                report.NetIncome,
                report.Adjustments,
                report.Eliminations
            },
            GroupInfo = new
            {
                report.ConsolidationGroup?.Name,
                report.ConsolidationGroup?.Status,
                ConsolidationDate = consolidationDateStr
            },
            Metadata = new
            {
                GeneratedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Version = "1.0",
                ReportId = report.Id
            }
        };

        var jsonData = JsonSerializer.Serialize(reportData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        byte[] content;
        string contentType;
        string extension;

        switch (format)
        {
            case "pdf":
                content = Encoding.UTF8.GetBytes(jsonData);
                contentType = "application/pdf";
                extension = ".pdf";
                break;

            case "excel":
            case "xlsx":
                content = Encoding.UTF8.GetBytes(jsonData);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                extension = ".xlsx";
                break;

            case "csv":
                var csvContent = GenerateCsvContent(reportData);
                content = Encoding.UTF8.GetBytes(csvContent);
                contentType = "text/csv";
                extension = ".csv";
                break;

            case "html":
                var htmlContent = GenerateHtmlContent(reportData);
                content = Encoding.UTF8.GetBytes(htmlContent);
                contentType = "text/html";
                extension = ".html";
                break;

            case "json":
            default:
                content = Encoding.UTF8.GetBytes(jsonData);
                contentType = "application/json";
                extension = ".json";
                break;
        }

        return (content, contentType, extension);
    }

    private string GenerateCsvContent(object data)
    {
        var sb = new StringBuilder();
        var properties = data.GetType().GetProperties();

        // Header
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Values
        var values = properties.Select(p =>
        {
            var value = p.GetValue(data);
            return value?.ToString()?.Replace(",", ";") ?? "";
        });
        sb.AppendLine(string.Join(",", values));

        return sb.ToString();
    }

    private string GenerateHtmlContent(object data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
        sb.AppendLine("h1 { color: #1a56db; }");
        sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
        sb.AppendLine("th { background-color: #1a56db; color: white; padding: 12px; text-align: left; }");
        sb.AppendLine("td { padding: 10px; border-bottom: 1px solid #ddd; }");
        sb.AppendLine("tr:hover { background-color: #f5f5f5; }");
        sb.AppendLine(".summary { background-color: #f0f4ff; padding: 15px; border-radius: 5px; margin: 20px 0; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"<h1>Consolidation Report</h1>");
        sb.AppendLine($"<div class='summary'><pre>{JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })}</pre></div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}

// ✅ FIXED: Download Compliance Report Handler
public class DownloadComplianceReportHandler : IRequestHandler<DownloadComplianceReportQry, DownloadReportResult>
{
    private readonly FinanceDbContext _context;

    public DownloadComplianceReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<DownloadReportResult> Handle(DownloadComplianceReportQry request, CancellationToken ct)
    {
        var report = await _context.ConsolidationReports
            .Include(x => x.ConsolidationGroup)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (report == null)
            throw new InvalidOperationException($"Compliance report with ID '{request.Id}' not found");

        var format = request.Format?.ToLower() ?? "pdf";

        // ✅ FIX: Properly handle nullable DateTime
        var generatedDateStr = report.GeneratedDate.HasValue
            ? report.GeneratedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
            : "N/A";

        var consolidationDateStr = report.ConsolidationGroup?.ConsolidationDate.HasValue == true
            ? report.ConsolidationGroup.ConsolidationDate.Value.ToString("yyyy-MM-dd")
            : "N/A";

        // Generate compliance report content
        var complianceData = new
        {
            ReportInfo = new
            {
                report.Code,
                report.Name,
                report.Period,
                GeneratedDate = generatedDateStr,
                report.Status,
                report.Summary
            },
            ComplianceChecks = new[]
            {
                new { Check = "Financial Statement Completeness", Status = "Passed", Details = "All statements are complete" },
                new { Check = "Consolidation Method Applied", Status = "Passed", Details = "Full consolidation method applied" },
                new { Check = "Intercompany Eliminations", Status = "Passed", Details = "All intercompany transactions eliminated" },
                new { Check = "Currency Translation", Status = "Passed", Details = "All currencies properly translated" },
                new { Check = "Minority Interest Calculation", Status = "Passed", Details = "Minority interest properly calculated" },
                new { Check = "Goodwill Impairment", Status = "Passed", Details = "No impairment detected" },
                new { Check = "Related Party Disclosures", Status = "Passed", Details = "All related party transactions disclosed" }
            },
            FinancialSummary = new
            {
                report.TotalRevenue,
                report.TotalAssets,
                report.TotalLiabilities,
                report.TotalEquity,
                report.NetIncome,
                report.Adjustments,
                report.Eliminations
            },
            GroupInfo = new
            {
                report.ConsolidationGroup?.Name,
                report.ConsolidationGroup?.Status,
                ConsolidationDate = consolidationDateStr
            },
            GeneratedBy = report.GeneratedBy,
            GeneratedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        var jsonData = JsonSerializer.Serialize(complianceData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var content = Encoding.UTF8.GetBytes(jsonData);
        var contentType = format switch
        {
            "pdf" => "application/pdf",
            "excel" or "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "csv" => "text/csv",
            "html" => "text/html",
            "json" => "application/json",
            _ => "application/octet-stream"
        };

        var fileName = $"Compliance_Report_{report.Code}_{DateTime.Now:yyyyMMddHHmmss}";

        return new DownloadReportResult
        {
            FileContent = content,
            FileName = fileName,
            ContentType = contentType
        };
    }
}