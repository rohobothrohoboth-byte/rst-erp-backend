// Commands/InternalOrderCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddInternalOrderCmd : IRequest<InternalOrderDto>
{
    public AddInternalOrderDto AddDto { get; set; } = new();
}

public class EditInternalOrderCmd : IRequest<InternalOrderDto>
{
    public EditInternalOrderDto EditDto { get; set; } = new();
}

public class DeleteInternalOrderCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============ HANDLERS ============

public class AddInternalOrderHandler : IRequestHandler<AddInternalOrderCmd, InternalOrderDto>
{
    private readonly FinanceDbContext _context;

    public AddInternalOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalOrderDto> Handle(AddInternalOrderCmd request, CancellationToken ct)
    {
        // Check for duplicate code
        var exists = await _context.InternalOrders
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Internal order with code '{request.AddDto.Code}' already exists");

        var order = new InternalOrder
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            Description = request.AddDto.Description,
            Type = request.AddDto.Type,
            BudgetAmount = request.AddDto.BudgetAmount,
            ActualAmount = 0,
            CommittedAmount = 0,
            StartDate = request.AddDto.StartDate,
            EndDate = request.AddDto.EndDate,
            Priority = request.AddDto.Priority ?? "Medium",
            Status = request.AddDto.Status ?? "Planning",
            ResponsiblePerson = request.AddDto.ResponsiblePerson,
            ProjectManager = request.AddDto.ProjectManager,
            CostCenterId = request.AddDto.CostCenterId,
            PeriodId = request.AddDto.PeriodId,
            DateAdd = DateTime.UtcNow
        };

        _context.InternalOrders.Add(order);
        await _context.SaveChangesAsync(ct);

        return MapToDto(order);
    }

    private InternalOrderDto MapToDto(InternalOrder order)
    {
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
            PeriodId = order.PeriodId,
            DateAdd = order.DateAdd,
            DateMod = order.DateMod
        };
    }
}

public class EditInternalOrderHandler : IRequestHandler<EditInternalOrderCmd, InternalOrderDto>
{
    private readonly FinanceDbContext _context;

    public EditInternalOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InternalOrderDto> Handle(EditInternalOrderCmd request, CancellationToken ct)
    {
        var order = await _context.InternalOrders
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (order == null)
            throw new InvalidOperationException($"Internal order with ID '{request.EditDto.Id}' not found");

        // Check for duplicate code (excluding this entity)
        var exists = await _context.InternalOrders
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Internal order with code '{request.EditDto.Code}' already exists");

        order.Code = request.EditDto.Code;
        order.Name = request.EditDto.Name;
        order.Description = request.EditDto.Description;
        order.Type = request.EditDto.Type;
        order.BudgetAmount = request.EditDto.BudgetAmount;
        order.ActualAmount = request.EditDto.ActualAmount;
        order.CommittedAmount = request.EditDto.CommittedAmount;
        order.StartDate = request.EditDto.StartDate;
        order.EndDate = request.EditDto.EndDate;
        order.Priority = request.EditDto.Priority;
        order.Status = request.EditDto.Status;
        order.ResponsiblePerson = request.EditDto.ResponsiblePerson;
        order.ProjectManager = request.EditDto.ProjectManager;
        order.CostCenterId = request.EditDto.CostCenterId;
        order.PeriodId = request.EditDto.PeriodId;
        order.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

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
            PeriodId = order.PeriodId,
            DateAdd = order.DateAdd,
            DateMod = order.DateMod
        };
    }
}

public class DeleteInternalOrderHandler : IRequestHandler<DeleteInternalOrderCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteInternalOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteInternalOrderCmd request, CancellationToken ct)
    {
        var order = await _context.InternalOrders
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (order == null)
            return false;

        order.IsDeleted = true;
        order.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}