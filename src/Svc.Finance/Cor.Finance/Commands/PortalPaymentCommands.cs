// Commands/PortalPaymentCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class AddPortalPaymentCmd : IRequest<PortalPaymentDto>
{
    public AddPortalPaymentDto AddDto { get; set; } = new();
}

public class EditPortalPaymentCmd : IRequest<PortalPaymentDto>
{
    public EditPortalPaymentDto EditDto { get; set; } = new();
}

public class UpdatePortalPaymentStatusCmd : IRequest<PortalPaymentDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ProcessPortalPaymentCmd : IRequest<PortalPaymentDto>
{
    public Guid Id { get; set; }
}

public class DeletePortalPaymentCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============================================================
// SHARED MAPPER
// ============================================================

public static class PortalPaymentMapper
{
    public static async Task<PortalPaymentDto> MapToDtoAsync(PortalPayment payment, FinanceDbContext context, CancellationToken ct)
    {
        var invoice = await context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == payment.InvoiceId, ct);
        var vendor = await context.Vendors
            .FirstOrDefaultAsync(x => x.Id == payment.VendorId, ct);

        return new PortalPaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            InvoiceId = payment.InvoiceId,
            InvoiceNumber = invoice?.InvoiceNumber ?? string.Empty,
            VendorId = payment.VendorId,
            VendorName = vendor?.Name ?? string.Empty,
            VendorCode = vendor?.Code ?? string.Empty,
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

// ============================================================
// HANDLERS
// ============================================================

public class AddPortalPaymentHandler : IRequestHandler<AddPortalPaymentCmd, PortalPaymentDto>
{
    private readonly FinanceDbContext _context;

    public AddPortalPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalPaymentDto> Handle(AddPortalPaymentCmd request, CancellationToken ct)
    {
        // Validate invoice exists
        var invoice = await _context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == request.AddDto.InvoiceId && !x.IsDeleted, ct);
        if (invoice == null)
            throw new InvalidOperationException($"Invoice with ID '{request.AddDto.InvoiceId}' not found");

        var payment = new PortalPayment
        {
            Id = Guid.NewGuid(),
            PaymentNumber = $"PAY-{DateTime.Now:yyyyMMdd-HHmmss}",
            InvoiceId = request.AddDto.InvoiceId,
            VendorId = invoice.VendorId,
            Amount = request.AddDto.Amount,
            Currency = request.AddDto.Currency,
            PaymentDate = request.AddDto.PaymentDate,
            PaymentMethod = request.AddDto.PaymentMethod,
            ReferenceNumber = request.AddDto.ReferenceNumber,
            Status = "Pending",
            Remarks = request.AddDto.Remarks,
            BankName = request.AddDto.BankName,
            AccountNumber = request.AddDto.AccountNumber,
            DateAdd = DateTime.UtcNow
        };

        _context.PortalPayments.Add(payment);
        await _context.SaveChangesAsync(ct);

        return await PortalPaymentMapper.MapToDtoAsync(payment, _context, ct);
    }
}

public class EditPortalPaymentHandler : IRequestHandler<EditPortalPaymentCmd, PortalPaymentDto>
{
    private readonly FinanceDbContext _context;

    public EditPortalPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalPaymentDto> Handle(EditPortalPaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.PortalPayments
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (payment == null)
            throw new InvalidOperationException($"Payment with ID '{request.EditDto.Id}' not found");

        if (payment.Status == "Completed" || payment.Status == "Failed")
            throw new InvalidOperationException($"Cannot edit a payment with status '{payment.Status}'");

        payment.Amount = request.EditDto.Amount;
        payment.Currency = request.EditDto.Currency;
        payment.PaymentDate = request.EditDto.PaymentDate;
        payment.PaymentMethod = request.EditDto.PaymentMethod;
        payment.ReferenceNumber = request.EditDto.ReferenceNumber;
        payment.Remarks = request.EditDto.Remarks;
        payment.BankName = request.EditDto.BankName;
        payment.AccountNumber = request.EditDto.AccountNumber;
        payment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await PortalPaymentMapper.MapToDtoAsync(payment, _context, ct);
    }
}

public class UpdatePortalPaymentStatusHandler : IRequestHandler<UpdatePortalPaymentStatusCmd, PortalPaymentDto>
{
    private readonly FinanceDbContext _context;

