using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Text.Json;

namespace Cor.Procurement.Commands;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreateInvoiceCommandHandler(
        ProcurementDbContext context,
        ILogger<CreateInvoiceCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new invoice");

            // Validate Purchase Order exists
            var po = await _context.PurchaseOrders
                .FirstOrDefaultAsync(p => p.Id == request.CreateDto.PurchaseOrderId && !p.IsDeleted, cancellationToken);

            if (po == null)
                throw new KeyNotFoundException($"Purchase Order with ID '{request.CreateDto.PurchaseOrderId}' not found");

            var invoiceNumber = GenerateInvoiceNumber();

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                PurchaseOrderId = request.CreateDto.PurchaseOrderId,
                PurchaseOrderNumber = po.PurchaseOrderNumber,
                VendorId = po.VendorId ?? Guid.Empty,
                VendorName = po.VendorName,
                Title = request.CreateDto.Title,
                InvoiceDate = request.CreateDto.InvoiceDate != DateTime.MinValue ? request.CreateDto.InvoiceDate : DateTime.UtcNow,
                DueDate = request.CreateDto.DueDate != DateTime.MinValue ? request.CreateDto.DueDate : DateTime.UtcNow.AddDays(30),
                PaymentTerms = request.CreateDto.PaymentTerms ?? "Net 30",
                Notes = request.CreateDto.Notes,
                Status = "Draft",
                NetAmount = 0,
                TaxAmount = 0,
                TotalAmount = 0,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            decimal netAmount = 0;
            decimal taxAmount = 0;

            foreach (var itemDto in request.CreateDto.LineItems)
            {
                var lineTotal = itemDto.Quantity * itemDto.UnitPrice;
                var lineTax = itemDto.TaxAmount ?? 0;

                netAmount += lineTotal;
                taxAmount += lineTax;

                var lineItem = new InvoiceLineItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    PurchaseOrderItemId = itemDto.PurchaseOrderItemId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalAmount = lineTotal,
                    Discount = itemDto.Discount,
                    TaxAmount = lineTax,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.InvoiceLineItems.Add(lineItem);
            }

            invoice.NetAmount = netAmount;
            invoice.TaxAmount = taxAmount;
            invoice.TotalAmount = netAmount + taxAmount;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("invoices_all", cancellationToken);

            _logger.LogInformation("Invoice created successfully: {InvoiceNumber}", invoiceNumber);

            return await MapToDto(invoice, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            throw;
        }
    }

    private string GenerateInvoiceNumber()
    {
        var today = DateTime.UtcNow;
        return $"INV-{today:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private async Task<InvoiceDto> MapToDto(Invoice invoice, CancellationToken ct)
    {
        var lineItems = await _context.InvoiceLineItems
            .Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted)
            .ToListAsync(ct);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PurchaseOrderId = invoice.PurchaseOrderId,
            PurchaseOrderNumber = invoice.PurchaseOrderNumber,
            VendorId = invoice.VendorId,
            VendorName = invoice.VendorName,
            Title = invoice.Title,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            ReceivedDate = invoice.ReceivedDate,
            NetAmount = invoice.NetAmount,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status,
            PaymentTerms = invoice.PaymentTerms,
            Notes = invoice.Notes,
            ApprovedBy = invoice.ApprovedBy,
            ApprovedDate = invoice.ApprovedDate,
            PaidBy = invoice.PaidBy,
            PaidDate = invoice.PaidDate,
            AttachmentCount = invoice.AttachmentCount,
            DateAdd = invoice.DateAdd,
            DateMod = invoice.DateMod,
            RowVersion = invoice.RowVersion,
            LineItems = lineItems.Select(i => new InvoiceLineItemDto
            {
                Id = i.Id,
                PurchaseOrderItemId = i.PurchaseOrderItemId,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalAmount = i.TotalAmount,
                Discount = i.Discount,
                TaxAmount = i.TaxAmount
            }).ToList()
        };
    }
}

public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand, InvoiceDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<UpdateInvoiceStatusCommandHandler> _logger;
    private readonly ICacheService _cache;

    public UpdateInvoiceStatusCommandHandler(
        ProcurementDbContext context,
        ILogger<UpdateInvoiceStatusCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<InvoiceDto> Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating invoice status: {InvoiceId} to {Status}",
                request.StatusDto.Id, request.StatusDto.Status);

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == request.StatusDto.Id && !i.IsDeleted, cancellationToken);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice with ID '{request.StatusDto.Id}' not found");

            // Validate status transition
            ValidateStatusTransition(invoice.Status, request.StatusDto.Status);

            invoice.Status = request.StatusDto.Status;
            invoice.DateMod = DateTime.UtcNow;

            switch (request.StatusDto.Status)
            {
                case "Sent":
                    invoice.ReceivedDate = DateTime.UtcNow;
                    break;
                case "Approved":
                    invoice.ApprovedDate = DateTime.UtcNow;
                    invoice.ApprovedBy = "System";
                    break;
                case "Paid":
                    invoice.PaidDate = DateTime.UtcNow;
                    invoice.PaidBy = "System";
                    break;
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"invoice_{invoice.Id}", cancellationToken);
            await _cache.RemoveAsync("invoices_all", cancellationToken);

            _logger.LogInformation("Invoice status updated successfully: {InvoiceNumber} -> {Status}",
                invoice.InvoiceNumber, invoice.Status);

            return await MapToDto(invoice, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice status");
            throw;
        }
    }

    private void ValidateStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, List<string>>
        {
            { "Draft", new List<string> { "Sent", "Cancelled" } },
            { "Sent", new List<string> { "Verified", "Cancelled" } },
            { "Verified", new List<string> { "Approved", "Rejected" } },
            { "Approved", new List<string> { "Paid", "Cancelled" } },
            { "Paid", new List<string>() },
            { "Rejected", new List<string>() },
            { "Cancelled", new List<string>() }
        };

        if (!validTransitions.ContainsKey(currentStatus))
            throw new InvalidOperationException($"Invalid current status: '{currentStatus}'");

        if (!validTransitions[currentStatus].Contains(newStatus))
            throw new InvalidOperationException($"Cannot transition from '{currentStatus}' to '{newStatus}'");
    }

    private async Task<InvoiceDto> MapToDto(Invoice invoice, CancellationToken ct)
    {
        var lineItems = await _context.InvoiceLineItems
            .Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted)
            .ToListAsync(ct);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PurchaseOrderId = invoice.PurchaseOrderId,
            PurchaseOrderNumber = invoice.PurchaseOrderNumber,
            VendorId = invoice.VendorId,
            VendorName = invoice.VendorName,
            Title = invoice.Title,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            ReceivedDate = invoice.ReceivedDate,
            NetAmount = invoice.NetAmount,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status,
            PaymentTerms = invoice.PaymentTerms,
            Notes = invoice.Notes,
            ApprovedBy = invoice.ApprovedBy,
            ApprovedDate = invoice.ApprovedDate,
            PaidBy = invoice.PaidBy,
            PaidDate = invoice.PaidDate,
            AttachmentCount = invoice.AttachmentCount,
            DateAdd = invoice.DateAdd,
            DateMod = invoice.DateMod,
            RowVersion = invoice.RowVersion,
            LineItems = lineItems.Select(i => new InvoiceLineItemDto
            {
                Id = i.Id,
                PurchaseOrderItemId = i.PurchaseOrderItemId,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalAmount = i.TotalAmount,
                Discount = i.Discount,
                TaxAmount = i.TaxAmount
            }).ToList()
        };
    }
}