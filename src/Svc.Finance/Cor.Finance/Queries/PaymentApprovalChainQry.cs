// Queries/PaymentApprovalChainQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllPaymentApprovalChainsQry : IRequest<List<PaymentApprovalChainDto>>
{
    public bool? IsActive { get; set; }
    public string? PaymentType { get; set; }
}

public class GetPaymentApprovalChainByIdQry : IRequest<PaymentApprovalChainDto>
{
    public Guid Id { get; set; }
}

public class GetAllPaymentApprovalChainsHandler : IRequestHandler<GetAllPaymentApprovalChainsQry, List<PaymentApprovalChainDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllPaymentApprovalChainsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentApprovalChainDto>> Handle(GetAllPaymentApprovalChainsQry request, CancellationToken ct)
    {
        var query = _context.PaymentApprovalChains
            .Where(x => !x.IsDeleted)
            .Include(x => x.Steps)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.PaymentType))
            query = query.Where(x => x.PaymentType == request.PaymentType);

        return await query
            .Select(x => new PaymentApprovalChainDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                NameAm = x.NameAm,
                Description = x.Description,
                PaymentType = x.PaymentType,
                MinAmount = x.MinAmount,
                MaxAmount = x.MaxAmount,
                IsActive = x.IsActive,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod,
                Steps = x.Steps
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.Order)
                    .Select(s => new ApprovalStepDto
                    {
                        Id = s.Id,
                        Order = s.Order,
                        Role = s.Role,
                        ApproverName = s.ApproverName,
                        ApproverId = s.ApproverId
                    })
                    .ToList()
            })
            .ToListAsync(ct);
    }
}

public class GetPaymentApprovalChainByIdHandler : IRequestHandler<GetPaymentApprovalChainByIdQry, PaymentApprovalChainDto>
{
    private readonly FinanceDbContext _context;

    public GetPaymentApprovalChainByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentApprovalChainDto> Handle(GetPaymentApprovalChainByIdQry request, CancellationToken ct)
    {
        var chain = await _context.PaymentApprovalChains
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (chain == null)
            throw new InvalidOperationException($"Payment approval chain with ID '{request.Id}' not found");

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
            DateMod = chain.DateMod,
            Steps = chain.Steps
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Order)
                .Select(s => new ApprovalStepDto
                {
                    Id = s.Id,
                    Order = s.Order,
                    Role = s.Role,
                    ApproverName = s.ApproverName,
                    ApproverId = s.ApproverId
                })
                .ToList()
        };
    }
}