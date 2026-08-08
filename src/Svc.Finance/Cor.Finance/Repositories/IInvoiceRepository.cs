// Repositories/InvoiceRepository.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Cor.Finance.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Linq.Expressions;

namespace Cor.Finance.Repositories;

public interface IInvoiceRepository
{
    // ============ WRITE OPERATIONS ============
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken ct = default);
    Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(Guid id, string status, CancellationToken ct = default);
    Task<Invoice> AddWithLinesAsync(Invoice invoice, List<InvoiceLine> lines, CancellationToken ct = default);
    Task<bool> AddPaymentToInvoiceAsync(Guid invoiceId, Payment payment, CancellationToken ct = default);

    // ============ READ OPERATIONS ============
    Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InvoiceDto?> GetByNumberAsync(string invoiceNumber, CancellationToken ct = default);
    Task<PaginatedResponse<InvoiceDto>> GetAllAsync(GetAllInvoicesQry query, CancellationToken ct = default);
    Task<PaginatedResponse<InvoiceDto>> GetByCustomerAsync(Guid customerId, GetAllInvoicesQry? query = null, CancellationToken ct = default);
    Task<PaginatedResponse<InvoiceDto>> GetByVendorAsync(Guid vendorId, GetAllInvoicesQry? query = null, CancellationToken ct = default);
    Task<PaginatedResponse<InvoiceDto>> GetByTypeAsync(string invoiceType, GetAllInvoicesQry? query = null, CancellationToken ct = default);
    Task<List<InvoiceDto>> GetOverdueInvoicesAsync(DateTime? asOfDate = null, CancellationToken ct = default);
    Task<InvoiceSummaryDto> GetSummaryAsync(Guid? periodId = null, CancellationToken ct = default);
    Task<AgingReportDto> GetAgingReportAsync(DateTime asOfDate, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNumberAsync(string invoiceNumber, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(Expression<Func<Invoice, bool>>? filter = null, CancellationToken ct = default);

    // ============ BULK OPERATIONS ============
    Task<List<Invoice>> BulkAddAsync(List<Invoice> invoices, CancellationToken ct = default);
    Task<bool> BulkDeleteAsync(List<Guid> ids, CancellationToken ct = default);
    Task<bool> BulkUpdateStatusAsync(List<Guid> ids, string status, CancellationToken ct = default);

    // ============ CACHE MANAGEMENT ============
    Task InvalidateCacheAsync(string? pattern = null, CancellationToken ct = default);
}
