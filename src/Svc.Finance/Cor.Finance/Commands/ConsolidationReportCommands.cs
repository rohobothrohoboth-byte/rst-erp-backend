// Commands/ConsolidationReportCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class GenerateConsolidationReportCmd : IRequest<ConsolidationReportDto>
{
    public GenerateConsolidationReportDto GenerateDto { get; set; } = new();
}

public class DeleteConsolidationReportCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class GenerateConsolidationReportHandler : IRequestHandler<GenerateConsolidationReportCmd, ConsolidationReportDto>
{
    private readonly FinanceDbContext _context;

    public GenerateConsolidationReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationReportDto> Handle(GenerateConsolidationReportCmd request, CancellationToken ct)
    {
        var group = await _context.ConsolidationGroups
            .FirstOrDefaultAsync(x => x.Id == request.GenerateDto.ConsolidationGroupId, ct);

        var report = new ConsolidationReport
        {
            Id = Guid.NewGuid(),
            Code = $"CR-{DateTime.Now:yyyyMMdd-HHmmss}",
            Name = $"Consolidation Report - {group?.Name ?? "General"}",
            Description = $"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            Type = "Financial",
            Period = request.GenerateDto.Period ?? "Q1 2025",
            ConsolidationGroupId = request.GenerateDto.ConsolidationGroupId,
            Format = request.GenerateDto.Format ?? "PDF",
            Status = "Generated",
            GeneratedDate = DateTime.UtcNow,
            GeneratedBy = "System",
            Summary = "Consolidation report generated successfully",
            TotalRevenue = group?.TotalRevenue ?? 0,
            TotalAssets = group?.TotalAssets ?? 0,
            TotalLiabilities = group?.TotalLiabilities ?? 0,
            TotalEquity = group?.TotalEquity ?? 0,
            NetIncome = group?.TotalProfit ?? 0,
            Adjustments = 0,
            Eliminations = 0,
            DateAdd = DateTime.UtcNow
        };

        _context.ConsolidationReports.Add(report);
        await _context.SaveChangesAsync(ct);

        return MapToDto(report);
    }

    private ConsolidationReportDto MapToDto(ConsolidationReport report)
    {
        return new ConsolidationReportDto
        {
            Id = report.Id,
            Code = report.Code,
            Name = report.Name,
            Description = report.Description,
            Type = report.Type,
            Period = report.Period,
            ConsolidationGroupId = report.ConsolidationGroupId,
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

public class DeleteConsolidationReportHandler : IRequestHandler<DeleteConsolidationReportCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteConsolidationReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteConsolidationReportCmd request, CancellationToken ct)
    {
        var report = await _context.ConsolidationReports
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (report == null)
            return false;

        report.IsDeleted = true;
        report.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}