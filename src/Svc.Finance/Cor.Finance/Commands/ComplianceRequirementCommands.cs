// Commands/ComplianceRequirementCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddComplianceRequirementCmd : IRequest<ComplianceRequirementDto>
{
    public AddComplianceRequirementDto AddDto { get; set; } = new();
}

public class EditComplianceRequirementCmd : IRequest<ComplianceRequirementDto>
{
    public EditComplianceRequirementDto EditDto { get; set; } = new();
}

public class DeleteComplianceRequirementCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class AddComplianceRequirementHandler : IRequestHandler<AddComplianceRequirementCmd, ComplianceRequirementDto>
{
    private readonly FinanceDbContext _context;

    public AddComplianceRequirementHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceRequirementDto> Handle(AddComplianceRequirementCmd request, CancellationToken ct)
    {
        var exists = await _context.ComplianceRequirements
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Compliance requirement with code '{request.AddDto.Code}' already exists");

        var requirement = new ComplianceRequirement
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            Description = request.AddDto.Description,
            Regulation = request.AddDto.Regulation,
            Section = request.AddDto.Section,
            ComplianceStatus = request.AddDto.ComplianceStatus ?? "NotAssessed",
            Deadline = request.AddDto.Deadline,
            Owner = request.AddDto.Owner,
            RiskLevel = request.AddDto.RiskLevel ?? "Medium",
            Notes = request.AddDto.Notes,
            DateAdd = DateTime.UtcNow
        };

        _context.ComplianceRequirements.Add(requirement);
        await _context.SaveChangesAsync(ct);

        return MapToDto(requirement);
    }

    private ComplianceRequirementDto MapToDto(ComplianceRequirement requirement)
    {
        return new ComplianceRequirementDto
        {
            Id = requirement.Id,
            Code = requirement.Code,
            Name = requirement.Name,
            Description = requirement.Description,
            Regulation = requirement.Regulation,
            Section = requirement.Section,
            ComplianceStatus = requirement.ComplianceStatus,
            Deadline = requirement.Deadline,
            Owner = requirement.Owner,
            RiskLevel = requirement.RiskLevel,
            Notes = requirement.Notes,
            DateAdd = requirement.DateAdd,
            DateMod = requirement.DateMod
        };
    }
}

public class EditComplianceRequirementHandler : IRequestHandler<EditComplianceRequirementCmd, ComplianceRequirementDto>
{
    private readonly FinanceDbContext _context;

    public EditComplianceRequirementHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceRequirementDto> Handle(EditComplianceRequirementCmd request, CancellationToken ct)
    {
        var requirement = await _context.ComplianceRequirements
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (requirement == null)
            throw new InvalidOperationException($"Compliance requirement with ID '{request.EditDto.Id}' not found");

        var exists = await _context.ComplianceRequirements
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Compliance requirement with code '{request.EditDto.Code}' already exists");

        requirement.Code = request.EditDto.Code;
        requirement.Name = request.EditDto.Name;
        requirement.Description = request.EditDto.Description;
        requirement.Regulation = request.EditDto.Regulation;
        requirement.Section = request.EditDto.Section;
        requirement.ComplianceStatus = request.EditDto.ComplianceStatus;
        requirement.Deadline = request.EditDto.Deadline;
        requirement.Owner = request.EditDto.Owner;
        requirement.RiskLevel = request.EditDto.RiskLevel;
        requirement.Notes = request.EditDto.Notes;
        requirement.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new ComplianceRequirementDto
        {
            Id = requirement.Id,
            Code = requirement.Code,
            Name = requirement.Name,
            Description = requirement.Description,
            Regulation = requirement.Regulation,
            Section = requirement.Section,
            ComplianceStatus = requirement.ComplianceStatus,
            Deadline = requirement.Deadline,
            Owner = requirement.Owner,
            RiskLevel = requirement.RiskLevel,
            Notes = requirement.Notes,
            DateAdd = requirement.DateAdd,
            DateMod = requirement.DateMod
        };
    }
}

public class DeleteComplianceRequirementHandler : IRequestHandler<DeleteComplianceRequirementCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteComplianceRequirementHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteComplianceRequirementCmd request, CancellationToken ct)
    {
        var requirement = await _context.ComplianceRequirements
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (requirement == null)
            return false;

        requirement.IsDeleted = true;
        requirement.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}