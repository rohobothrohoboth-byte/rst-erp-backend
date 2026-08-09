using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Commands;

public class GenerateReportCommandHandler
    : IRequestHandler<GenerateReportCommand, GenerateReportResponseDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GenerateReportCommandHandler> _logger;
    private readonly ICacheService _cache;

    public GenerateReportCommandHandler(
        ProcurementDbContext context,
        ILogger<GenerateReportCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<GenerateReportResponseDto> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating report: {ReportName}", request.CreateDto.Name);

            // Generate report data based on category
            object reportData = await GenerateReportData(request.CreateDto, cancellationToken);

            var report = new Report
            {
                Id = Guid.NewGuid(),
                Name = request.CreateDto.Name,
                Description = request.CreateDto.Description,
                Category = request.CreateDto.Category,
                Type = "detailed",
                GeneratedDate = DateTime.UtcNow,
                Period = request.CreateDto.Period,
                Status = "ready",
                Format = request.CreateDto.Format,
                Size = "2.4 MB",
                Downloads = 0,
                TagsJson = JsonSerializer.Serialize(request.CreateDto.Tags ?? new List<string>()),
                DataJson = JsonSerializer.Serialize(reportData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }),
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Reports.AddAsync(report, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("reports_dashboard", cancellationToken);

            return new GenerateReportResponseDto
            {
                Id = report.Id,
                Name = report.Name,
                DownloadUrl = $"/api/procurement/v1/Reports/download/{report.Id}",
                GeneratedDate = report.GeneratedDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            throw;
        }
    }

    private async Task<object> GenerateReportData(CreateReportDto createDto, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var startDate = createDto.StartDate ?? now.AddMonths(-6);
        var endDate = createDto.EndDate ?? now;

        return createDto.Category switch
        {
            "spend" => await GenerateSpendReportData(startDate, endDate, ct),
            "vendor" => await GenerateVendorReportData(startDate, endDate, ct),
            "performance" => await GeneratePerformanceReportData(startDate, endDate, ct),
            "inventory" => await GenerateInventoryReportData(startDate, endDate, ct),
            "compliance" => await GenerateComplianceReportData(startDate, endDate, ct),
            _ => new { Message = "Report type not supported" }
        };
    }

    private async Task<object> GenerateSpendReportData(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var invoices = await _context.Invoices
            .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .ToListAsync(ct);

        var vendors = await _context.Vendors
            .Where(v => !v.IsDeleted)
            .ToListAsync(ct);

        var totalSpend = invoices.Sum(i => i.TotalAmount);

        var categories = invoices
            .Where(i => i.VendorId != Guid.Empty)
            .Join(vendors, i => i.VendorId, v => v.Id, (i, v) => new { Invoice = i, Vendor = v })
            .GroupBy(x => x.Vendor.VendorType ?? "Uncategorized")
            .Select(g => new
            {
                Category = g.Key,
                Amount = g.Sum(x => x.Invoice.TotalAmount),
                Percentage = totalSpend > 0 ? (int)((g.Sum(x => x.Invoice.TotalAmount) / totalSpend) * 100) : 0
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        return new
        {
            TotalSpend = totalSpend,
            Period = $"{startDate:MMM yyyy} - {endDate:MMM yyyy}",
            Categories = categories,
            TotalInvoices = invoices.Count,
            AverageInvoice = invoices.Count > 0 ? totalSpend / invoices.Count : 0
        };
    }

    private async Task<object> GenerateVendorReportData(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var vendors = await _context.Vendors
            .Where(v => !v.IsDeleted && v.IsActive)
            .ToListAsync(ct);

        var invoices = await _context.Invoices
            .Where(i => !i.IsDeleted && i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
            .ToListAsync(ct);

        var vendorPerformance = vendors.Select(v =>
        {
            var vendorInvoices = invoices.Where(i => i.VendorId == v.Id).ToList();
            var totalSpend = vendorInvoices.Sum(i => i.TotalAmount);
            var rating = v.Rating ?? 0;
            var score = (int)(rating * 20);

            return new
            {
                Vendor = v.Name,
                Code = v.Code,
                TotalSpent = totalSpend,
                TransactionCount = vendorInvoices.Count,
                Rating = rating,
                Score = score,
                Status = score >= 80 ? "Excellent" : score >= 60 ? "Good" : score >= 40 ? "Average" : "Poor"
            };
        })
        .OrderByDescending(x => x.Score)
        .ToList();

        return new
        {
            Vendors = vendorPerformance,
            TotalVendors = vendors.Count,
            AverageScore = vendors.Count > 0 ? (int)vendors.Average(v => (v.Rating ?? 0) * 20) : 0
        };
    }

    private async Task<object> GeneratePerformanceReportData(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        var requisitions = await _context.Requisitions
            .Where(r => !r.IsDeleted && r.DateAdd >= startDate && r.DateAdd <= endDate)
            .ToListAsync(ct);

        var pos = await _context.PurchaseOrders
            .Where(p => !p.IsDeleted && p.DateAdd >= startDate && p.DateAdd <= endDate)
            .ToListAsync(ct);

        var grns = await _context.GoodsReceiptNotes
            .Where(g => !g.IsDeleted && g.DateAdd >= startDate && g.DateAdd <= endDate)
            .ToListAsync(ct);

       var avgRequisitionTime = requisitions.Count > 0
           ? (int)requisitions
               .Select(r => (r.ApprovedAt - r.SubmittedDate))
               .Where(d => d.HasValue)
               .Select(d => d!.Value.TotalDays)
               .DefaultIfEmpty(0)
               .Average()
           : 0;

        return new
        {
            TotalRequisitions = requisitions.Count,
            TotalPurchaseOrders = pos.Count,
            TotalGRNs = grns.Count,
            AverageRequisitionApprovalTime = avgRequisitionTime,
            CompletedRequisitions = requisitions.Count(r => r.Status == "Approved"),
            PendingRequisitions = requisitions.Count(r => r.Status == "Submitted")
        };
    }
public async Task<bool> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

            if (report == null)
                return false;

            report.IsDeleted = true;
            report.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"report_{request.Id}", cancellationToken);
            await _cache.RemoveAsync("reports_dashboard", cancellationToken);

            _logger.LogInformation("Report deleted: {ReportName}", report.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting report");
            throw;
        }
    }
    private Task<object> GenerateInventoryReportData(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        // Simulate inventory data
        return Task.FromResult<object>(new
        {
            TotalItems = 150,
            LowStockItems = 12,
            OverstockItems = 8,
            ReorderNeeded = 5,
            StockValue = 125000.00m
        });
    }

    private Task<object> GenerateComplianceReportData(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        // Simulate compliance data
        return Task.FromResult<object>(new
        {
            TotalVendors = 42,
            CompliantVendors = 38,
            NonCompliantVendors = 4,
            PendingAudits = 3,
            OverallComplianceRate = 90.5m
        });
    }
}