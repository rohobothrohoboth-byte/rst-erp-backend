// Queries/IFRSQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============ IFRS REPORT QUERIES ============
public class GetAllIFRSReportsQry : IRequest<List<IFRSReportDto>>
{
    public string? Standard { get; set; }
    public string? Status { get; set; }
    public string? Period { get; set; }
}

public class GetIFRSReportByIdQry : IRequest<IFRSReportDto>
{
    public Guid Id { get; set; }
}

// ============ HANDLERS ============
public class GetAllIFRSReportsHandler : IRequestHandler<GetAllIFRSReportsQry, List<IFRSReportDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllIFRSReportsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<IFRSReportDto>> Handle(GetAllIFRSReportsQry request, CancellationToken ct)
    {
        var query = _context.IFRSReports
            .Include(x => x.Metrics)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Standard))
            query = query.Where(x => x.Standard == request.Standard);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Period))
            query = query.Where(x => x.Period == request.Period);

        return await query
            .Select(x => new IFRSReportDto
            {
                Id = x.Id,
                Standard = x.Standard,
                Name = x.Name,
                Description = x.Description,
                Period = x.Period,
                ReportDate = x.ReportDate,
                Status = x.Status,
                Format = x.Format,
                GeneratedBy = x.GeneratedBy,
                FilePath = x.FilePath,
                FileSize = x.FileSize,
                Summary = x.Summary,
                Metrics = x.Metrics.Select(m => new IFRSMetricDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Value = m.Value,
                    PreviousValue = m.PreviousValue,
                    Change = m.Change,
                    ChangePercentage = m.ChangePercentage,
                    Status = m.Status,
                    Unit = m.Unit
                }).ToList(),
                DateAdd = x.DateAdd
            })
            .ToListAsync(ct);
    }
}

public class GetIFRSReportByIdHandler : IRequestHandler<GetIFRSReportByIdQry, IFRSReportDto>
{
    private readonly FinanceDbContext _context;

    public GetIFRSReportByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<IFRSReportDto> Handle(GetIFRSReportByIdQry request, CancellationToken ct)
    {
        var report = await _context.IFRSReports
            .Include(x => x.Metrics)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (report == null)
            throw new InvalidOperationException($"IFRS Report with ID '{request.Id}' not found");

        return new IFRSReportDto
        {
            Id = report.Id,
            Standard = report.Standard,
            Name = report.Name,
            Description = report.Description,
            Period = report.Period,
            ReportDate = report.ReportDate,
            Status = report.Status,
            Format = report.Format,
            GeneratedBy = report.GeneratedBy,
            FilePath = report.FilePath,
            FileSize = report.FileSize,
            Summary = report.Summary,
            Metrics = report.Metrics.Select(m => new IFRSMetricDto
            {
                Id = m.Id,
                Name = m.Name,
                Value = m.Value,
                PreviousValue = m.PreviousValue,
                Change = m.Change,
                ChangePercentage = m.ChangePercentage,
                Status = m.Status,
                Unit = m.Unit
            }).ToList(),
            DateAdd = report.DateAdd
        };
    }
}