// Commands/IFRSCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Queries;
namespace Cor.Finance.Commands;

// ============ IFRS REPORT COMMANDS ============
public class GenerateIFRSReportCmd : IRequest<IFRSReportDto>
{
    public GenerateIFRSReportDto GenerateDto { get; set; } = new();
}

public class ScheduleIFRSReportCmd : IRequest<IFRSReportDto>
{
    public ScheduleIFRSReportDto ScheduleDto { get; set; } = new();
}

public class DeleteIFRSReportCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============ HANDLERS ============
public class GenerateIFRSReportHandler : IRequestHandler<GenerateIFRSReportCmd, IFRSReportDto>
{
    private readonly FinanceDbContext _context;

    public GenerateIFRSReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IFRSReportDto> Handle(GenerateIFRSReportCmd request, CancellationToken ct)
    {
        var report = new IFRSReport
        {
            Id = Guid.NewGuid(),
            Standard = request.GenerateDto.Standard,
            Name = $"{request.GenerateDto.Standard} - {request.GenerateDto.Period ?? DateTime.Now.Year.ToString()}",
            Description = $"{request.GenerateDto.Standard} compliance report for {request.GenerateDto.Period}",
            Period = request.GenerateDto.Period ?? DateTime.Now.ToString("yyyy-MM"),
            ReportDate = DateTime.UtcNow,
            Status = "Generated",
            Format = request.GenerateDto.Format ?? "PDF",
            GeneratedBy = "System", // Would come from current user
            Summary = $"IFRS {request.GenerateDto.Standard} report generated successfully",
            DateAdd = DateTime.UtcNow
        };

        // Add sample metrics based on standard
        if (request.GenerateDto.IncludeMetrics)
        {
            var metrics = GetMetricsForStandard(request.GenerateDto.Standard);
            foreach (var metric in metrics)
            {
                report.Metrics.Add(metric);
            }
        }

        _context.IFRSReports.Add(report);
        await _context.SaveChangesAsync(ct);

        return await new GetIFRSReportByIdHandler(_context).Handle(
            new GetIFRSReportByIdQry { Id = report.Id }, ct);
    }

    private List<IFRSMetric> GetMetricsForStandard(string standard)
    {
        var metrics = new List<IFRSMetric>();

        switch (standard)
        {
            case "IFRS 9":
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Financial Assets",
                    Value = 15000000,
                    PreviousValue = 14000000,
                    Change = 1000000,
                    ChangePercentage = 7.1m,
                    Status = "Positive",
                    Unit = "USD"
                });
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Expected Credit Losses",
                    Value = 250000,
                    PreviousValue = 300000,
                    Change = -50000,
                    ChangePercentage = -16.7m,
                    Status = "Positive",
                    Unit = "USD"
                });
                break;

            case "IFRS 15":
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Total Revenue",
                    Value = 25000000,
                    PreviousValue = 23000000,
                    Change = 2000000,
                    ChangePercentage = 8.7m,
                    Status = "Positive",
                    Unit = "USD"
                });
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Contract Assets",
                    Value = 3500000,
                    PreviousValue = 3200000,
                    Change = 300000,
                    ChangePercentage = 9.4m,
                    Status = "Positive",
                    Unit = "USD"
                });
                break;

            case "IFRS 16":
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Right-of-Use Assets",
                    Value = 12000000,
                    PreviousValue = 11000000,
                    Change = 1000000,
                    ChangePercentage = 9.1m,
                    Status = "Positive",
                    Unit = "USD"
                });
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Lease Liabilities",
                    Value = 11000000,
                    PreviousValue = 10500000,
                    Change = 500000,
                    ChangePercentage = 4.8m,
                    Status = "Negative",
                    Unit = "USD"
                });
                break;

            default:
                metrics.Add(new IFRSMetric
                {
                    Id = Guid.NewGuid(),
                    Name = "Total Assets",
                    Value = 50000000,
                    PreviousValue = 48000000,
                    Change = 2000000,
                    ChangePercentage = 4.2m,
                    Status = "Positive",
                    Unit = "USD"
                });
                break;
        }

        return metrics;
    }
}

public class ScheduleIFRSReportHandler : IRequestHandler<ScheduleIFRSReportCmd, IFRSReportDto>
{
    private readonly FinanceDbContext _context;

    public ScheduleIFRSReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IFRSReportDto> Handle(ScheduleIFRSReportCmd request, CancellationToken ct)
    {
        var report = new IFRSReport
        {
            Id = Guid.NewGuid(),
            Standard = request.ScheduleDto.Standard,
            Name = $"{request.ScheduleDto.Standard} - Scheduled Report",
            Description = $"Scheduled {request.ScheduleDto.Standard} report - {request.ScheduleDto.Frequency}",
            Period = request.ScheduleDto.Period ?? DateTime.Now.ToString("yyyy-MM"),
            ReportDate = DateTime.UtcNow,
            Status = "Scheduled",
            Format = request.ScheduleDto.Format ?? "PDF",
            GeneratedBy = "System",
            Summary = $"IFRS {request.ScheduleDto.Standard} report scheduled {request.ScheduleDto.Frequency}",
            DateAdd = DateTime.UtcNow
        };

        _context.IFRSReports.Add(report);
        await _context.SaveChangesAsync(ct);

        return await new GetIFRSReportByIdHandler(_context).Handle(
            new GetIFRSReportByIdQry { Id = report.Id }, ct);
    }
}

public class DeleteIFRSReportHandler : IRequestHandler<DeleteIFRSReportCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteIFRSReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteIFRSReportCmd request, CancellationToken ct)
    {
        var report = await _context.IFRSReports
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (report == null)
            return false;

        report.IsDeleted = true;
        report.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}