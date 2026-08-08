// Cor.Finance/Queries/FinanceQueryHandlers.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities.Local;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetCompanyByIdHandler : IRequestHandler<GetCompanyByIdQry, CompanyDto?>
{
    private readonly FinanceDbContext _context;

    public GetCompanyByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyDto?> Handle(GetCompanyByIdQry request, CancellationToken ct)
    {
        var company = await _context.LocalCompanies
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (company == null) return null;

        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            NameAm = company.NameAm,
            TaxId = company.TaxId,
            Phone = company.Phone,
            Email = company.Email,
            Address = company.Address
        };
    }
}

public class GetBranchByIdHandler : IRequestHandler<GetBranchByIdQry, BranchDto?>
{
    private readonly FinanceDbContext _context;

    public GetBranchByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BranchDto?> Handle(GetBranchByIdQry request, CancellationToken ct)
    {
        var branch = await _context.LocalBranches
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (branch == null) return null;

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location ?? string.Empty,
            CompId = branch.CompId ?? Guid.Empty
        };
    }
}

// Add similar handlers for Department, Position, JobGrade, and Employee...