    public UpdatePortalPaymentStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalPaymentDto> Handle(UpdatePortalPaymentStatusCmd request, CancellationToken ct)
    {
        var payment = await _context.PortalPayments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            throw new InvalidOperationException($"Payment with ID '{request.Id}' not found");

        var oldStatus = payment.Status;
        payment.Status = request.Status;
        payment.DateMod = DateTime.UtcNow;

        if (request.Status == "Processing")
        {
            payment.ProcessedDate = DateTime.UtcNow;
            payment.ProcessedBy = "System";
        }
        else if (request.Status == "Completed")
        {
            payment.CompletedDate = DateTime.UtcNow;
            payment.TransactionId = $"TXN-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            payment.Status = "Completed";

            // Update invoice status to Paid if all payments are completed
            await UpdateInvoiceStatus(payment.InvoiceId, ct);
        }
        else if (request.Status == "Failed")
        {
            payment.FailureReason = "Payment processing failed";
        }
        else if (request.Status == "Cancelled")
        {
            payment.Status = "Cancelled";
        }

        await _context.SaveChangesAsync(ct);

        return await PortalPaymentMapper.MapToDtoAsync(payment, _context, ct);
    }

    private async Task UpdateInvoiceStatus(Guid invoiceId, CancellationToken ct)
    {
        var invoice = await _context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, ct);
        if (invoice == null) return;

        var totalPaid = await _context.PortalPayments
            .Where(x => x.InvoiceId == invoiceId && x.Status == "Completed" && !x.IsDeleted)
            .SumAsync(x => x.Amount, ct);

        if (totalPaid >= invoice.Amount)
        {
            invoice.Status = "Paid";
            invoice.PaymentDate = DateTime.UtcNow;
        }
        else if (totalPaid > 0)
        {
            invoice.Status = "PartiallyPaid";
        }

        await _context.SaveChangesAsync(ct);
    }
}

public class ProcessPortalPaymentHandler : IRequestHandler<ProcessPortalPaymentCmd, PortalPaymentDto>
{
    private readonly FinanceDbContext _context;

    public ProcessPortalPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalPaymentDto> Handle(ProcessPortalPaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.PortalPayments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            throw new InvalidOperationException($"Payment with ID '{request.Id}' not found");

        if (payment.Status != "Pending")
            throw new InvalidOperationException($"Cannot process payment with status '{payment.Status}'");

        // Simulate processing - in real scenario, call payment gateway
        payment.Status = "Processing";
        payment.ProcessedDate = DateTime.UtcNow;
        payment.ProcessedBy = "System";
        payment.DateMod = DateTime.UtcNow;

        // Simulate successful processing (80% success rate for demo)
        var random = new Random();
        var isSuccess = random.Next(0, 10) < 8;

        if (isSuccess)
        {
            payment.Status = "Completed";
            payment.CompletedDate = DateTime.UtcNow;
            payment.TransactionId = $"TXN-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";

            await UpdateInvoiceStatus(payment.InvoiceId, ct);
        }
        else
        {
            payment.Status = "Failed";
            payment.FailureReason = "Payment processing failed - bank declined transaction";
        }

        await _context.SaveChangesAsync(ct);

        return await PortalPaymentMapper.MapToDtoAsync(payment, _context, ct);
    }

    private async Task UpdateInvoiceStatus(Guid invoiceId, CancellationToken ct)
    {
        var invoice = await _context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, ct);
        if (invoice == null) return;

        var totalPaid = await _context.PortalPayments
            .Where(x => x.InvoiceId == invoiceId && x.Status == "Completed" && !x.IsDeleted)
            .SumAsync(x => x.Amount, ct);

        if (totalPaid >= invoice.Amount)
        {
            invoice.Status = "Paid";
            invoice.PaymentDate = DateTime.UtcNow;
        }
        else if (totalPaid > 0)
        {
            invoice.Status = "PartiallyPaid";
        }

        await _context.SaveChangesAsync(ct);
    }
}

public class DeletePortalPaymentHandler : IRequestHandler<DeletePortalPaymentCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeletePortalPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePortalPaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.PortalPayments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            return false;

        if (payment.Status == "Completed" || payment.Status == "Processing")
            throw new InvalidOperationException($"Cannot delete a payment with status '{payment.Status}'");

        payment.IsDeleted = true;
        payment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}