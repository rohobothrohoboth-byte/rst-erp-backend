// Commands/PaymentApprovalChainCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddPaymentApprovalChainCmd : IRequest<PaymentApprovalChainDto>
{
    public AddPaymentApprovalChainDto AddDto { get; set; } = new();
}

public class EditPaymentApprovalChainCmd : IRequest<PaymentApprovalChainDto>
{
    public EditPaymentApprovalChainDto EditDto { get; set; } = new();
}

public class DeletePaymentApprovalChainCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class AddPaymentApprovalChainHandler : IRequestHandler<AddPaymentApprovalChainCmd, PaymentApprovalChainDto>
{
    private readonly FinanceDbContext _context;

    public AddPaymentApprovalChainHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentApprovalChainDto> Handle(AddPaymentApprovalChainCmd request, CancellationToken ct)
    {
        var exists = await _context.PaymentApprovalChains
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Chain with code '{request.AddDto.Code}' already exists");

        if (!request.AddDto.Steps.Any())
            throw new InvalidOperationException("At least one approval step is required");

        var chain = new PaymentApprovalChain
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            NameAm = request.AddDto.NameAm,
            Description = request.AddDto.Description,
            PaymentType = request.AddDto.PaymentType,
            MinAmount = request.AddDto.MinAmount,
            MaxAmount = request.AddDto.MaxAmount,
            IsActive = request.AddDto.IsActive,
            DateAdd = DateTime.UtcNow,
            Steps = request.AddDto.Steps.Select((s, index) => new ApprovalStep
            {
                Id = Guid.NewGuid(),
                Order = index + 1,
                Role = s.Role,
                ApproverName = s.ApproverName,
                ApproverId = s.ApproverId,
                DateAdd = DateTime.UtcNow
            }).ToList()
        };

        _context.PaymentApprovalChains.Add(chain);
        await _context.SaveChangesAsync(ct);

        return new PaymentApprovalChainDto
        {
            Id = chain.Id,
            Code = chain.Code,
            Name = chain.Name,
            NameAm = chain.NameAm,
            Description = chain.Description,
            PaymentType = chain.PaymentType,
            MinAmount = chain.MinAmount,
            MaxAmount = chain.MaxAmount,
            IsActive = chain.IsActive,
            DateAdd = chain.DateAdd,
            Steps = chain.Steps
                .OrderBy(s => s.Order)
                .Select(s => new ApprovalStepDto
                {
                    Id = s.Id,
                    Order = s.Order,
                    Role = s.Role,
                    ApproverName = s.ApproverName,
                    ApproverId = s.ApproverId
                }).ToList()
        };
    }
}

public class EditPaymentApprovalChainHandler : IRequestHandler<EditPaymentApprovalChainCmd, PaymentApprovalChainDto>
{
    private readonly FinanceDbContext _context;

    public EditPaymentApprovalChainHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentApprovalChainDto> Handle(EditPaymentApprovalChainCmd request, CancellationToken ct)
    {
        var chain = await _context.PaymentApprovalChains
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (chain == null)
            throw new InvalidOperationException($"Chain with ID '{request.EditDto.Id}' not found");

        var exists = await _context.PaymentApprovalChains
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Chain with code '{request.EditDto.Code}' already exists");

        if (!request.EditDto.Steps.Any())
            throw new InvalidOperationException("At least one approval step is required");

        chain.Code = request.EditDto.Code;
        chain.Name = request.EditDto.Name;
        chain.NameAm = request.EditDto.NameAm;
        chain.Description = request.EditDto.Description;
        chain.PaymentType = request.EditDto.PaymentType;
        chain.MinAmount = request.EditDto.MinAmount;
        chain.MaxAmount = request.EditDto.MaxAmount;
        chain.IsActive = request.EditDto.IsActive;
        chain.DateMod = DateTime.UtcNow;

        // Update steps - delete old, add new
        var existingStepIds = chain.Steps.Select(s => s.Id).ToHashSet();
        var updatedStepIds = request.EditDto.Steps.Where(s => s.Id != Guid.Empty).Select(s => s.Id).ToHashSet();

        // Remove steps not in updated list
        var stepsToRemove = chain.Steps
            .Where(s => !updatedStepIds.Contains(s.Id))
            .ToList();

        foreach (var step in stepsToRemove)
        {
            step.IsDeleted = true;
        }

        // Update or add steps
        foreach (var stepDto in request.EditDto.Steps)
        {
            if (stepDto.Id != Guid.Empty && existingStepIds.Contains(stepDto.Id))
            {
                var existingStep = chain.Steps.First(s => s.Id == stepDto.Id);
                existingStep.Order = stepDto.Order;
                existingStep.Role = stepDto.Role;
                existingStep.ApproverName = stepDto.ApproverName;
                existingStep.ApproverId = stepDto.ApproverId;
                existingStep.DateMod = DateTime.UtcNow;
            }
            else
            {
                chain.Steps.Add(new ApprovalStep
                {
                    Id = Guid.NewGuid(),
                    Order = stepDto.Order,
                    Role = stepDto.Role,
                    ApproverName = stepDto.ApproverName,
                    ApproverId = stepDto.ApproverId,
                    PaymentApprovalChainId = chain.Id,
                    DateAdd = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(ct);

        // Refetch to include updated steps
        var updatedChain = await _context.PaymentApprovalChains
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == chain.Id && !x.IsDeleted, ct);

        return new PaymentApprovalChainDto
        {
            Id = updatedChain!.Id,
            Code = updatedChain.Code,
            Name = updatedChain.Name,
            NameAm = updatedChain.NameAm,
            Description = updatedChain.Description,
            PaymentType = updatedChain.PaymentType,
            MinAmount = updatedChain.MinAmount,
            MaxAmount = updatedChain.MaxAmount,
            IsActive = updatedChain.IsActive,
            DateAdd = updatedChain.DateAdd,
            DateMod = updatedChain.DateMod,
            Steps = updatedChain.Steps
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Order)
                .Select(s => new ApprovalStepDto
                {
                    Id = s.Id,
                    Order = s.Order,
                    Role = s.Role,
                    ApproverName = s.ApproverName,
                    ApproverId = s.ApproverId
                }).ToList()
        };
    }
}

public class DeletePaymentApprovalChainHandler : IRequestHandler<DeletePaymentApprovalChainCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeletePaymentApprovalChainHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePaymentApprovalChainCmd request, CancellationToken ct)
    {
        var chain = await _context.PaymentApprovalChains
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (chain == null)
            return false;

        chain.IsDeleted = true;
        chain.IsActive = false;
        chain.DateMod = DateTime.UtcNow;

        foreach (var step in chain.Steps)
        {
            step.IsDeleted = true;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}