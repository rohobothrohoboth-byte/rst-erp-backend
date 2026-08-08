// Shared/Helpers/Services/CachedReferenceDataService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Shared.Helpers.Services;

public class CachedReferenceDataService
{
    private readonly ProcurementDbContext _context;
    private readonly RedisCacheService _cache;
    private readonly ILogger<CachedReferenceDataService> _logger;
    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(30);

    // Cache Keys
    private const string VENDORS_CACHE_KEY = "reference:vendors";
    private const string PO_STATUSES_CACHE_KEY = "reference:po_statuses";
    private const string REQ_STATUSES_CACHE_KEY = "reference:req_statuses";
    private const string FINANCIAL_PERIODS_CACHE_KEY = "reference:periods";

    public CachedReferenceDataService(
        ProcurementDbContext context,
        RedisCacheService cache,
        ILogger<CachedReferenceDataService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // ============================================================
    // ✅ VENDORS
    // ============================================================

    public async Task<List<VendorDto>> GetCachedVendorsAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<VendorDto>>(VENDORS_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Vendors retrieved from cache ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogDebug("📊 Vendors cache MISS - fetching from database");

            var vendors = await _context.Vendors
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new VendorDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Email = x.Email,
                    Phone = x.Phone,
                    Mobile = x.Mobile,
                    Address = x.Address,
                    City = x.City,
                    Country = x.Country,
                    TaxId = x.TaxId,
                    RegistrationNumber = x.RegistrationNumber,
                    VendorType = x.VendorType ?? "Supplier",
                    Status = x.Status ?? "Active",
                    PaymentTerms = x.PaymentTerms,
                    Currency = x.Currency,
                    BankName = x.BankName,
                    BankAccount = x.BankAccount,
                    Website = x.Website,
                    // ✅ Convert JSON string to ContactPersonDto
                    ContactPerson = DeserializeContactPerson(x.ContactPerson),
                    Rating = x.Rating,
                    TotalSpent = x.TotalSpent,
                    TotalTransactions = x.TotalTransactions,
                    IsActive = x.IsActive,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    SourceId = x.SourceId
                })
                .ToListAsync(ct);

            await _cache.SetAsync(VENDORS_CACHE_KEY, vendors, _defaultCacheDuration, ct);
            _logger.LogDebug("✅ Cached {Count} vendors", vendors.Count);

