// Cor.Finance.Handlers - AmendmentHandlers.cs

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Cor.Finance.Commands;
using Helpers;
using Cor.Finance.Queries;

namespace Cor.Finance.Handlers;

// Request Amendment Handler
public class RequestAmendmentHandler : IRequestHandler<RequestAmendmentCommand, AmendmentDto>
{
    private readonly FinanceDbContext _context;

    public RequestAmendmentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<AmendmentDto> Handle(RequestAmendmentCommand request, CancellationToken ct)
    {
        // Get the invoice
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == request.Dto.InvoiceId && !x.IsDeleted, ct);

        if (invoice == null)
            throw new DomainException($"Invoice with ID {request.Dto.InvoiceId} not found.");

        // Only allow amendments for Posted or Paid invoices
        if (invoice.Status != "Posted" && invoice.Status != "Paid")
            throw new DomainException("Amendments can only be requested for Posted or Paid invoices.");

        // Create amendment record
        var amendment = new InvoiceAmendment
        {
            Id = Guid.NewGuid(),
            InvoiceId = request.Dto.InvoiceId,
            Reason = request.Dto.Reason,
            OriginalSubTotal = invoice.SubTotal,
            OriginalTaxAmount = invoice.TaxAmount,
            OriginalTotalAmount = invoice.TotalAmount,
            RequestedSubTotal = request.Dto.RequestedSubTotal,
            RequestedTaxAmount = request.Dto.RequestedTaxAmount,
            RequestedTotalAmount = request.Dto.RequestedTotalAmount,
            Comment = request.Dto.Comment,
            Status = "Pending_Approval",
            RequestedBy = "User", // Get from current user
            RequestedAt = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        };

        _context.InvoiceAmendments.Add(amendment);
        await _context.SaveChangesAsync(ct);

        return MapToDto(amendment);
    }

    private AmendmentDto MapToDto(InvoiceAmendment amendment)
    {
        return new AmendmentDto
        {
            Id = amendment.Id,
            InvoiceId = amendment.InvoiceId,
            Reason = amendment.Reason,
            OriginalSubTotal = amendment.OriginalSubTotal,
            OriginalTaxAmount = amendment.OriginalTaxAmount,
            OriginalTotalAmount = amendment.OriginalTotalAmount,
            RequestedSubTotal = amendment.RequestedSubTotal,
            RequestedTaxAmount = amendment.RequestedTaxAmount,
            RequestedTotalAmount = amendment.RequestedTotalAmount,
            Comment = amendment.Comment,
            Status = amendment.Status,
            RequestedBy = amendment.RequestedBy,
            RequestedAt = amendment.RequestedAt,
            ApprovedBy = amendment.ApprovedBy,
            ApprovedAt = amendment.ApprovedAt,
            RejectionReason = amendment.RejectionReason
        };
    }
}

// Approve Amendment Handler
public class ApproveAmendmentHandler : IRequestHandler<ApproveAmendmentCommand, bool>
{
    private readonly FinanceDbContext _context;

    public ApproveAmendmentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ApproveAmendmentCommand request, CancellationToken ct)
    {
        var amendment = await _context.InvoiceAmendments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (amendment == null)
            throw new DomainException($"Amendment with ID {request.Id} not found.");

        if (amendment.Status != "Pending_Approval")
            throw new DomainException($"Amendment is already {amendment.Status}.");

        // Update amendment status
        amendment.Status = "Approved";
        amendment.ApprovedBy = "Admin"; // Get from current user
        amendment.ApprovedAt = DateTime.UtcNow;
        amendment.DateMod = DateTime.UtcNow;

        // Update the invoice with new amounts
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == amendment.InvoiceId && !x.IsDeleted, ct);

        if (invoice != null)
        {
            invoice.SubTotal = amendment.RequestedSubTotal;
            invoice.TaxAmount = amendment.RequestedTaxAmount;
            invoice.TotalAmount = amendment.RequestedTotalAmount;
            invoice.DateMod = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);

        return true;
    }
}
// Cor.Finance.Handlers - AmendmentQueryHandlers.cs

public class GetAmendmentsHandler : IRequestHandler<GetAmendmentsQuery, List<AmendmentDto>>
{
    private readonly FinanceDbContext _context;

    public GetAmendmentsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<AmendmentDto>> Handle(GetAmendmentsQuery request, CancellationToken ct)
    {
        var amendments = await _context.InvoiceAmendments
            .Where(x => x.InvoiceId == request.InvoiceId && !x.IsDeleted)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(ct);

        return amendments.Select(MapToDto).ToList();
    }

    private AmendmentDto MapToDto(InvoiceAmendment amendment)
    {
        return new AmendmentDto
        {
            Id = amendment.Id,
            InvoiceId = amendment.InvoiceId,
            Reason = amendment.Reason,
            OriginalSubTotal = amendment.OriginalSubTotal,
            OriginalTaxAmount = amendment.OriginalTaxAmount,
            OriginalTotalAmount = amendment.OriginalTotalAmount,
            RequestedSubTotal = amendment.RequestedSubTotal,
            RequestedTaxAmount = amendment.RequestedTaxAmount,
            RequestedTotalAmount = amendment.RequestedTotalAmount,
            Comment = amendment.Comment,
            Status = amendment.Status,
            RequestedBy = amendment.RequestedBy,
            RequestedAt = amendment.RequestedAt,
            ApprovedBy = amendment.ApprovedBy,
            ApprovedAt = amendment.ApprovedAt,
            RejectionReason = amendment.RejectionReason
        };
    }
}
// Reject Amendment Handler
public class RejectAmendmentHandler : IRequestHandler<RejectAmendmentCommand, bool>
{
    private readonly FinanceDbContext _context;

    public RejectAmendmentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RejectAmendmentCommand request, CancellationToken ct)
    {
        var amendment = await _context.InvoiceAmendments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (amendment == null)
            throw new DomainException($"Amendment with ID {request.Id} not found.");

        if (amendment.Status != "Pending_Approval")
            throw new DomainException($"Amendment is already {amendment.Status}.");

        // Update amendment status
        amendment.Status = "Rejected";
        amendment.RejectionReason = request.Dto.Reason;
        amendment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return true;
    }
}