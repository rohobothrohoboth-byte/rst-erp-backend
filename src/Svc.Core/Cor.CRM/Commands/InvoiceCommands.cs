// Cor.CRM/Commands/InvoiceCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class InvoiceAddCmd : IRequest<InvoiceDto>
{
    public CreateInvoiceDto Dto { get; set; } = default!;
}

public class InvoiceModCmd : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
    public UpdateInvoiceDto Dto { get; set; } = default!;
}

public class InvoiceDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class InvoiceSendCmd : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
}

public class InvoiceMarkPaidCmd : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
    public decimal? AmountPaid { get; set; }
}

public class InvoiceCancelCmd : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
}

public class InvoiceRefundCmd : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

// ============================================================
// INVOICE ADD HANDLER
// ============================================================

public class InvoiceAddHandler : IRequestHandler<InvoiceAddCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(InvoiceAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Generate invoice number
            var invoiceNumber = await GenerateInvoiceNumber(ct);

            var invoice = new Invoice
            {
                Id = Guid.CreateVersion7(),
                InvoiceNumber = invoiceNumber,
                LeadId = request.Dto.LeadId,
                CustomerId = request.Dto.CustomerId,
                OpportunityId = request.Dto.OpportunityId,
                QuoteId = request.Dto.QuoteId,
                InvoiceDate = request.Dto.InvoiceDate,
                DueDate = request.Dto.DueDate,
                Type = string.IsNullOrEmpty(request.Dto.Type) ? InvoiceType.Sales : Enum.Parse<InvoiceType>(request.Dto.Type),
                Status = InvoiceStatus.Draft,
                Terms = request.Dto.Terms,
                Notes = request.Dto.Notes,
                Currency = request.Dto.Currency ?? "USD",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // Calculate totals
            decimal subTotal = 0;
            int sortOrder = 0;
            var invoiceLines = new List<InvoiceLine>();

            foreach (var lineDto in request.Dto.InvoiceLines)
            {
                var lineTotal = lineDto.Quantity * lineDto.UnitPrice;
                if (lineDto.Discount.HasValue)
                {
                    lineTotal -= lineTotal * (lineDto.Discount.Value / 100);
                }
                subTotal += lineTotal;

                var invoiceLine = new InvoiceLine
                {
                    Id = Guid.CreateVersion7(),
                    InvoiceId = invoice.Id,
                    ProductId = lineDto.ProductId,
                    Description = lineDto.Description,
                    Quantity = lineDto.Quantity,
                    UnitPrice = lineDto.UnitPrice,
                    Discount = lineDto.Discount,
                    TaxRate = lineDto.TaxRate,
                    TotalPrice = lineTotal,
                    Notes = lineDto.Notes,
                    SortOrder = sortOrder++,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                invoiceLines.Add(invoiceLine);
                await _uow.Add(invoiceLine, ct);
            }

            invoice.SubTotal = subTotal;
            invoice.TaxAmount = subTotal * 0.1m;
            invoice.DiscountAmount = request.Dto.DiscountAmount;
            invoice.TotalAmount = subTotal + (invoice.TaxAmount ?? 0) - (invoice.DiscountAmount ?? 0);
            invoice.BalanceDue = invoice.TotalAmount;

            await _uow.Add(invoice, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice created: {InvoiceId} - {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);

            // Return DTO with the already loaded lines
            return MapToDto(invoice, invoiceLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<string> GenerateInvoiceNumber(CancellationToken ct)
    {
        var count = await _uow.Set<Invoice>().CountAsync(ct) + 1;
        return $"INV-{DateTime.Now:yyyyMMdd}-{count:D4}";
    }

    // Map with pre-loaded lines - no DB query
    private InvoiceDto MapToDto(Invoice invoice, List<InvoiceLine> invoiceLines)
    {
        var dto = new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            LeadId = invoice.LeadId,
            CustomerId = invoice.CustomerId,
            OpportunityId = invoice.OpportunityId,
            QuoteId = invoice.QuoteId,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            AmountPaid = invoice.AmountPaid,
            BalanceDue = invoice.BalanceDue,
            Status = invoice.Status.ToString(),
            Type = invoice.Type.ToString(),
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            Currency = invoice.Currency,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt
        };

        dto.InvoiceLines = invoiceLines.Select(l => new InvoiceLineDto
        {
            Id = l.Id,
            Description = l.Description,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            Discount = l.Discount,
            TaxRate = l.TaxRate,
            TotalPrice = l.TotalPrice,
            ProductId = l.ProductId,
            Notes = l.Notes
        }).ToList();

        dto.Payments = new List<PaymentDto>();
        return dto;
    }

    // ✅ NEW: Get DTO using a fresh context (for queries, not after commit)
    public static async Task<InvoiceDto> GetDtoFresh(Guid invoiceId, IUnitOfWork uow, CancellationToken ct)
    {
        var invoice = await uow.Set<Invoice>()
            .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, ct);

        if (invoice == null)
            throw new DomainException($"Invoice with id [{invoiceId}] NOT FOUND.");

        var dto = new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            LeadId = invoice.LeadId,
            CustomerId = invoice.CustomerId,
            OpportunityId = invoice.OpportunityId,
            QuoteId = invoice.QuoteId,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            AmountPaid = invoice.AmountPaid,
            BalanceDue = invoice.BalanceDue,
            Status = invoice.Status.ToString(),
            Type = invoice.Type.ToString(),
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            Currency = invoice.Currency,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt
        };

        var lines = await uow.Set<InvoiceLine>()
            .Where(x => x.InvoiceId == invoiceId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(ct);

        dto.InvoiceLines = lines.Select(l => new InvoiceLineDto
        {
            Id = l.Id,
            Description = l.Description,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            Discount = l.Discount,
            TaxRate = l.TaxRate,
            TotalPrice = l.TotalPrice,
            ProductId = l.ProductId,
            Notes = l.Notes
        }).ToList();

        var payments = await uow.Set<Payment>()
            .Where(x => x.InvoiceId == invoiceId && !x.IsDeleted)
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync(ct);

        dto.Payments = payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            PaymentNumber = p.PaymentNumber,
            InvoiceId = p.InvoiceId,
            PaymentDate = p.PaymentDate,
            Amount = p.Amount,
            Status = p.Status.ToString(),
            Method = p.Method.ToString(),
            ReferenceNumber = p.ReferenceNumber,
            Notes = p.Notes,
            ProcessedDate = p.ProcessedDate,
            IsReconciled = p.IsReconciled,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        return dto;
    }
}

// ============================================================
// INVOICE SEND HANDLER - FIXED
// ============================================================

public class InvoiceSendHandler : IRequestHandler<InvoiceSendCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceSendHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(InvoiceSendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var invoice = await _uow.Set<Invoice>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (invoice == null)
                throw new DomainException($"Invoice with id [{request.Id}] NOT FOUND.");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new DomainException($"Cannot send invoice with status [{invoice.Status}]");

            invoice.Status = InvoiceStatus.Sent;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(invoice);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice sent: {InvoiceId} - {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);

            // ✅ Use the new GetDtoFresh method
            return await InvoiceAddHandler.GetDtoFresh(invoice.Id, _uow, ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// INVOICE MARK PAID HANDLER - FIXED
// ============================================================

public class InvoiceMarkPaidHandler : IRequestHandler<InvoiceMarkPaidCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceMarkPaidHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(InvoiceMarkPaidCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var invoice = await _uow.Set<Invoice>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (invoice == null)
                throw new DomainException($"Invoice with id [{request.Id}] NOT FOUND.");

            if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Cancelled)
                throw new DomainException($"Cannot mark invoice with status [{invoice.Status}] as paid");

            var amountPaid = request.AmountPaid ?? invoice.BalanceDue ?? invoice.TotalAmount;

            invoice.AmountPaid = (invoice.AmountPaid ?? 0) + amountPaid;
            invoice.BalanceDue = invoice.TotalAmount - invoice.AmountPaid;

            if (invoice.BalanceDue <= 0)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidDate = DateTime.UtcNow;
            }
            else if (invoice.BalanceDue < invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.Partial;
            }

            invoice.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(invoice);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice marked as paid: {InvoiceId} - {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);

            // ✅ Use the new GetDtoFresh method
            return await InvoiceAddHandler.GetDtoFresh(invoice.Id, _uow, ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// INVOICE CANCEL HANDLER - FIXED
// ============================================================

public class InvoiceCancelHandler : IRequestHandler<InvoiceCancelCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceCancelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(InvoiceCancelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var invoice = await _uow.Set<Invoice>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (invoice == null)
                throw new DomainException($"Invoice with id [{request.Id}] NOT FOUND.");

            if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Cancelled)
                throw new DomainException($"Cannot cancel invoice with status [{invoice.Status}]");

            invoice.Status = InvoiceStatus.Cancelled;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(invoice);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice cancelled: {InvoiceId} - {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);

            // ✅ Use the new GetDtoFresh method
            return await InvoiceAddHandler.GetDtoFresh(invoice.Id, _uow, ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// INVOICE REFUND HANDLER - FIXED
// ============================================================

public class InvoiceRefundHandler : IRequestHandler<InvoiceRefundCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceRefundHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(InvoiceRefundCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var invoice = await _uow.Set<Invoice>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (invoice == null)
                throw new DomainException($"Invoice with id [{request.Id}] NOT FOUND.");

            if (invoice.Status != InvoiceStatus.Paid)
                throw new DomainException($"Cannot refund invoice with status [{invoice.Status}]. Only Paid invoices can be refunded.");

            if (request.Amount > (invoice.AmountPaid ?? 0))
                throw new DomainException($"Refund amount [{request.Amount}] exceeds paid amount [{invoice.AmountPaid}]");

            invoice.AmountPaid = (invoice.AmountPaid ?? 0) - request.Amount;
            invoice.BalanceDue = invoice.TotalAmount - (invoice.AmountPaid ?? 0);
            invoice.Status = InvoiceStatus.Refunded;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(invoice);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice refunded: {InvoiceId} - {InvoiceNumber}, Amount: {Amount}",
                invoice.Id, invoice.InvoiceNumber, request.Amount);

            // ✅ Use the new GetDtoFresh method
            return await InvoiceAddHandler.GetDtoFresh(invoice.Id, _uow, ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// INVOICE MODIFY HANDLER - FIXED
// ============================================================

public class InvoiceModHandler : IRequestHandler<InvoiceModCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(InvoiceModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var invoice = await _uow.Set<Invoice>()
                .Include(x => x.InvoiceLines)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (invoice == null)
                throw new DomainException($"Invoice with id [{request.Id}] NOT FOUND.");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new DomainException($"Cannot modify invoice with status [{invoice.Status}]");

            if (request.Dto.InvoiceDate.HasValue)
                invoice.InvoiceDate = request.Dto.InvoiceDate.Value;
            if (request.Dto.DueDate.HasValue)
                invoice.DueDate = request.Dto.DueDate.Value;
            if (request.Dto.Type != null)
                invoice.Type = Enum.Parse<InvoiceType>(request.Dto.Type);
            if (request.Dto.Terms != null)
                invoice.Terms = request.Dto.Terms;
            if (request.Dto.Notes != null)
                invoice.Notes = request.Dto.Notes;

            // Update lines if provided
            if (request.Dto.InvoiceLines != null && request.Dto.InvoiceLines.Any())
            {
                var existingLines = await _uow.Set<InvoiceLine>()
                    .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var line in existingLines)
                {
                    line.IsDeleted = true;
                    await _uow.Update(line);
                }

                decimal subTotal = 0;
                int sortOrder = 0;
                var newLines = new List<InvoiceLine>();

                foreach (var lineDto in request.Dto.InvoiceLines)
                {
                    var lineTotal = lineDto.Quantity * lineDto.UnitPrice;
                    if (lineDto.Discount.HasValue)
                    {
                        lineTotal -= lineTotal * (lineDto.Discount.Value / 100);
                    }
                    subTotal += lineTotal;

                    var invoiceLine = new InvoiceLine
                    {
                        Id = Guid.CreateVersion7(),
                        InvoiceId = invoice.Id,
                        ProductId = lineDto.ProductId,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        Discount = lineDto.Discount,
                        TaxRate = lineDto.TaxRate,
                        TotalPrice = lineTotal,
                        Notes = lineDto.Notes,
                        SortOrder = sortOrder++,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    newLines.Add(invoiceLine);
                    await _uow.Add(invoiceLine, ct);
                }

                invoice.SubTotal = subTotal;
                invoice.TaxAmount = subTotal * 0.1m;
                invoice.TotalAmount = subTotal + (invoice.TaxAmount ?? 0) - (invoice.DiscountAmount ?? 0);
                invoice.BalanceDue = invoice.TotalAmount - (invoice.AmountPaid ?? 0);
            }

            invoice.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(invoice);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice updated: {InvoiceId} - {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);

            // ✅ Use the new GetDtoFresh method
            return await InvoiceAddHandler.GetDtoFresh(invoice.Id, _uow, ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// INVOICE DELETE HANDLER
// ============================================================

public class InvoiceDelHandler : IRequestHandler<InvoiceDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InvoiceDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(InvoiceDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var invoice = await _uow.Set<Invoice>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (invoice == null)
                throw new DomainException($"Invoice with id [{request.Id}] NOT FOUND.");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new DomainException($"Cannot delete invoice with status [{invoice.Status}]");

            invoice.IsDeleted = true;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(invoice);
            await _uow.Commit(ct);

            _logger.LogInformation("Invoice deleted: {InvoiceId} - {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}