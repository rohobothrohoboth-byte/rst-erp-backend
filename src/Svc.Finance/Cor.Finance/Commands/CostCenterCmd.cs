// Commands/CostCenterCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddCostCenterCmd : IRequest<CostCenterDto>
{
    public AddCostCenterDto AddDto { get; set; } = new();
}

public class EditCostCenterCmd : IRequest<CostCenterDto>
{
    public EditCostCenterDto EditDto { get; set; } = new();
}

public class DeleteCostCenterCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// Commands/CostCenterCmd.cs - Add this at the bottom

public class BulkAddCostCenterCmd : IRequest<List<CostCenterDto>>
{
    public List<AddCostCenterDto> AddDtos { get; set; } = new();
}

public class BulkAddCostCenterHandler : IRequestHandler<BulkAddCostCenterCmd, List<CostCenterDto>>
{
    private readonly FinanceDbContext _context;

    public BulkAddCostCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<CostCenterDto>> Handle(BulkAddCostCenterCmd request, CancellationToken ct)
    {
        var result = new List<CostCenterDto>();
        var handler = new AddCostCenterHandler(_context);

        foreach (var dto in request.AddDtos)
        {
            var cmd = new AddCostCenterCmd { AddDto = dto };
            var center = await handler.Handle(cmd, ct);
            result.Add(center);
        }

        return result;
    }
}
public class AddCostCenterHandler : IRequestHandler<AddCostCenterCmd, CostCenterDto>
{
    private readonly FinanceDbContext _context;

    public AddCostCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    // ✅ MapToDto method - keep it public or internal
    public async Task<CostCenterDto> MapToDto(CostCenter center, CancellationToken ct)
    {
        var parent = center.ParentId.HasValue
            ? await _context.CostCenters
                .FirstOrDefaultAsync(x => x.Id == center.ParentId.Value && !x.IsDeleted, ct)
            : null;

        return new CostCenterDto
        {
            Id = center.Id,
            Code = center.Code,
            Name = center.Name,
            NameAm = center.NameAm,
            Description = center.Description,
            IsActive = center.IsActive,
            DepartmentId = center.DepartmentId,
            BudgetHolder = center.BudgetHolder,
            ParentId = center.ParentId,
            ParentName = parent?.Name,
            DateAdd = center.DateAdd,
            DateMod = center.DateMod
        };
    }

    public async Task<CostCenterDto> Handle(AddCostCenterCmd request, CancellationToken ct)
    {
        // 1. Check if code already exists
        var exists = await _context.CostCenters
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Cost center with code '{request.AddDto.Code}' already exists");

        // 2. Create new entity
        var center = new CostCenter
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            NameAm = request.AddDto.NameAm,
            Description = request.AddDto.Description,
            IsActive = request.AddDto.IsActive,
            DepartmentId = request.AddDto.DepartmentId,
            BudgetHolder = request.AddDto.BudgetHolder,
            ParentId = request.AddDto.ParentId,
            DateAdd = DateTime.UtcNow
        };

        // 3. Save to database
        _context.CostCenters.Add(center);
        await _context.SaveChangesAsync(ct);

        // 4. Return mapped DTO
        return await MapToDto(center, ct);
    }
}

public class EditCostCenterHandler : IRequestHandler<EditCostCenterCmd, CostCenterDto>
{
    private readonly FinanceDbContext _context;

    public EditCostCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CostCenterDto> Handle(EditCostCenterCmd request, CancellationToken ct)
    {
        var center = await _context.CostCenters
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (center == null)
            throw new InvalidOperationException($"Cost center with ID '{request.EditDto.Id}' not found");

        var exists = await _context.CostCenters
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Cost center with code '{request.EditDto.Code}' already exists");

        center.Code = request.EditDto.Code;
        center.Name = request.EditDto.Name;
        center.NameAm = request.EditDto.NameAm;
        center.Description = request.EditDto.Description;
        center.IsActive = request.EditDto.IsActive;
        center.DepartmentId = request.EditDto.DepartmentId;
        center.BudgetHolder = request.EditDto.BudgetHolder;
        center.ParentId = request.EditDto.ParentId;
        center.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        // ✅ Now this works
        var handler = new AddCostCenterHandler(_context);
        return await handler.MapToDto(center, ct);
    }
}

public class DeleteCostCenterHandler : IRequestHandler<DeleteCostCenterCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteCostCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCostCenterCmd request, CancellationToken ct)
    {
        var center = await _context.CostCenters
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (center == null)
            return false;

        var hasChildren = await _context.CostCenters
            .AnyAsync(x => x.ParentId == request.Id && !x.IsDeleted, ct);
        if (hasChildren)
            throw new InvalidOperationException("Cannot delete cost center with child centers");

        center.IsDeleted = true;
        center.IsActive = false;
        center.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}