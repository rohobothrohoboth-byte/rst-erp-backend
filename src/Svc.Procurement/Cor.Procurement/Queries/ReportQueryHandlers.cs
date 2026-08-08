using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;
using System.Text;
using Cor.Procurement.Models.Entities;
namespace Cor.Procurement.Queries;

public class GetReportByIdQueryHandler
    : IRequestHandler<GetReportByIdQuery, ReportDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetReportByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetReportByIdQueryHandler(
        ProcurementDbContext context,
        ILogger<GetReportByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ReportDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (report == null)
                throw new KeyNotFoundException($"Report with ID '{request.Id}' not found");

            var dto = new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                Description = report.Description,
                Category = report.Category,
                Type = report.Type,
                GeneratedDate = report.GeneratedDate,
                Period = report.Period,
                Status = report.Status,
                Format = report.Format,
                Size = report.Size,
                Downloads = report.Downloads,
                LastViewed = report.LastViewed,
                Tags = JsonSerializer.Deserialize<List<string>>(report.TagsJson ?? "[]") ?? new List<string>(),
                ReportUrl = report.ReportUrl,
                Data = !string.IsNullOrEmpty(report.DataJson)
                    ? JsonSerializer.Deserialize<object>(report.DataJson)
                    : null
            };

            // Update last viewed
            report.LastViewed = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching report {Id}", request.Id);
            throw;
        }
    }
}

public class DownloadReportQueryHandler
    : IRequestHandler<DownloadReportQuery, (byte[] FileData, string FileName, string ContentType)>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DownloadReportQueryHandler> _logger;

    public DownloadReportQueryHandler(
        ProcurementDbContext context,
        ILogger<DownloadReportQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(byte[] FileData, string FileName, string ContentType)> Handle(
        DownloadReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (report == null)
                throw new KeyNotFoundException($"Report with ID '{request.Id}' not found");

            // Generate report content based on format
            var content = GenerateReportContent(report);

            // Update download count
            report.Downloads++;
            await _context.SaveChangesAsync(cancellationToken);

            var fileName = $"{report.Name.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.{report.Format}";
            var contentType = report.Format switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return (content, fileName, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report {Id}", request.Id);
            throw;
        }
    }

    private byte[] GenerateReportContent(Report report)
    {
        var data = !string.IsNullOrEmpty(report.DataJson)
            ? JsonSerializer.Deserialize<object>(report.DataJson)
            : null;

        var sb = new StringBuilder();

        sb.AppendLine("=".PadRight(70, '='));
        sb.AppendLine("                    PROCUREMENT REPORT");
        sb.AppendLine("=".PadRight(70, '='));
        sb.AppendLine();
        sb.AppendLine($"Report ID:      {report.Id}");
        sb.AppendLine($"Report Name:    {report.Name}");
        sb.AppendLine($"Category:       {report.Category}");
        sb.AppendLine($"Period:         {report.Period}");
        sb.AppendLine($"Generated:      {report.GeneratedDate:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Format:         {report.Format.ToUpper()}");
        sb.AppendLine($"Downloads:      {report.Downloads}");
        sb.AppendLine();
        sb.AppendLine("-".PadRight(70, '-'));
        sb.AppendLine();
        sb.AppendLine($"Description:");
        sb.AppendLine($"  {report.Description ?? "No description available"}");
        sb.AppendLine();
        sb.AppendLine("-".PadRight(70, '-'));
        sb.AppendLine();
        sb.AppendLine($"Report Data:");
        sb.AppendLine($"  {JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })}");
        sb.AppendLine();
        sb.AppendLine("=".PadRight(70, '='));
        sb.AppendLine($"Generated by RST ERP Procurement System");
        sb.AppendLine($"Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine("=".PadRight(70, '='));

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}