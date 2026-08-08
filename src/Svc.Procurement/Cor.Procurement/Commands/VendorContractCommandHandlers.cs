using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Commands;

public class CreateVendorContractCommandHandler
    : IRequestHandler<CreateVendorContractCommand, VendorContractDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreateVendorContractCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreateVendorContractCommandHandler(
        ProcurementDbContext context,
        ILogger<CreateVendorContractCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorContractDto> Handle(CreateVendorContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating vendor contract");

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == request.CreateDto.VendorId && !v.IsDeleted, cancellationToken);

            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID '{request.CreateDto.VendorId}' not found");

            // Check for duplicate contract number
            var exists = await _context.VendorContracts
                .AnyAsync(c => c.ContractNumber == request.CreateDto.ContractNumber && !c.IsDeleted, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"Contract number '{request.CreateDto.ContractNumber}' already exists");

            var contract = new VendorContract
            {
                Id = Guid.NewGuid(),
                VendorId = request.CreateDto.VendorId,
                VendorName = vendor.Name,
                VendorCode = vendor.Code,
                ContractNumber = request.CreateDto.ContractNumber,
                Title = request.CreateDto.Title,
                Type = request.CreateDto.Type,
                StartDate = request.CreateDto.StartDate,
                EndDate = request.CreateDto.EndDate,
                Value = request.CreateDto.Value,
                Status = request.CreateDto.Status,
                AutoRenew = request.CreateDto.AutoRenew,
                RenewalDate = request.CreateDto.RenewalDate,
                SignedDate = request.CreateDto.SignedDate,
                TermsJson = JsonSerializer.Serialize(request.CreateDto.Terms ?? new List<string>()),
                Notes = request.CreateDto.Notes,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.VendorContracts.AddAsync(contract, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("contracts_all", cancellationToken);
            await _cache.RemoveAsync($"contracts_vendor_{request.CreateDto.VendorId}", cancellationToken);

            _logger.LogInformation($"Contract created for {vendor.Name}: {contract.ContractNumber}");

            return await MapToDto(contract, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vendor contract");
            throw;
        }
    }

    private async Task<VendorContractDto> MapToDto(VendorContract contract, CancellationToken ct)
    {
        return new VendorContractDto
        {
            Id = contract.Id,
            VendorId = contract.VendorId,
            VendorName = contract.VendorName,
            VendorCode = contract.VendorCode,
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
    }
}

public class DeleteVendorContractCommandHandler
    : IRequestHandler<DeleteVendorContractCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DeleteVendorContractCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteVendorContractCommandHandler(
        ProcurementDbContext context,
        ILogger<DeleteVendorContractCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteVendorContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _context.VendorContracts
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (contract == null)
                return false;

            var vendorId = contract.VendorId;

            contract.IsDeleted = true;
            contract.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"contract_{request.Id}", cancellationToken);
            await _cache.RemoveAsync("contracts_all", cancellationToken);
            await _cache.RemoveAsync($"contracts_vendor_{vendorId}", cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vendor contract");
            throw;
        }
    }
}