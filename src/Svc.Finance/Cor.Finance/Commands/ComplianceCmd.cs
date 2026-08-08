// Commands/ComplianceCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Cor.Finance.Commands;

// ============ INTERNAL CONTROLS COMMANDS ============
public class AddInternalControlCmd : IRequest<InternalControlDto>
{
    public AddInternalControlDto AddDto { get; set; } = new();
}

public class EditInternalControlCmd : IRequest<InternalControlDto>
{
    public EditInternalControlDto EditDto { get; set; } = new();
}

public class DeleteInternalControlCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class TestInternalControlCmd : IRequest<InternalControlDto>
{
    public Guid Id { get; set; }
}

// ============ COMPLIANCE REQUIREMENTS COMMANDS ============




public class GenerateComplianceReportCmd : IRequest<ComplianceReportDto>
{
    public GenerateComplianceReportDto GenerateDto { get; set; } = new();
}

public class DeleteComplianceReportCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}
// ============ HANDLERS ============

public class GenerateComplianceReportHandler : IRequestHandler<GenerateComplianceReportCmd, ComplianceReportDto>
{
    private readonly FinanceDbContext _context;

    public GenerateComplianceReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceReportDto> Handle(GenerateComplianceReportCmd request, CancellationToken ct)
    {
        // Generate a new compliance report
        var report = new ComplianceReport // You'll need to create this entity
        {
            Id = Guid.NewGuid(),
            Code = $"CR-{DateTime.Now:yyyyMMdd-HHmmss}",
            Name = $"Compliance Report - {request.GenerateDto.Type ?? "General"}",
            Description = $"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            Type = request.GenerateDto.Type,
            Category = request.GenerateDto.Category,
            Status = "Generated",
            Format = request.GenerateDto.Format ?? "PDF",
            GeneratedDate = DateTime.UtcNow,
            GeneratedBy = "System", // You might want to get this from the current user
            Summary = "Compliance report generated successfully",
            Findings = 0,
            Passed = 0,
            Failed = 0,
            PartiallyPassed = 0,
            ComplianceScore = 100,
            DateAdd = DateTime.UtcNow
        };

        _context.ComplianceReports.Add(report);
        await _context.SaveChangesAsync(ct);

        return MapToDto(report);
    }

    private ComplianceReportDto MapToDto(ComplianceReport report)
    {
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

// Also need the DeleteComplianceReportHandler
public class DeleteComplianceReportHandler : IRequestHandler<DeleteComplianceReportCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteComplianceReportHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteComplianceReportCmd request, CancellationToken ct)
    {
        var report = await _context.ComplianceReports
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (report == null)
            return false;

        report.IsDeleted = true;
        report.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
public class AddInternalControlHandler : IRequestHandler<AddInternalControlCmd, InternalControlDto>
{
    private readonly FinanceDbContext _context;

    public AddInternalControlHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalControlDto> Handle(AddInternalControlCmd request, CancellationToken ct)
    {
        var exists = await _context.InternalControls
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Internal control with code '{request.AddDto.Code}' already exists");

        var control = new InternalControl
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            Description = request.AddDto.Description,
            Type = request.AddDto.Type,
            Category = request.AddDto.Category,
            Frequency = request.AddDto.Frequency,
            Owner = request.AddDto.Owner,
            Department = request.AddDto.Department,
            Status = request.AddDto.Status ?? "Active",
            Effectiveness = request.AddDto.Effectiveness ?? "NotTested",
            LastTestedDate = request.AddDto.LastTestedDate,
            NextTestDate = request.AddDto.NextTestDate,
            DateAdd = DateTime.UtcNow
        };

        _context.InternalControls.Add(control);
        await _context.SaveChangesAsync(ct);

        return MapToDto(control);
    }

   public InternalControlDto MapToDto(InternalControl control)
       {
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

public class EditInternalControlHandler : IRequestHandler<EditInternalControlCmd, InternalControlDto>
{
    private readonly FinanceDbContext _context;

    public EditInternalControlHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalControlDto> Handle(EditInternalControlCmd request, CancellationToken ct)
    {
        var control = await _context.InternalControls
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (control == null)
            throw new InvalidOperationException($"Internal control with ID '{request.EditDto.Id}' not found");

        var exists = await _context.InternalControls
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Internal control with code '{request.EditDto.Code}' already exists");

        control.Code = request.EditDto.Code;
        control.Name = request.EditDto.Name;
        control.Description = request.EditDto.Description;
        control.Type = request.EditDto.Type;
        control.Category = request.EditDto.Category;
        control.Frequency = request.EditDto.Frequency;
        control.Owner = request.EditDto.Owner;
        control.Department = request.EditDto.Department;
        control.Status = request.EditDto.Status;
        control.Effectiveness = request.EditDto.Effectiveness;
        control.LastTestedDate = request.EditDto.LastTestedDate;
        control.NextTestDate = request.EditDto.NextTestDate;
        control.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new AddInternalControlHandler(_context).MapToDto(control);
    }
}

public class DeleteInternalControlHandler : IRequestHandler<DeleteInternalControlCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteInternalControlHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteInternalControlCmd request, CancellationToken ct)
    {
        var control = await _context.InternalControls
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (control == null)
            return false;

        control.IsDeleted = true;
        control.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class TestInternalControlHandler : IRequestHandler<TestInternalControlCmd, InternalControlDto>
{
    private readonly FinanceDbContext _context;

    public TestInternalControlHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalControlDto> Handle(TestInternalControlCmd request, CancellationToken ct)
    {
        var control = await _context.InternalControls
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (control == null)
            throw new InvalidOperationException($"Internal control with ID '{request.Id}' not found");

        control.LastTestedDate = DateTime.UtcNow;
        control.NextTestDate = DateTime.UtcNow.AddMonths(3);
        control.Effectiveness = "High"; // Simplified - would be determined by test results
        control.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new AddInternalControlHandler(_context).MapToDto(control);
    }
}

