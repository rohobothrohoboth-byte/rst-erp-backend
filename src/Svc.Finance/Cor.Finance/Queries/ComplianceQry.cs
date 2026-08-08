// Queries/ComplianceQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============ INTERNAL CONTROLS QUERIES ============
public class GetAllInternalControlsQry : IRequest<List<InternalControlDto>>
{
    public string? Status { get; set; }
    public string? Category { get; set; }
}

public class GetInternalControlByIdQry : IRequest<InternalControlDto>
{
    public Guid Id { get; set; }
}
public class GetComplianceReportByIdQry : IRequest<ComplianceReportDto>
{
    public Guid Id { get; set; }
}
// ============ COMPLIANCE REQUIREMENTS QUERIES ============


public class GetAllComplianceReportsQry : IRequest<List<ComplianceReportDto>>
{
    public string? Status { get; set; }
    public string? Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
public class GetComplianceReportByIdHandler : IRequestHandler<GetComplianceReportByIdQry, ComplianceReportDto>
{
    private readonly FinanceDbContext _context;

    public GetComplianceReportByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceReportDto> Handle(GetComplianceReportByIdQry request, CancellationToken ct)
    {
        var report = await _context.ComplianceReports
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (report == null)
            throw new InvalidOperationException($"Compliance report with ID '{request.Id}' not found");

        return new ComplianceReportDto
        {
            Id = report.Id,
            Code = report.Code,
            Name = report.Name,
            Description = report.Description,
            Type = report.Type,
            Category = report.Category,
            Status = report.Status,
            Format = report.Format,
            GeneratedDate = report.GeneratedDate,
            GeneratedBy = report.GeneratedBy,
            FilePath = report.FilePath,
            FileSize = report.FileSize,
            Summary = report.Summary,
            Findings = report.Findings,
            Passed = report.Passed,
            Failed = report.Failed,
            PartiallyPassed = report.PartiallyPassed,
            ComplianceScore = report.ComplianceScore,
            DateAdd = report.DateAdd,
            DateMod = report.DateMod
        };
    }
}
// ✅ Make sure this handler exists
public class GetAllComplianceReportsHandler : IRequestHandler<GetAllComplianceReportsQry, List<ComplianceReportDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllComplianceReportsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComplianceReportDto>> Handle(GetAllComplianceReportsQry request, CancellationToken ct)
    {
        var query = _context.ComplianceReports
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Type))
            query = query.Where(x => x.Type == request.Type);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.GeneratedDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.GeneratedDate <= request.ToDate.Value);

        return await query
            .Select(x => new ComplianceReportDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type,
                Category = x.Category,
                Status = x.Status,
                Format = x.Format,
                GeneratedDate = x.GeneratedDate,
                GeneratedBy = x.GeneratedBy,
                FilePath = x.FilePath,
                FileSize = x.FileSize,
                Summary = x.Summary,
                Findings = x.Findings,
                Passed = x.Passed,
                Failed = x.Failed,
                PartiallyPassed = x.PartiallyPassed,
                ComplianceScore = x.ComplianceScore,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}
// ============ HANDLERS ============
public class GetAllInternalControlsHandler : IRequestHandler<GetAllInternalControlsQry, List<InternalControlDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllInternalControlsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<InternalControlDto>> Handle(GetAllInternalControlsQry request, CancellationToken ct)
    {
        var query = _context.InternalControls
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(x => x.Category == request.Category);

        return await query
            .Select(x => new InternalControlDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type,
                Category = x.Category,
                Frequency = x.Frequency,
                Owner = x.Owner,
                Department = x.Department,
                Status = x.Status,
                Effectiveness = x.Effectiveness,
                LastTestedDate = x.LastTestedDate,
                NextTestDate = x.NextTestDate,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetInternalControlByIdHandler : IRequestHandler<GetInternalControlByIdQry, InternalControlDto>
{
    private readonly FinanceDbContext _context;

    public GetInternalControlByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalControlDto> Handle(GetInternalControlByIdQry request, CancellationToken ct)
    {
        var control = await _context.InternalControls
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (control == null)
            throw new InvalidOperationException($"Internal control with ID '{request.Id}' not found");

        return new InternalControlDto
        {
            Id = control.Id,
            Code = control.Code,
            Name = control.Name,
            Description = control.Description,
            Type = control.Type,
            Category = control.Category,
            Frequency = control.Frequency,
            Owner = control.Owner,
            Department = control.Department,
            Status = control.Status,
            Effectiveness = control.Effectiveness,
            LastTestedDate = control.LastTestedDate,
            NextTestDate = control.NextTestDate,
            DateAdd = control.DateAdd,
            DateMod = control.DateMod
        };
    }
}

