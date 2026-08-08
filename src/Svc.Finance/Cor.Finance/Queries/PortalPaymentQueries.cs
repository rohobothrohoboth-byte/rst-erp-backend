// Queries/PortalPaymentQueries.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllPortalPaymentsQry : IRequest<List<PortalPaymentDto>>
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? InvoiceId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetPortalPaymentByIdQry : IRequest<PortalPaymentDto>
{
    public Guid Id { get; set; }
}

public class GetPortalPaymentsByInvoiceQry : IRequest<List<PortalPaymentDto>>
{
    public Guid InvoiceId { get; set; }
}

public class GetPortalPaymentSummaryQry : IRequest<PortalPaymentSummaryDto>
{
    public Guid? VendorId { get; set; }
}

// ============ HANDLERS ============

public class GetAllPortalPaymentsHandler : IRequestHandler<GetAllPortalPaymentsQry, List<PortalPaymentDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllPortalPaymentsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PortalPaymentDto>> Handle(GetAllPortalPaymentsQry request, CancellationToken ct)
    {
        var query = _context.PortalPayments
            .Include(x => x.Invoice)
            .Include(x => x.Vendor)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.VendorId.HasValue)
            query = query.Where(x => x.VendorId == request.VendorId.Value);

        if (request.InvoiceId.HasValue)
            query = query.Where(x => x.InvoiceId == request.InvoiceId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.PaymentDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.PaymentDate <= request.ToDate.Value);

        return await query
            .OrderByDescending(x => x.PaymentDate)
            .Select(x => new PortalPaymentDto
            {
                Id = x.Id,
                PaymentNumber = x.PaymentNumber,
                InvoiceId = x.InvoiceId,
                InvoiceNumber = x.Invoice != null ? x.Invoice.InvoiceNumber : string.Empty,
                VendorId = x.VendorId,
                VendorName = x.Vendor != null ? x.Vendor.Name : string.Empty,
                VendorCode = x.Vendor != null ? x.Vendor.Code : string.Empty,
                Amount = x.Amount,
                Currency = x.Currency,
                PaymentDate = x.PaymentDate,
                PaymentMethod = x.PaymentMethod,
                ReferenceNumber = x.ReferenceNumber,
                Status = x.Status,
                Remarks = x.Remarks,
                ProcessedDate = x.ProcessedDate,
                ProcessedBy = x.ProcessedBy,
                TransactionId = x.TransactionId,
                BankName = x.BankName,
                AccountNumber = x.AccountNumber,
                CompletedDate = x.CompletedDate,
                FailureReason = x.FailureReason,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetPortalPaymentByIdHandler : IRequestHandler<GetPortalPaymentByIdQry, PortalPaymentDto>
{
    private readonly FinanceDbContext _context;

    public GetPortalPaymentByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalPaymentDto> Handle(GetPortalPaymentByIdQry request, CancellationToken ct)
    {
        var payment = await _context.PortalPayments
            .Include(x => x.Invoice)
            .Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            throw new InvalidOperationException($"Payment with ID '{request.Id}' not found");

        return new PortalPaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            InvoiceId = payment.InvoiceId,
            InvoiceNumber = payment.Invoice?.InvoiceNumber ?? string.Empty,
            VendorId = payment.VendorId,
            VendorName = payment.Vendor?.Name ?? string.Empty,
            VendorCode = payment.Vendor?.Code ?? string.Empty,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod,
            ReferenceNumber = payment.ReferenceNumber,
            Status = payment.Status,
            Remarks = payment.Remarks,
            ProcessedDate = payment.ProcessedDate,
            ProcessedBy = payment.ProcessedBy,
            TransactionId = payment.TransactionId,
            BankName = payment.BankName,
            AccountNumber = payment.AccountNumber,
            CompletedDate = payment.CompletedDate,
            FailureReason = payment.FailureReason,
            DateAdd = payment.DateAdd,
            DateMod = payment.DateMod
        };
    }
}

public class GetPortalPaymentsByInvoiceHandler : IRequestHandler<GetPortalPaymentsByInvoiceQry, List<PortalPaymentDto>>
{
    private readonly FinanceDbContext _context;

    public GetPortalPaymentsByInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PortalPaymentDto>> Handle(GetPortalPaymentsByInvoiceQry request, CancellationToken ct)
    {
        var payments = await _context.PortalPayments
            .Include(x => x.Vendor)
            .Where(x => x.InvoiceId == request.InvoiceId && !x.IsDeleted)
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync(ct);

        return payments.Select(p => new PortalPaymentDto
        {
            Id = p.Id,
            PaymentNumber = p.PaymentNumber,
            InvoiceId = p.InvoiceId,
            InvoiceNumber = string.Empty,
            VendorId = p.VendorId,
            VendorName = p.Vendor?.Name ?? string.Empty,
            VendorCode = p.Vendor?.Code ?? string.Empty,
            Amount = p.Amount,
            Currency = p.Currency,
            PaymentDate = p.PaymentDate,
            PaymentMethod = p.PaymentMethod,
            ReferenceNumber = p.ReferenceNumber,
            Status = p.Status,
            Remarks = p.Remarks,
            ProcessedDate = p.ProcessedDate,
            ProcessedBy = p.ProcessedBy,
            TransactionId = p.TransactionId,
            BankName = p.BankName,
            AccountNumber = p.AccountNumber,
            CompletedDate = p.CompletedDate,
            FailureReason = p.FailureReason,
            DateAdd = p.DateAdd,
            DateMod = p.DateMod
        }).ToList();
    }
}

public class GetPortalPaymentSummaryHandler : IRequestHandler<GetPortalPaymentSummaryQry, PortalPaymentSummaryDto>
{
    private readonly FinanceDbContext _context;

    public GetPortalPaymentSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalPaymentSummaryDto> Handle(GetPortalPaymentSummaryQry request, CancellationToken ct)
    {
        var query = _context.PortalPayments
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.VendorId.HasValue)
            query = query.Where(x => x.VendorId == request.VendorId.Value);

        var payments = await query.ToListAsync(ct);

        var totalPayments = payments.Sum(x => x.Amount);
        var pendingPayments = payments.Where(x => x.Status == "Pending" || x.Status == "Processing").Sum(x => x.Amount);
        var completedPayments = payments.Where(x => x.Status == "Completed").Sum(x => x.Amount);

        return new PortalPaymentSummaryDto
        {
            TotalPayments = totalPayments,
            PendingPayments = pendingPayments,
            CompletedPayments = completedPayments,
            PaymentCount = payments.Count,
            PendingCount = payments.Count(x => x.Status == "Pending" || x.Status == "Processing"),
            CompletedCount = payments.Count(x => x.Status == "Completed"),
            AveragePaymentAmount = payments.Count > 0 ? totalPayments / payments.Count : 0,
            PaymentsByMethod = payments.GroupBy(x => x.PaymentMethod)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount)),
            PaymentsByStatus = payments.GroupBy(x => x.Status)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}