// Queries/VendorQueries.cs
using MediatR;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Cor.Finance.Models.Entities;

namespace Cor.Finance.Queries;

// ============================================================
// QUERIES
// ============================================================

public class VendorAllQry : IRequest<List<VendorDto>>
{
    public string? Status { get; set; }
    public string? VendorType { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
}

public class VendorByIdQry : IRequest<VendorDto?>
{
    public Guid Id { get; set; }
}

public class VendorByCodeQry : IRequest<VendorDto?>
{
    public string Code { get; set; } = string.Empty;
}

public class VendorByTypeQry : IRequest<List<VendorDto>>
{
    public string VendorType { get; set; } = string.Empty;
}

public class VendorActiveQry : IRequest<List<VendorDto>>
{
    public bool? IsActive { get; set; } = true;
}

public class VendorSummaryQry : IRequest<VendorSummaryDto>
{
    public Guid? VendorId { get; set; }
}



// ============================================================
// HANDLERS
// ============================================================

public class VendorAllHandler : IRequestHandler<VendorAllQry, List<VendorDto>>
{
    private readonly FinanceDbContext _context;

    public VendorAllHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorDto>> Handle(VendorAllQry request, CancellationToken ct)
    {
        var query = _context.Vendors
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.VendorType))
            query = query.Where(x => x.VendorType == request.VendorType);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                (x.NameAm != null && x.NameAm.ToLower().Contains(search)) ||
                (x.Code != null && x.Code.ToLower().Contains(search)) ||
                (x.Email != null && x.Email.ToLower().Contains(search)));
        }

        var vendors = await query
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return vendors.Select(v => MapToDto(v)).ToList();
    }

    // ✅ MAKE THIS PUBLIC STATIC
    public static VendorDto MapToDto(Vendor vendor)
    {
        ContactPersonDto? contactPerson = null;
        if (!string.IsNullOrEmpty(vendor.ContactPerson))
        {
            try
            {
                contactPerson = JsonSerializer.Deserialize<ContactPersonDto>(vendor.ContactPerson);
            }
            catch { /* Ignore */ }
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
            DateMod = vendor.DateMod
        };
    }
}

public class VendorByIdHandler : IRequestHandler<VendorByIdQry, VendorDto?>
{
    private readonly FinanceDbContext _context;

    public VendorByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VendorDto?> Handle(VendorByIdQry request, CancellationToken ct)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (vendor == null)
            return null;

        return VendorAllHandler.MapToDto(vendor); // ✅ Use public static method
    }
}

public class VendorByCodeHandler : IRequestHandler<VendorByCodeQry, VendorDto?>
{
    private readonly FinanceDbContext _context;

    public VendorByCodeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VendorDto?> Handle(VendorByCodeQry request, CancellationToken ct)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Code == request.Code && !x.IsDeleted, ct);

        if (vendor == null)
            return null;

        return VendorAllHandler.MapToDto(vendor); // ✅ Use public static method
    }
}

public class VendorByTypeHandler : IRequestHandler<VendorByTypeQry, List<VendorDto>>
{
    private readonly FinanceDbContext _context;

    public VendorByTypeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorDto>> Handle(VendorByTypeQry request, CancellationToken ct)
    {
        var vendors = await _context.Vendors
            .Where(x => x.VendorType == request.VendorType && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return vendors.Select(VendorAllHandler.MapToDto).ToList(); // ✅ Use public static method
    }
}

public class VendorActiveHandler : IRequestHandler<VendorActiveQry, List<VendorDto>>
{
    private readonly FinanceDbContext _context;

    public VendorActiveHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorDto>> Handle(VendorActiveQry request, CancellationToken ct)
    {
        var vendors = await _context.Vendors
            .Where(x => x.IsActive == request.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return vendors.Select(VendorAllHandler.MapToDto).ToList(); // ✅ Use public static method
    }
}

public class VendorSummaryHandler : IRequestHandler<VendorSummaryQry, VendorSummaryDto>
{
    private readonly FinanceDbContext _context;

    public VendorSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VendorSummaryDto> Handle(VendorSummaryQry request, CancellationToken ct)
    {
        var query = _context.Vendors
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.VendorId.HasValue)
            query = query.Where(x => x.Id == request.VendorId.Value);

        var vendors = await query.ToListAsync(ct);

        var totalVendors = vendors.Count;
        var activeVendors = vendors.Count(x => x.IsActive);
        var inactiveVendors = totalVendors - activeVendors;

        // Get financial data from related tables
        var vendorIds = vendors.Select(x => x.Id).ToList();
        var payments = await _context.PortalPayments
            .Where(x => vendorIds.Contains(x.VendorId) && x.Status == "Completed" && !x.IsDeleted)
            .ToListAsync(ct);
        var invoices = await _context.PortalInvoices
            .Where(x => vendorIds.Contains(x.VendorId) && !x.IsDeleted)
            .ToListAsync(ct);

        var totalSpent = payments.Sum(x => x.Amount);
        var totalTransactions = invoices.Count;

        var avgRating = vendors.Where(x => x.Rating.HasValue).Average(x => x.Rating ?? 0);

        return new VendorSummaryDto
        {
            TotalVendors = totalVendors,
            ActiveVendors = activeVendors,
            InactiveVendors = inactiveVendors,
            TotalSpent = totalSpent,
            TotalTransactions = totalTransactions,
            AverageRating = avgRating,
            VendorsByType = vendors.GroupBy(x => x.VendorType)
                .ToDictionary(g => g.Key, g => g.Count()),
            VendorsByStatus = vendors.GroupBy(x => x.Status)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}