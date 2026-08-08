using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities.Local;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Queries;

public class GetAllVendorsQueryHandler
    : IRequestHandler<GetAllVendorsQuery, List<VendorDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllVendorsQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllVendorsQueryHandler(
        ProcurementDbContext context,
        ILogger<GetAllVendorsQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<VendorDto>> Handle(GetAllVendorsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"vendors_all_{request.Status}_{request.VendorType}_{request.SearchTerm}";
            var cached = await _cache.GetAsync<List<VendorDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Vendors ({Count} items)", cached.Count);
                return cached;
            }

            var query = _context.Vendors
                .Where(v => !v.IsDeleted);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(v => v.Status == request.Status);

            if (!string.IsNullOrEmpty(request.VendorType))
                query = query.Where(v => v.VendorType == request.VendorType);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(v =>
                    v.Name.ToLower().Contains(search) ||
                    v.Code.ToLower().Contains(search) ||
                    (v.Email != null && v.Email.ToLower().Contains(search)));
            }

            // ✅ FIX: Use Select with explicit mapping (no instance method)
            var vendors = await query
                .OrderBy(v => v.Name)
                .Select(v => new VendorDto
                {
                    Id = v.Id,
                    Code = v.Code,
                    Name = v.Name,
                    NameAm = v.NameAm,
                    Description = v.Description,
                    Email = v.Email,
                    Phone = v.Phone,
                    Mobile = v.Mobile,
                    Address = v.Address,
                    City = v.City,
                    Country = v.Country,
                    TaxId = v.TaxId,
                    RegistrationNumber = v.RegistrationNumber,
                    VendorType = v.VendorType,
                    Status = v.Status,
                    PaymentTerms = v.PaymentTerms,
                    Currency = v.Currency,
                    BankName = v.BankName,
                    BankAccount = v.BankAccount,
                    Website = v.Website,
                    // ✅ Use static method for JSON deserialization
                    ContactPerson = VendorQueryHelpers.DeserializeContactPerson(v.ContactPerson),
                    Rating = v.Rating,
                    TotalSpent = v.TotalSpent,
                    TotalTransactions = v.TotalTransactions,
                    IsActive = v.IsActive,
                    SyncedAt = v.SyncedAt,
                    SourceId = v.SourceId,
                    IsLocalOnly = v.IsLocalOnly,
                    DateAdd = v.DateAdd,
                    DateMod = v.DateMod
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, vendors, TimeSpan.FromMinutes(15), cancellationToken);
            return vendors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendors");
            throw;
        }
    }
}

// ✅ Static helper class for mapping
public static class VendorQueryHelpers
{
    public static ContactPersonDto? DeserializeContactPerson(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<ContactPersonDto>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return null;
        }
    }
}

public class GetVendorByIdQueryHandler
    : IRequestHandler<GetVendorByIdQuery, VendorDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetVendorByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetVendorByIdQueryHandler(
        ProcurementDbContext context,
        ILogger<GetVendorByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorDto> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"vendor_{request.Id}";
            var cached = await _cache.GetAsync<VendorDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, cancellationToken);

            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID '{request.Id}' not found");

            // ✅ Use explicit mapping
            var dto = new VendorDto
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
                ContactPerson = VendorQueryHelpers.DeserializeContactPerson(vendor.ContactPerson),
                Rating = vendor.Rating,
                TotalSpent = vendor.TotalSpent,
                TotalTransactions = vendor.TotalTransactions,
                IsActive = vendor.IsActive,
                SyncedAt = vendor.SyncedAt,
                SourceId = vendor.SourceId,
                IsLocalOnly = vendor.IsLocalOnly,
                DateAdd = vendor.DateAdd,
                DateMod = vendor.DateMod
            };

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendor {Id}", request.Id);
            throw;
        }
    }
}

public class GetVendorByCodeQueryHandler
    : IRequestHandler<GetVendorByCodeQuery, VendorDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetVendorByCodeQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetVendorByCodeQueryHandler(
        ProcurementDbContext context,
        ILogger<GetVendorByCodeQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorDto> Handle(GetVendorByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"vendor_code_{request.Code}";
            var cached = await _cache.GetAsync<VendorDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Code == request.Code && !v.IsDeleted, cancellationToken);

            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with Code '{request.Code}' not found");

            var dto = new VendorDto
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
                ContactPerson = VendorQueryHelpers.DeserializeContactPerson(vendor.ContactPerson),
                Rating = vendor.Rating,
                TotalSpent = vendor.TotalSpent,
                TotalTransactions = vendor.TotalTransactions,
                IsActive = vendor.IsActive,
                SyncedAt = vendor.SyncedAt,
                SourceId = vendor.SourceId,
                IsLocalOnly = vendor.IsLocalOnly,
                DateAdd = vendor.DateAdd,
                DateMod = vendor.DateMod
            };

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vendor by code {Code}", request.Code);
            throw;
        }
    }
}