// Cor.CRM/Commands/QuoteCommands.cs

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

public class QuoteAddCmd : IRequest<QuoteDto>
{
    public CreateQuoteDto Dto { get; set; } = default!;
}

public class QuoteModCmd : IRequest<QuoteDto>
{
    public Guid Id { get; set; }
    public UpdateQuoteDto Dto { get; set; } = default!;
}

public class QuoteDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class QuoteSendCmd : IRequest<QuoteDto>
{
    public Guid Id { get; set; }
}

public class QuoteAcceptCmd : IRequest<QuoteDto>
{
    public Guid Id { get; set; }
}

public class QuoteRejectCmd : IRequest<QuoteDto>
{
    public Guid Id { get; set; }
}

public class QuoteConvertCmd : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// QUOTE ADD HANDLER
// ============================================================

public class QuoteAddHandler : IRequestHandler<QuoteAddCmd, QuoteDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public QuoteAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<QuoteDto> Handle(QuoteAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Generate quote number
            var quoteNumber = await GenerateQuoteNumber(ct);

            var quote = new Quote
            {
                Id = Guid.CreateVersion7(),
                QuoteNumber = quoteNumber,
                LeadId = request.Dto.LeadId,
                CustomerId = request.Dto.CustomerId,
                OpportunityId = request.Dto.OpportunityId,
                ValidUntil = request.Dto.ValidUntil.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.ValidUntil.Value)
                    : null,
                TermsAndConditions = request.Dto.TermsAndConditions,
                Notes = request.Dto.Notes,
                ShippingCost = request.Dto.ShippingCost ?? 0,
                DiscountAmount = request.Dto.DiscountAmount ?? 0,
                Status = QuoteStatus.Draft,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            // Calculate totals
            decimal subTotal = 0;
            decimal taxTotal = 0;
            int sortOrder = 0;
            var quoteLines = new List<QuoteLine>();

            foreach (var lineDto in request.Dto.QuoteLines)
            {
                var lineTotal = lineDto.Quantity * lineDto.UnitPrice;
                var discount = lineDto.Discount ?? 0;
                var taxRate = lineDto.TaxRate ?? 0;

                // Apply discount
                var discountedTotal = lineTotal * (1 - (discount / 100));
                // Apply tax
                var taxAmount = discountedTotal * (taxRate / 100);
                var lineFinalTotal = discountedTotal + taxAmount;

                subTotal += discountedTotal;
                taxTotal += taxAmount;

                var quoteLine = new QuoteLine
                {
                    Id = Guid.CreateVersion7(),
                    QuoteId = quote.Id,
                    ProductId = lineDto.ProductId,
                    Description = lineDto.Description,
                   Quantity = (int)lineDto.Quantity,
                    UnitPrice = lineDto.UnitPrice,
                    Discount = lineDto.Discount,
                    TaxRate = lineDto.TaxRate,
                    TotalPrice = lineFinalTotal,
                    Notes = lineDto.Notes,
                    SortOrder = sortOrder++,
                    CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                    UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                    IsDeleted = false
                };
                quoteLines.Add(quoteLine);
                await _uow.Add(quoteLine, ct);
            }

            quote.SubTotal = subTotal;
            quote.TaxAmount = taxTotal;
            quote.TotalAmount = subTotal + taxTotal - (quote.DiscountAmount ?? 0) + (quote.ShippingCost ?? 0);

            await _uow.Add(quote, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote created: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            return MapToDto(quote, quoteLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<string> GenerateQuoteNumber(CancellationToken ct)
    {
        var count = await _uow.Set<Quote>().CountAsync(ct) + 1;
        return $"QT-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
    }

    public static QuoteDto MapToDto(Quote quote, List<QuoteLine> quoteLines)
    {
        var dto = new QuoteDto
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            LeadId = quote.LeadId,
            CustomerId = quote.CustomerId,
            OpportunityId = quote.OpportunityId,
            SubTotal = quote.SubTotal,
            TaxAmount = quote.TaxAmount,
            DiscountAmount = quote.DiscountAmount,
            TotalAmount = quote.TotalAmount,
            ShippingCost = quote.ShippingCost,
            ValidUntil = quote.ValidUntil,
            Status = quote.Status.ToString(),
            TermsAndConditions = quote.TermsAndConditions,
            Notes = quote.Notes,
            ViewCount = quote.ViewCount,
            SentDate = quote.SentDate,
            AcceptedDate = quote.AcceptedDate,
            CreatedAt = quote.CreatedAt,
            UpdatedAt = quote.UpdatedAt,
            QuoteLines = quoteLines.Select(l => new QuoteLineDto
            {
                Id = l.Id,
                QuoteId = l.QuoteId,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                Discount = l.Discount ?? 0,
                TaxRate = l.TaxRate ?? 0,
                TotalPrice = l.TotalPrice,
                ProductId = l.ProductId,
                ProductName = l.Product?.Name,
                Notes = l.Notes,
                SortOrder = l.SortOrder ?? 0
            }).ToList()
        };

        return dto;
    }
}

// ============================================================
// QUOTE MODIFY HANDLER
// ============================================================

public class QuoteModHandler : IRequestHandler<QuoteModCmd, QuoteDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public QuoteModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<QuoteDto> Handle(QuoteModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var quote = await _uow.Set<Quote>()
                .Include(x => x.QuoteLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (quote == null)
                throw new DomainException($"Quote with id [{request.Id}] NOT FOUND.");

            if (quote.Status != QuoteStatus.Draft)
                throw new DomainException($"Cannot modify quote with status [{quote.Status}]");

            // Update basic fields
            if (request.Dto.ValidUntil.HasValue)
                quote.ValidUntil = DateTimeHelper.EnsureUtc(request.Dto.ValidUntil.Value);

            if (request.Dto.TermsAndConditions != null)
                quote.TermsAndConditions = request.Dto.TermsAndConditions;

            if (request.Dto.Notes != null)
                quote.Notes = request.Dto.Notes;

            if (request.Dto.ShippingCost.HasValue)
                quote.ShippingCost = request.Dto.ShippingCost.Value;

            if (request.Dto.DiscountAmount.HasValue)
                quote.DiscountAmount = request.Dto.DiscountAmount.Value;

            // Update lines if provided
            if (request.Dto.QuoteLines != null && request.Dto.QuoteLines.Any())
            {
                // Soft delete existing lines
                var existingLines = await _uow.Set<QuoteLine>()
                    .Where(x => x.QuoteId == quote.Id && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var line in existingLines)
                {
                    line.IsDeleted = true;
                    line.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                    await _uow.Update(line);
                }

                // Add new lines
                decimal subTotal = 0;
                decimal taxTotal = 0;
                int sortOrder = 0;
                var newQuoteLines = new List<QuoteLine>();

                foreach (var lineDto in request.Dto.QuoteLines)
                {
                    var lineTotal = lineDto.Quantity * lineDto.UnitPrice;
                    var discount = lineDto.Discount ?? 0;
                    var taxRate = lineDto.TaxRate ?? 0;

                    var discountedTotal = lineTotal * (1 - (discount / 100));
                    var taxAmount = discountedTotal * (taxRate / 100);
                    var lineFinalTotal = discountedTotal + taxAmount;

                    subTotal += discountedTotal;
                    taxTotal += taxAmount;

                    var quoteLine = new QuoteLine
                    {
                        Id = Guid.CreateVersion7(),
                        QuoteId = quote.Id,
                        ProductId = lineDto.ProductId,
                        Description = lineDto.Description,
                        Quantity = (int)lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        Discount = lineDto.Discount,
                        TaxRate = lineDto.TaxRate,
                        TotalPrice = lineFinalTotal,
                        Notes = lineDto.Notes,
                        SortOrder = sortOrder++,
                        CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        IsDeleted = false
                    };
                    newQuoteLines.Add(quoteLine);
                    await _uow.Add(quoteLine, ct);
                }

                quote.SubTotal = subTotal;
                quote.TaxAmount = taxTotal;
                quote.TotalAmount = subTotal + taxTotal - (quote.DiscountAmount ?? 0) + (quote.ShippingCost ?? 0);
            }

            quote.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(quote);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote updated: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            // ✅ FIX: Use the already loaded quote and its lines
            // Filter out deleted lines
            var activeLines = quote.QuoteLines.Where(l => !l.IsDeleted).ToList();
            return QuoteAddHandler.MapToDto(quote, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// QUOTE DELETE HANDLER
// ============================================================

public class QuoteDelHandler : IRequestHandler<QuoteDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public QuoteDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(QuoteDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var quote = await _uow.Set<Quote>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (quote == null)
                throw new DomainException($"Quote with id [{request.Id}] NOT FOUND.");

            if (quote.Status != QuoteStatus.Draft)
                throw new DomainException($"Cannot delete quote with status [{quote.Status}]");

            quote.IsDeleted = true;
            quote.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(quote);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote deleted: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// QUOTE SEND HANDLER
// ============================================================

public class QuoteSendHandler : IRequestHandler<QuoteSendCmd, QuoteDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public QuoteSendHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<QuoteDto> Handle(QuoteSendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var quote = await _uow.Set<Quote>()
                .Include(x => x.QuoteLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (quote == null)
                throw new DomainException($"Quote with id [{request.Id}] NOT FOUND.");

            if (quote.Status != QuoteStatus.Draft)
                throw new DomainException($"Cannot send quote with status [{quote.Status}]");

            quote.Status = QuoteStatus.Sent;
            quote.SentDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            quote.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(quote);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote sent: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            // ✅ FIX: Use the already loaded quote and its lines
            var activeLines = quote.QuoteLines.Where(l => !l.IsDeleted).ToList();
            return QuoteAddHandler.MapToDto(quote, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// QUOTE ACCEPT HANDLER
// ============================================================

public class QuoteAcceptHandler : IRequestHandler<QuoteAcceptCmd, QuoteDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public QuoteAcceptHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<QuoteDto> Handle(QuoteAcceptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var quote = await _uow.Set<Quote>()
                .Include(x => x.QuoteLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (quote == null)
                throw new DomainException($"Quote with id [{request.Id}] NOT FOUND.");

            if (quote.Status != QuoteStatus.Sent && quote.Status != QuoteStatus.Viewed)
                throw new DomainException($"Cannot accept quote with status [{quote.Status}]");

            quote.Status = QuoteStatus.Accepted;
            quote.AcceptedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            quote.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(quote);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote accepted: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            // ✅ FIX: Use the already loaded quote and its lines
            var activeLines = quote.QuoteLines.Where(l => !l.IsDeleted).ToList();
            return QuoteAddHandler.MapToDto(quote, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// QUOTE REJECT HANDLER
// ============================================================

public class QuoteRejectHandler : IRequestHandler<QuoteRejectCmd, QuoteDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public QuoteRejectHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<QuoteDto> Handle(QuoteRejectCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var quote = await _uow.Set<Quote>()
                .Include(x => x.QuoteLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (quote == null)
                throw new DomainException($"Quote with id [{request.Id}] NOT FOUND.");

            if (quote.Status != QuoteStatus.Sent && quote.Status != QuoteStatus.Viewed)
                throw new DomainException($"Cannot reject quote with status [{quote.Status}]");

            quote.Status = QuoteStatus.Rejected;
            quote.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(quote);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote rejected: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            // ✅ FIX: Use the already loaded quote and its lines
            var activeLines = quote.QuoteLines.Where(l => !l.IsDeleted).ToList();
            return QuoteAddHandler.MapToDto(quote, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// QUOTE CONVERT TO INVOICE HANDLER
// ============================================================

public class QuoteConvertHandler : IRequestHandler<QuoteConvertCmd, InvoiceDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IMediator _mediator;

    public QuoteConvertHandler(IUnitOfWork uow, ILogService logger, IMediator mediator)
    {
        _uow = uow;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<InvoiceDto> Handle(QuoteConvertCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var quote = await _uow.Set<Quote>()
                .Include(x => x.QuoteLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (quote == null)
                throw new DomainException($"Quote with id [{request.Id}] NOT FOUND.");

            if (quote.Status != QuoteStatus.Accepted)
                throw new DomainException($"Cannot convert quote with status [{quote.Status}]. Only Accepted quotes can be converted.");

            // Create invoice from quote
            var createInvoiceDto = new CreateInvoiceDto
            {
                CustomerId = quote.CustomerId,
                LeadId = quote.LeadId,
                OpportunityId = quote.OpportunityId,
                QuoteId = quote.Id,
                InvoiceDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                DueDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow.AddDays(30)),
                Type = "Sales",
                Terms = quote.TermsAndConditions,
                Notes = $"Converted from Quote: {quote.QuoteNumber}\n{quote.Notes}",
                Currency = "USD",
                DiscountAmount = quote.DiscountAmount,
                InvoiceLines = quote.QuoteLines.Where(q => !q.IsDeleted).Select(q => new CreateInvoiceLineDto
                {
                    Description = q.Description,
                    Quantity = q.Quantity,
                    UnitPrice = q.UnitPrice,
                    Discount = q.Discount,
                    TaxRate = q.TaxRate,
                    ProductId = q.ProductId,
                    Notes = q.Notes
                }).ToList()
            };

            // Update quote status to Converted
            quote.Status = QuoteStatus.Converted;
            quote.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            await _uow.Update(quote);

            // Save the quote status change
            await _uow.SaveChangesAsync(ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Quote converted to invoice: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            // Create invoice using MediatR with a fresh context
            var invoice = await _mediator.Send(new InvoiceAddCmd { Dto = createInvoiceDto }, ct);
            return invoice;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}