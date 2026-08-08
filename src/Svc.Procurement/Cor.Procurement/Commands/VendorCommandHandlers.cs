using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities.Local;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Commands;

public class CreateVendorCommandHandler
    : IRequestHandler<CreateVendorCommand, VendorDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreateVendorCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreateVendorCommandHandler(
        ProcurementDbContext context,
        ILogger<CreateVendorCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorDto> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new vendor: {VendorName}", request.CreateDto.Name);

            // Check if vendor with same code already exists (including synced ones)
            var existing = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Code == request.CreateDto.Code && !v.IsDeleted, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"Vendor with code '{request.CreateDto.Code}' already exists");

            var vendor = new Vendor
            {
                Id = Guid.NewGuid(),
                Code = request.CreateDto.Code,
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
                VendorType = request.CreateDto.VendorType,
                Status = request.CreateDto.Status,
                PaymentTerms = request.CreateDto.PaymentTerms,
                Currency = request.CreateDto.Currency,
                BankName = request.CreateDto.BankName,
                BankAccount = request.CreateDto.BankAccount,
                Website = request.CreateDto.Website,
                ContactPerson = SerializeContactPerson(request.CreateDto.ContactPerson),
                IsActive = request.CreateDto.IsActive,
                IsLocalOnly = true, // Mark as locally created
                SyncedAt = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Vendors.AddAsync(vendor, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("vendors_all", cancellationToken);

            _logger.LogInformation("Vendor created successfully: {VendorCode}", vendor.Code);

            return MapToDto(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vendor");
            throw;
        }
    }

    private string SerializeContactPerson(ContactPersonDto? contact)
    {
        if (contact == null) return "{}";
        try
        {
            return JsonSerializer.Serialize(contact, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return "{}";
        }
    }

    private VendorDto MapToDto(Vendor vendor)
    {
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
            ContactPerson = DeserializeContactPerson(vendor.ContactPerson),
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
    }

    private ContactPersonDto? DeserializeContactPerson(string? json)
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

public class UpdateVendorCommandHandler
    : IRequestHandler<UpdateVendorCommand, VendorDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<UpdateVendorCommandHandler> _logger;
    private readonly ICacheService _cache;

    public UpdateVendorCommandHandler(
        ProcurementDbContext context,
        ILogger<UpdateVendorCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<VendorDto> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating vendor: {VendorId}", request.UpdateDto.Id);

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == request.UpdateDto.Id && !v.IsDeleted, cancellationToken);

            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID '{request.UpdateDto.Id}' not found");

            // Check if code is being changed and if it conflicts
            if (vendor.Code != request.UpdateDto.Code)
            {
                var existing = await _context.Vendors
                    .FirstOrDefaultAsync(v => v.Code == request.UpdateDto.Code && v.Id != request.UpdateDto.Id && !v.IsDeleted, cancellationToken);

                if (existing != null)
                    throw new InvalidOperationException($"Vendor with code '{request.UpdateDto.Code}' already exists");
            }

            // Update fields
            vendor.Code = request.UpdateDto.Code;
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
            vendor.VendorType = request.UpdateDto.VendorType;
            vendor.Status = request.UpdateDto.Status;
            vendor.PaymentTerms = request.UpdateDto.PaymentTerms;
            vendor.Currency = request.UpdateDto.Currency;
            vendor.BankName = request.UpdateDto.BankName;
            vendor.BankAccount = request.UpdateDto.BankAccount;
            vendor.Website = request.UpdateDto.Website;
            vendor.ContactPerson = SerializeContactPerson(request.UpdateDto.ContactPerson);
            if (request.UpdateDto.IsActive.HasValue)
                vendor.IsActive = request.UpdateDto.IsActive.Value;
            vendor.SyncedAt = DateTime.UtcNow;
            vendor.DateMod = DateTime.UtcNow;
            vendor.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"vendor_{vendor.Id}", cancellationToken);
            await _cache.RemoveAsync($"vendor_code_{vendor.Code}", cancellationToken);
            await _cache.RemoveAsync("vendors_all", cancellationToken);

            _logger.LogInformation("Vendor updated successfully: {VendorCode}", vendor.Code);

            return MapToDto(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vendor");
            throw;
        }
    }

    private string SerializeContactPerson(ContactPersonDto? contact)
    {
        if (contact == null) return "{}";
        try
        {
            return JsonSerializer.Serialize(contact, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return "{}";
        }
    }

    private VendorDto MapToDto(Vendor vendor)
    {
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
            ContactPerson = DeserializeContactPerson(vendor.ContactPerson),
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
    }

    private ContactPersonDto? DeserializeContactPerson(string? json)
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

public class DeleteVendorCommandHandler
    : IRequestHandler<DeleteVendorCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DeleteVendorCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteVendorCommandHandler(
        ProcurementDbContext context,
        ILogger<DeleteVendorCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting vendor: {VendorId}", request.Id);

            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, cancellationToken);

            if (vendor == null)
                return false;

            // Soft delete
            vendor.IsDeleted = true;
            vendor.IsActive = false;
            vendor.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"vendor_{vendor.Id}", cancellationToken);
            await _cache.RemoveAsync($"vendor_code_{vendor.Code}", cancellationToken);
            await _cache.RemoveAsync("vendors_all", cancellationToken);

            _logger.LogInformation("Vendor deleted successfully: {VendorCode}", vendor.Code);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vendor");
            throw;
        }
    }
}