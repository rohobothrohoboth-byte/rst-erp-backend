// Queries/InternalOrderQueries.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllInternalOrdersQry : IRequest<List<InternalOrderDto>>
{
    public string? Status { get; set; }
    public Guid? CostCenterId { get; set; }
}

public class GetInternalOrderByIdQry : IRequest<InternalOrderDto>
{
    public Guid Id { get; set; }
}

// ============ HANDLERS ============

public class GetAllInternalOrdersHandler : IRequestHandler<GetAllInternalOrdersQry, List<InternalOrderDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllInternalOrdersHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<InternalOrderDto>> Handle(GetAllInternalOrdersQry request, CancellationToken ct)
    {
        var query = _context.InternalOrders
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.CostCenterId.HasValue)
            query = query.Where(x => x.CostCenterId == request.CostCenterId.Value);

        return await query
            .Select(x => new InternalOrderDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                Type = x.Type,
                BudgetAmount = x.BudgetAmount,
                ActualAmount = x.ActualAmount,
                CommittedAmount = x.CommittedAmount,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Priority = x.Priority,
                Status = x.Status,
                ResponsiblePerson = x.ResponsiblePerson,
                ProjectManager = x.ProjectManager,
                CostCenterId = x.CostCenterId,
                CostCenterName = x.CostCenter != null ? x.CostCenter.Name : null,
                PeriodId = x.PeriodId,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetInternalOrderByIdHandler : IRequestHandler<GetInternalOrderByIdQry, InternalOrderDto>
{
    private readonly FinanceDbContext _context;

    public GetInternalOrderByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalOrderDto> Handle(GetInternalOrderByIdQry request, CancellationToken ct)
    {
        var order = await _context.InternalOrders
            .Include(x => x.CostCenter)
            .Include(x => x.Period)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (order == null)
            throw new InvalidOperationException($"Internal order with ID '{request.Id}' not found");

        return new InternalOrderDto
        {
            Id = order.Id,
            Code = order.Code,
            Name = order.Name,
            Description = order.Description,
            Type = order.Type,
            BudgetAmount = order.BudgetAmount,
            ActualAmount = order.ActualAmount,
            CommittedAmount = order.CommittedAmount,
            StartDate = order.StartDate,
            EndDate = order.EndDate,
            Priority = order.Priority,
            Status = order.Status,
            ResponsiblePerson = order.ResponsiblePerson,
            ProjectManager = order.ProjectManager,
            CostCenterId = order.CostCenterId,
            CostCenterName = order.CostCenter?.Name,
            PeriodId = order.PeriodId,
            DateAdd = order.DateAdd,
            DateMod = order.DateMod
        };
    }
}