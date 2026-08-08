// Commands/VendorCommands.cs
using MediatR;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cor.Finance.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class VendorAddCmd : IRequest<VendorDto>
{
    public VendorCreateDto CreateDto { get; set; } = default!;
}

public class VendorModCmd : IRequest<VendorDto>
{
    public VendorUpdateDto UpdateDto { get; set; } = default!;
}

public class VendorDelCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class VendorToggleStatusCmd : IRequest<VendorDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class VendorAddHandler : IRequestHandler<VendorAddCmd, VendorDto>
{
    private readonly FinanceDbContext _context;

    public VendorAddHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VendorDto> Handle(VendorAddCmd request, CancellationToken ct)
    {
        // Generate vendor code
        var code = await GenerateVendorCode(ct);

        var vendor = new Vendor
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = request.CreateDto.Name,
            NameAm = request.CreateDto.NameAm,
            Description = request.CreateDto.Description,
            Email = request.CreateDto.Email,
            Phone = request.CreateDto.Phone,
            Mobile = request.CreateDto.Mobile,
            Address = request.CreateDto.Address,
            City = request.CreateDto.City,
            Country = request.CreateDto.Country,
            TaxId = request.CreateDto.TaxId,
            RegistrationNumber = request.CreateDto.RegistrationNumber,
            VendorType = request.CreateDto.VendorType ?? "Supplier",
            Status = "Active",
            PaymentTerms = request.CreateDto.PaymentTerms ?? "Net 30",
            Currency = request.CreateDto.Currency ?? "USD",
            BankName = request.CreateDto.BankName,
            BankAccount = request.CreateDto.BankAccount,
            Website = request.CreateDto.Website,
            ContactPerson = request.CreateDto.ContactPerson != null
                ? JsonSerializer.Serialize(request.CreateDto.ContactPerson)
                : null,
            IsActive = request.CreateDto.IsActive,
             RowVersion = Guid.NewGuid().ToString("N"),
            DateAdd = DateTime.UtcNow
        };

        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync(ct);

        return MapToDto(vendor);
    }

    private async Task<string> GenerateVendorCode(CancellationToken ct)
    {
        var lastVendor = await _context.Vendors
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(ct);

        if (lastVendor == null)
            return "VND-001";

        var lastNumber = int.Parse(lastVendor.Code.Split('-')[1]);
        return $"VND-{(lastNumber + 1):D3}";
    }

    // ✅ MAKE THIS PUBLIC STATIC - accessible from other handlers
    public static VendorDto MapToDto(Vendor vendor)
    {
        ContactPersonDto? contactPerson = null;
        if (!string.IsNullOrEmpty(vendor.ContactPerson))
        {
            try
            {
                contactPerson = JsonSerializer.Deserialize<ContactPersonDto>(vendor.ContactPerson);
            }
            catch { /* Ignore deserialization errors */ }
        }

        return new VendorDto
        {
            Id = vendor.Id,
            Code = vendor.Code,
            Name = vendor.Name,
            NameAm = vendor.NameAm,
            Description = vendor.Description,
            Email = vendor.Email,
            Phone = vendor.Phone,
            Mobile = vendor.Mobile,
            Address = vendor.Address,
            City = vendor.City,
            Country = vendor.Country,
            TaxId = vendor.TaxId,
            RegistrationNumber = vendor.RegistrationNumber,
            VendorType = vendor.VendorType,
            Status = vendor.Status,
            PaymentTerms = vendor.PaymentTerms,
            Currency = vendor.Currency,
            BankName = vendor.BankName,
            BankAccount = vendor.BankAccount,
            Website = vendor.Website,
            ContactPerson = contactPerson,
            IsActive = vendor.IsActive,
            DateAdd = vendor.DateAdd,
            DateMod = vendor.DateMod,

        };
    }
}

public class VendorModHandler : IRequestHandler<VendorModCmd, VendorDto>
{
    private readonly FinanceDbContext _context;

    public VendorModHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VendorDto> Handle(VendorModCmd request, CancellationToken ct)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == request.UpdateDto.Id && !x.IsDeleted, ct);

        if (vendor == null)
            throw new InvalidOperationException($"Vendor with ID '{request.UpdateDto.Id}' not found");

        // Update properties
        vendor.Name = request.UpdateDto.Name;
        vendor.NameAm = request.UpdateDto.NameAm;
        vendor.Description = request.UpdateDto.Description;
        vendor.Email = request.UpdateDto.Email;
        vendor.Phone = request.UpdateDto.Phone;
        vendor.Mobile = request.UpdateDto.Mobile;
        vendor.Address = request.UpdateDto.Address;
        vendor.City = request.UpdateDto.City;
        vendor.Country = request.UpdateDto.Country;
        vendor.TaxId = request.UpdateDto.TaxId;
        vendor.RegistrationNumber = request.UpdateDto.RegistrationNumber;
        vendor.VendorType = request.UpdateDto.VendorType ?? vendor.VendorType;
        vendor.Status = request.UpdateDto.Status ?? vendor.Status;
        vendor.PaymentTerms = request.UpdateDto.PaymentTerms ?? vendor.PaymentTerms;
        vendor.Currency = request.UpdateDto.Currency ?? vendor.Currency;
        vendor.BankName = request.UpdateDto.BankName;
        vendor.BankAccount = request.UpdateDto.BankAccount;
        vendor.Website = request.UpdateDto.Website;
        vendor.ContactPerson = request.UpdateDto.ContactPerson != null
            ? JsonSerializer.Serialize(request.UpdateDto.ContactPerson)
            : vendor.ContactPerson;
        vendor.IsActive = request.UpdateDto.IsActive;
        vendor.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return VendorAddHandler.MapToDto(vendor); // ✅ Use public static method
    }
}

public class VendorDelHandler : IRequestHandler<VendorDelCmd, bool>
{
    private readonly FinanceDbContext _context;

    public VendorDelHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(VendorDelCmd request, CancellationToken ct)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (vendor == null)
            return false;

        // Check if vendor has related records
        var hasInvoices = await _context.PortalInvoices
            .AnyAsync(x => x.VendorId == request.Id && !x.IsDeleted, ct);
        var hasPayments = await _context.PortalPayments
            .AnyAsync(x => x.VendorId == request.Id && !x.IsDeleted, ct);
        var hasUsers = await _context.VendorPortalUsers
            .AnyAsync(x => x.VendorId == request.Id && !x.IsDeleted, ct);

        if (hasInvoices || hasPayments || hasUsers)
            throw new InvalidOperationException("Cannot delete vendor with existing transactions. Please archive instead.");

        vendor.IsDeleted = true;
        vendor.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class VendorToggleStatusHandler : IRequestHandler<VendorToggleStatusCmd, VendorDto>
{
    private readonly FinanceDbContext _context;

    public VendorToggleStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VendorDto> Handle(VendorToggleStatusCmd request, CancellationToken ct)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (vendor == null)
            throw new InvalidOperationException($"Vendor with ID '{request.Id}' not found");

        vendor.IsActive = !vendor.IsActive;
        vendor.Status = vendor.IsActive ? "Active" : "Inactive";
        vendor.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return VendorAddHandler.MapToDto(vendor); // ✅ Use public static method
    }
}