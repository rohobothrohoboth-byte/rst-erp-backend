using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Services;

public interface IFinanceApiService
{
    // Financial Periods
    Task<List<FinancialPeriodDto>> GetAllFinancialPeriodsAsync(CancellationToken ct = default);
    Task<FinancialPeriodDto?> GetFinancialPeriodAsync(Guid id, CancellationToken ct = default);

    // Vendors
    Task<List<VendorDto>> GetAllVendorsAsync(CancellationToken ct = default);
    Task<VendorDto?> GetVendorAsync(Guid id, CancellationToken ct = default);
    Task<VendorDto?> GetVendorByCodeAsync(string code, CancellationToken ct = default);
}