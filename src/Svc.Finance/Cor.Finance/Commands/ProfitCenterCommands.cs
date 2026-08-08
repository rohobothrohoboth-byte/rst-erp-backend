// Commands/ProfitCenterCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddProfitCenterCmd : IRequest<ProfitCenterDto>
{
    public AddProfitCenterDto AddDto { get; set; } = new();
}

public class EditProfitCenterCmd : IRequest<ProfitCenterDto>
{
    public EditProfitCenterDto EditDto { get; set; } = new();
}

public class DeleteProfitCenterCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============ HANDLERS ============

public class AddProfitCenterHandler : IRequestHandler<AddProfitCenterCmd, ProfitCenterDto>
{
    private readonly FinanceDbContext _context;

    public AddProfitCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitCenterDto> Handle(AddProfitCenterCmd request, CancellationToken ct)
    {
        // Check for duplicate code
        var exists = await _context.ProfitCenters
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Profit center with code '{request.AddDto.Code}' already exists");

        var center = new ProfitCenter
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            Description = request.AddDto.Description,
            IsActive = request.AddDto.IsActive,
            Manager = request.AddDto.Manager,
            Region = request.AddDto.Region,
            ParentId = request.AddDto.ParentId,
            DateAdd = DateTime.UtcNow
        };

        _context.ProfitCenters.Add(center);
        await _context.SaveChangesAsync(ct);

        return MapToDto(center);
    }

    private ProfitCenterDto MapToDto(ProfitCenter center)
    {
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
            DateAdd = center.DateAdd,
            DateMod = center.DateMod
        };
    }
}

public class EditProfitCenterHandler : IRequestHandler<EditProfitCenterCmd, ProfitCenterDto>
{
    private readonly FinanceDbContext _context;

    public EditProfitCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ProfitCenterDto> Handle(EditProfitCenterCmd request, CancellationToken ct)
    {
        var center = await _context.ProfitCenters
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (center == null)
            throw new InvalidOperationException($"Profit center with ID '{request.EditDto.Id}' not found");

        // Check for duplicate code (excluding this entity)
        var exists = await _context.ProfitCenters
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Profit center with code '{request.EditDto.Code}' already exists");

        center.Code = request.EditDto.Code;
        center.Name = request.EditDto.Name;
        center.Description = request.EditDto.Description;
        center.IsActive = request.EditDto.IsActive;
        center.Manager = request.EditDto.Manager;
        center.Region = request.EditDto.Region;
        center.ParentId = request.EditDto.ParentId;
        center.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

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
            DateAdd = center.DateAdd,
            DateMod = center.DateMod
        };
    }
}

public class DeleteProfitCenterHandler : IRequestHandler<DeleteProfitCenterCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteProfitCenterHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProfitCenterCmd request, CancellationToken ct)
    {
        var center = await _context.ProfitCenters
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (center == null)
            return false;

        center.IsDeleted = true;
        center.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}