            return vendors;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get vendors from cache, falling back to database");
            return await _context.Vendors
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new VendorDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    Email = x.Email,
                    Phone = x.Phone,
                    Mobile = x.Mobile,
                    Address = x.Address,
                    City = x.City,
                    Country = x.Country,
                    TaxId = x.TaxId,
                    RegistrationNumber = x.RegistrationNumber,
                    VendorType = x.VendorType ?? "Supplier",
                    Status = x.Status ?? "Active",
                    PaymentTerms = x.PaymentTerms,
                    Currency = x.Currency,
                    BankName = x.BankName,
                    BankAccount = x.BankAccount,
                    Website = x.Website,
                    ContactPerson = DeserializeContactPerson(x.ContactPerson),
                    Rating = x.Rating,
                    TotalSpent = x.TotalSpent,
                    TotalTransactions = x.TotalTransactions,
                    IsActive = x.IsActive,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    SourceId = x.SourceId
                })
                .ToListAsync(ct);
        }
    }

    // ============================================================
    // ✅ HELPER: Serialize/Deserialize ContactPerson
    // ============================================================

    private static ContactPersonDto? DeserializeContactPerson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

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

    private static string SerializeContactPerson(ContactPersonDto? contact)
    {
        if (contact == null)
            return "{}";

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



    // ============================================================
    // ✅ FINANCIAL PERIODS
    // ============================================================

    public async Task<List<FinancialPeriodDto>> GetCachedFinancialPeriodsAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<FinancialPeriodDto>>(FINANCIAL_PERIODS_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Financial Periods retrieved from cache ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogDebug("📊 Financial Periods cache MISS - fetching from database");

            var periods = await _context.FinancialPeriods
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new FinancialPeriodDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsClosed = x.IsClosed,
                    Status = x.Status.ToString(),
                    PeriodType = x.PeriodType.ToString(),
                    Notes = x.Notes,
                    IsActive = x.IsActive,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    ClosedDate = x.ClosedDate,
                    ClosedBy = x.ClosedBy,
                    FiscalYear = x.FiscalYear,
                    SourceId = x.SourceId,

                })
                .ToListAsync(ct);

            await _cache.SetAsync(FINANCIAL_PERIODS_CACHE_KEY, periods, _defaultCacheDuration, ct);
            _logger.LogDebug("✅ Cached {Count} financial periods", periods.Count);

            return periods;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get financial periods from cache, falling back to database");
            return await _context.FinancialPeriods
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new FinancialPeriodDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsClosed = x.IsClosed,
                    Status = x.Status.ToString(),
                    PeriodType = x.PeriodType.ToString(),
                    Notes = x.Notes,
                    IsActive = x.IsActive,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod,
                    ClosedDate = x.ClosedDate,
                    ClosedBy = x.ClosedBy,
                    FiscalYear = x.FiscalYear,
                    SourceId = x.SourceId,

                })
                .ToListAsync(ct);
        }
    }

    // ============================================================
    // ✅ PURCHASE ORDER STATUSES
    // ============================================================

    public async Task<List<PurchaseOrderStatusDto>> GetCachedPurchaseOrderStatusesAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<PurchaseOrderStatusDto>>(PO_STATUSES_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ PO Statuses retrieved from cache ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogDebug("📊 PO Statuses cache MISS - generating defaults");

            var statuses = new List<PurchaseOrderStatusDto>
            {
                new() { Status = "Draft", DisplayName = "Draft", DisplayNameAm = "ረቂቅ", IsActive = true },
                new() { Status = "Sent", DisplayName = "Sent", DisplayNameAm = "ተልኳል", IsActive = true },
                new() { Status = "Confirmed", DisplayName = "Confirmed", DisplayNameAm = "ተረጋግጧል", IsActive = true },
                new() { Status = "Shipped", DisplayName = "Shipped", DisplayNameAm = "ተልኳል", IsActive = true },
                new() { Status = "Delivered", DisplayName = "Delivered", DisplayNameAm = "ተረክቧል", IsActive = true },
                new() { Status = "Cancelled", DisplayName = "Cancelled", DisplayNameAm = "ተሰርዟል", IsActive = true },
                new() { Status = "PartiallyReceived", DisplayName = "Partially Received", DisplayNameAm = "በከፊል ተቀብሏል", IsActive = true }
            };

            await _cache.SetAsync(PO_STATUSES_CACHE_KEY, statuses, _defaultCacheDuration, ct);
            _logger.LogDebug("✅ Cached {Count} PO statuses", statuses.Count);

            return statuses;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get PO statuses from cache, returning defaults");
            return new List<PurchaseOrderStatusDto>
            {
                new() { Status = "Draft", DisplayName = "Draft", DisplayNameAm = "ረቂቅ", IsActive = true },
                new() { Status = "Sent", DisplayName = "Sent", DisplayNameAm = "ተልኳል", IsActive = true },
                new() { Status = "Confirmed", DisplayName = "Confirmed", DisplayNameAm = "ተረጋግጧል", IsActive = true },
                new() { Status = "Shipped", DisplayName = "Shipped", DisplayNameAm = "ተልኳል", IsActive = true },
                new() { Status = "Delivered", DisplayName = "Delivered", DisplayNameAm = "ተረክቧል", IsActive = true },
                new() { Status = "Cancelled", DisplayName = "Cancelled", DisplayNameAm = "ተሰርዟል", IsActive = true }
            };
        }
    }

    // ============================================================
    // ✅ REQUISITION STATUSES
    // ============================================================

    public async Task<List<RequisitionStatusDto>> GetCachedRequisitionStatusesAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<RequisitionStatusDto>>(REQ_STATUSES_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogDebug("✅ Requisition Statuses retrieved from cache ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogDebug("📊 Requisition Statuses cache MISS - generating defaults");

            var statuses = new List<RequisitionStatusDto>
            {
                new() { Status = "Draft", DisplayName = "Draft", DisplayNameAm = "ረቂቅ", IsActive = true },
                new() { Status = "Submitted", DisplayName = "Submitted", DisplayNameAm = "ቀርቧል", IsActive = true },
                new() { Status = "UnderReview", DisplayName = "Under Review", DisplayNameAm = "በግምገማ ላይ", IsActive = true },
                new() { Status = "Approved", DisplayName = "Approved", DisplayNameAm = "ጸድቋል", IsActive = true },
                new() { Status = "Rejected", DisplayName = "Rejected", DisplayNameAm = "ውድቅ ተደርጓል", IsActive = true },
                new() { Status = "Purchased", DisplayName = "Purchased", DisplayNameAm = "ተገዝቷል", IsActive = true }
            };

            await _cache.SetAsync(REQ_STATUSES_CACHE_KEY, statuses, _defaultCacheDuration, ct);
            _logger.LogDebug("✅ Cached {Count} requisition statuses", statuses.Count);

            return statuses;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get requisition statuses from cache, returning defaults");
            return new List<RequisitionStatusDto>
            {
                new() { Status = "Draft", DisplayName = "Draft", DisplayNameAm = "ረቂቅ", IsActive = true },
                new() { Status = "Submitted", DisplayName = "Submitted", DisplayNameAm = "ቀርቧል", IsActive = true },
                new() { Status = "UnderReview", DisplayName = "Under Review", DisplayNameAm = "በግምገማ ላይ", IsActive = true },
                new() { Status = "Approved", DisplayName = "Approved", DisplayNameAm = "ጸድቋል", IsActive = true },
                new() { Status = "Rejected", DisplayName = "Rejected", DisplayNameAm = "ውድቅ ተደርጓል", IsActive = true },
                new() { Status = "Purchased", DisplayName = "Purchased", DisplayNameAm = "ተገዝቷል", IsActive = true }
            };
        }
    }

    // ============================================================
    // ✅ CACHE INVALIDATION
    // ============================================================

    public async Task InvalidateReferenceDataAsync(CancellationToken ct = default)
    {
        var keys = new[]
        {
            VENDORS_CACHE_KEY,
            PO_STATUSES_CACHE_KEY,
            REQ_STATUSES_CACHE_KEY,
            FINANCIAL_PERIODS_CACHE_KEY
        };

        foreach (var key in keys)
        {
            try
            {
                await _cache.RemoveAsync(key, ct);
                _logger.LogDebug("🗑️ Cache invalidated: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate cache for key: {Key}", key);
            }
        }
    }

    public async Task InvalidateVendorsCacheAsync(CancellationToken ct = default)
    {
        await _cache.RemoveAsync(VENDORS_CACHE_KEY, ct);
        _logger.LogDebug("🗑️ Vendors cache invalidated");
    }

    public async Task InvalidateStatusesCacheAsync(CancellationToken ct = default)
    {
        await _cache.RemoveAsync(PO_STATUSES_CACHE_KEY, ct);
        await _cache.RemoveAsync(REQ_STATUSES_CACHE_KEY, ct);
        _logger.LogDebug("🗑️ Statuses cache invalidated");
    }

    public async Task InvalidateFinancialPeriodsCacheAsync(CancellationToken ct = default)
    {
        await _cache.RemoveAsync(FINANCIAL_PERIODS_CACHE_KEY, ct);
        _logger.LogDebug("🗑️ Financial Periods cache invalidated");
    }
}