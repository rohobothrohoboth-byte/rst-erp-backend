// Queries/ProfitCenterQueries.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllProfitCentersQry : IRequest<List<ProfitCenterDto>>
{
    public bool? IsActive { get; set; }
}

public class GetProfitCenterByIdQry : IRequest<ProfitCenterDto>
{
    public Guid Id { get; set; }
}

// ============ HANDLERS ============

public class GetAllProfitCentersHandler : IRequestHandler<GetAllProfitCentersQry, List<ProfitCenterDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllProfitCentersHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfitCenterDto>> Handle(GetAllProfitCentersQry request, CancellationToken ct)
    {
        var query = _context.ProfitCenters
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        return await query
            .Select(x => new ProfitCenterDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                Manager = x.Manager,
                Region = x.Region,
                ParentId = x.ParentId,
                ParentName = x.Parent != null ? x.Parent.Name : null,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetProfitCenterByIdHandler : IRequestHandler<GetProfitCenterByIdQry, ProfitCenterDto>
{
    private readonly FinanceDbContext _context;

    public GetProfitCenterByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitCenterDto> Handle(GetProfitCenterByIdQry request, CancellationToken ct)
    {
        var center = await _context.ProfitCenters
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (center == null)
            throw new InvalidOperationException($"Profit center with ID '{request.Id}' not found");

        return new ProfitCenterDto
        {
            Id = center.Id,
            Code = center.Code,
            Name = center.Name,
            Description = center.Description,
            IsActive = center.IsActive,
            Manager = center.Manager,
            Region = center.Region,
            ParentId = center.ParentId,
            ParentName = center.Parent?.Name,
            DateAdd = center.DateAdd,
            DateMod = center.DateMod
        };
    }
}