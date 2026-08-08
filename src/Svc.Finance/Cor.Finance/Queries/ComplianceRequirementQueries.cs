// Queries/ComplianceRequirementQueries.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllComplianceRequirementsQry : IRequest<List<ComplianceRequirementDto>>
{
    public string? Status { get; set; }
    public string? Regulation { get; set; }
    public string? RiskLevel { get; set; }
}

public class GetComplianceRequirementByIdQry : IRequest<ComplianceRequirementDto>
{
    public Guid Id { get; set; }
}

public class GetAllComplianceRequirementsHandler : IRequestHandler<GetAllComplianceRequirementsQry, List<ComplianceRequirementDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllComplianceRequirementsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComplianceRequirementDto>> Handle(GetAllComplianceRequirementsQry request, CancellationToken ct)
    {
        var query = _context.ComplianceRequirements
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.ComplianceStatus == request.Status);

        if (!string.IsNullOrEmpty(request.Regulation))
            query = query.Where(x => x.Regulation == request.Regulation);

        if (!string.IsNullOrEmpty(request.RiskLevel))
            query = query.Where(x => x.RiskLevel == request.RiskLevel);

        return await query
            .Select(x => new ComplianceRequirementDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Regulation = x.Regulation,
                Section = x.Section,
                ComplianceStatus = x.ComplianceStatus,
                Deadline = x.Deadline,
                Owner = x.Owner,
                RiskLevel = x.RiskLevel,
                Notes = x.Notes,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod,
                Controls = x.Controls.Select(c => new ComplianceRequirementControlDto
                {
                    Id = c.Id,
                    ComplianceRequirementId = c.ComplianceRequirementId,
                    ControlName = c.ControlName,
                    Description = c.Description
                }).ToList(),
                Evidence = x.Evidence.Select(e => new ComplianceRequirementEvidenceDto
                {
                    Id = e.Id,
                    ComplianceRequirementId = e.ComplianceRequirementId,
                    FileName = e.FileName,
                    FilePath = e.FilePath,
                    UploadedAt = e.UploadedAt
                }).ToList()
            })
            .ToListAsync(ct);
    }
}

public class GetComplianceRequirementByIdHandler : IRequestHandler<GetComplianceRequirementByIdQry, ComplianceRequirementDto>
{
    private readonly FinanceDbContext _context;

    public GetComplianceRequirementByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceRequirementDto> Handle(GetComplianceRequirementByIdQry request, CancellationToken ct)
    {
        var requirement = await _context.ComplianceRequirements
            .Include(x => x.Controls)
            .Include(x => x.Evidence)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (requirement == null)
            throw new InvalidOperationException($"Compliance requirement with ID '{request.Id}' not found");

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
            DateMod = requirement.DateMod,
            Controls = requirement.Controls.Select(c => new ComplianceRequirementControlDto
            {
                Id = c.Id,
                ComplianceRequirementId = c.ComplianceRequirementId,
                ControlName = c.ControlName,
                Description = c.Description
            }).ToList(),
            Evidence = requirement.Evidence.Select(e => new ComplianceRequirementEvidenceDto
            {
                Id = e.Id,
                ComplianceRequirementId = e.ComplianceRequirementId,
                FileName = e.FileName,
                FilePath = e.FilePath,
                UploadedAt = e.UploadedAt
            }).ToList()
        };
    }
}