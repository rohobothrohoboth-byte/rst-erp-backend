using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Queries;

public class GetAllVendorContractsQueryHandler
    : IRequestHandler<GetAllVendorContractsQuery, List<VendorContractDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllVendorContractsQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllVendorContractsQueryHandler(
        ProcurementDbContext context,
        ILogger<GetAllVendorContractsQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<VendorContractDto>> Handle(GetAllVendorContractsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"contracts_all_{request.VendorId}_{request.Status}_{request.Type}";
            var cached = await _cache.GetAsync<List<VendorContractDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var query = _context.VendorContracts
                .Include(c => c.Vendor)
                .Where(c => !c.IsDeleted);

            if (request.VendorId.HasValue)
                query = query.Where(c => c.VendorId == request.VendorId.Value);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(c => c.Status == request.Status);

            if (!string.IsNullOrEmpty(request.Type))
                query = query.Where(c => c.Type == request.Type);

            if (request.FromDate.HasValue)
                query = query.Where(c => c.StartDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(c => c.EndDate <= request.ToDate.Value);

            var contracts = await query
                .OrderByDescending(c => c.StartDate)
                .Select(c => new VendorContractDto
                {
                    Id = c.Id,
                    VendorId = c.VendorId,
                    VendorName = c.VendorName ?? c.Vendor!.Name,
                    VendorCode = c.VendorCode ?? c.Vendor!.Code,
                    ContractNumber = c.ContractNumber,
                    Title = c.Title,
                    Type = c.Type,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Value = c.Value,
                    Status = c.Status,
                    AutoRenew = c.AutoRenew,
                    RenewalDate = c.RenewalDate,
                    SignedDate = c.SignedDate,
                    AttachmentCount = c.AttachmentCount,
                    Terms = JsonSerializer.Deserialize<List<string>>(c.TermsJson ?? "[]") ?? new(),
                    Notes = c.Notes,
                    DateAdd = c.DateAdd,
                    DateMod = c.DateMod,
                    RowVersion = c.RowVersion
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, contracts, TimeSpan.FromMinutes(15), cancellationToken);
            return contracts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendor contracts");
            throw;
        }
    }
}

public class GetVendorContractByIdQueryHandler
    : IRequestHandler<GetVendorContractByIdQuery, VendorContractDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetVendorContractByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetVendorContractByIdQueryHandler(
        ProcurementDbContext context,
        ILogger<GetVendorContractByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorContractDto> Handle(GetVendorContractByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"contract_{request.Id}";
            var cached = await _cache.GetAsync<VendorContractDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var contract = await _context.VendorContracts
                .Include(c => c.Vendor)
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (contract == null)
                throw new KeyNotFoundException($"Contract with ID '{request.Id}' not found");

            var dto = new VendorContractDto
            {
                Id = contract.Id,
                VendorId = contract.VendorId,
                VendorName = contract.VendorName ?? contract.Vendor?.Name,
                VendorCode = contract.VendorCode ?? contract.Vendor?.Code,
                ContractNumber = contract.ContractNumber,
                Title = contract.Title,
                Type = contract.Type,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Value = contract.Value,
                Status = contract.Status,
                AutoRenew = contract.AutoRenew,
                RenewalDate = contract.RenewalDate,
                SignedDate = contract.SignedDate,
                AttachmentCount = contract.AttachmentCount,
                Terms = JsonSerializer.Deserialize<List<string>>(contract.TermsJson ?? "[]") ?? new(),
                Notes = contract.Notes,
                DateAdd = contract.DateAdd,
                DateMod = contract.DateMod,
                RowVersion = contract.RowVersion
            };

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendor contract {Id}", request.Id);
            throw;
        }
    }
}