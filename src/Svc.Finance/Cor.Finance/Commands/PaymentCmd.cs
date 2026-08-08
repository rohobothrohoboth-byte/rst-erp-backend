using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Helpers;

namespace Cor.Finance.Commands;

public class AddPaymentCmd : IRequest<PaymentDto>
{
    public AddPaymentDto AddDto { get; set; } = default!;
}

public class EditPaymentCmd : IRequest<PaymentDto>
{
    public EditPaymentDto EditDto { get; set; } = default!;
}

public class ProcessPaymentCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class CancelPaymentCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeletePaymentCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class BulkAddPaymentCmd : IRequest<List<PaymentDto>>
{
    public List<AddPaymentDto> AddDtos { get; set; } = new();
}

// ==================== ADD PAYMENT HANDLER ====================
// Commands/PaymentCmd.cs - AddPaymentHandler

public class AddPaymentHandler : IRequestHandler<AddPaymentCmd, PaymentDto>
{
    private readonly FinanceDbContext _context;

    public AddPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDto> Handle(AddPaymentCmd request, CancellationToken ct)
    {
        var paymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        Console.WriteLine("========================================");
        Console.WriteLine("?? PAYMENT CREATION STARTED");
        Console.WriteLine($"?? Payment Number: {paymentNumber}");
        Console.WriteLine($"?? Payment Amount: {request.AddDto.Amount:C}");
        Console.WriteLine($"?? InvoiceId: {request.AddDto.InvoiceId}");
        Console.WriteLine($"?? CustomerId: {request.AddDto.CustomerId}");
        Console.WriteLine($"?? Bank Account ID: {request.AddDto.BankAccountId}");
        Console.WriteLine($"?? Payment Method: {request.AddDto.PaymentMethod}");
        Console.WriteLine("========================================");

        // ? STEP 1: VALIDATE PERIOD EXISTS AND IS OPEN
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new DomainException($"Period with ID {request.AddDto.PeriodId} not found");

        if (period.IsClosed)
            throw new DomainException($"Cannot create payment in a closed period: {period.Name}");

        // ? STEP 2: VALIDATE PAYMENT DATE IS WITHIN PERIOD RANGE
        var paymentDate = request.AddDto.PaymentDate;
        if (paymentDate.Kind == DateTimeKind.Unspecified)
            paymentDate = DateTime.SpecifyKind(paymentDate, DateTimeKind.Utc);
        else if (paymentDate.Kind == DateTimeKind.Local)
            paymentDate = paymentDate.ToUniversalTime();

        var periodStartUtc = period.StartDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(period.StartDate, DateTimeKind.Utc)
            : period.StartDate.ToUniversalTime();

        var periodEndUtc = period.EndDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(period.EndDate, DateTimeKind.Utc)
            : period.EndDate.ToUniversalTime();

        if (paymentDate < periodStartUtc || paymentDate > periodEndUtc)
            throw new DomainException(
                $"Payment date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

        string paymentStatus = "Pending";
        Invoice? invoice = null;
        BankAccount? bankAccount = null;

        // ? STEP 3: Get the bank account or petty cash based on payment method
        if (request.AddDto.PaymentMethod == "Cash")
        {
            bankAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.AccountType == "Cash" && !x.IsDeleted, ct);

            if (bankAccount == null)
            {
                bankAccount = await _context.BankAccounts
                    .FirstOrDefaultAsync(x => x.AccountName.Contains("Petty Cash") && !x.IsDeleted, ct);
            }

            if (bankAccount == null)
            {
                throw new DomainException("No Petty Cash account found. Please create a Petty Cash account with AccountType = 'Cash'.");
            }

            Console.WriteLine($"?? Petty Cash Account Found: {bankAccount.AccountName}");
            Console.WriteLine($"?? Current Balance: {bankAccount.CurrentBalance:C}");

            if (bankAccount.CurrentBalance < request.AddDto.Amount)
            {
                throw new DomainException(
                    $"Insufficient petty cash balance. Available: {bankAccount.CurrentBalance:C}, Required: {request.AddDto.Amount:C}"
                );
            }
        }
        else
        {
            if (request.AddDto.BankAccountId == null || request.AddDto.BankAccountId == Guid.Empty)
            {
                var defaultBank = await _context.BankAccounts
                    .FirstOrDefaultAsync(x => x.AccountName.Contains("Main") && !x.IsDeleted && x.AccountType != "Cash", ct);

                if (defaultBank != null)
                {
                    request.AddDto.BankAccountId = defaultBank.Id;
                }
                else
                {
                    throw new DomainException("Bank account is required for non-cash payments.");
                }
            }

            bankAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.BankAccountId.Value && !x.IsDeleted, ct);

            if (bankAccount == null)
            {
                throw new DomainException($"Bank account with ID {request.AddDto.BankAccountId} not found.");
            }

            Console.WriteLine($"?? Bank Account Found: {bankAccount.AccountName}");
            Console.WriteLine($"?? Current Balance: {bankAccount.CurrentBalance:C}");

            if (bankAccount.CurrentBalance < request.AddDto.Amount)
            {
                throw new DomainException(
                    $"Insufficient balance in account '{bankAccount.AccountName}'. " +
                    $"Available: {bankAccount.CurrentBalance:C}, Required: {request.AddDto.Amount:C}"
                );
            }
        }

        // ? STEP 4: Process invoice if linked
        if (request.AddDto.InvoiceId.HasValue)
        {
            invoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.InvoiceId && !x.IsDeleted, ct);

            if (invoice != null)
            {
                var existingPayments = await _context.Payments
                    .Where(p => p.InvoiceId == request.AddDto.InvoiceId && !p.IsDeleted)
                    .ToListAsync(ct);

                var totalPaidSoFar = existingPayments.Sum(p => p.Amount);
                var totalPaidAfterThis = totalPaidSoFar + request.AddDto.Amount;
                var remainingBalance = invoice.TotalAmount - totalPaidSoFar;

                Console.WriteLine("?? INVOICE SUMMARY:");
                Console.WriteLine($"   Invoice ID: {invoice.Id}");
                Console.WriteLine($"   Invoice Number: {invoice.InvoiceNumber}");
                Console.WriteLine($"   Invoice Total: {invoice.TotalAmount:C}");
                Console.WriteLine($"   Total Paid Before: {totalPaidSoFar:C}");
                Console.WriteLine($"   Remaining Balance: {remainingBalance:C}");

                if (request.AddDto.Amount > remainingBalance)
                {
                    throw new DomainException(
                        $"Payment amount ({request.AddDto.Amount:C}) exceeds remaining balance ({remainingBalance:C})."
                    );
                }

                invoice.PaidAmount = totalPaidAfterThis;
                invoice.DateMod = DateTime.UtcNow;

                if (totalPaidAfterThis >= invoice.TotalAmount)
                {
                    paymentStatus = "Completed";
                    invoice.Status = "Paid";
                }
                else if (totalPaidAfterThis > 0)
                {
                    paymentStatus = "Partially_Paid";
                    invoice.Status = "Partially_Paid";
                }

                Console.WriteLine($"?? Invoice Updated: Status={invoice.Status}, PaidAmount={invoice.PaidAmount:C}");
            }
        }
        else
        {
            paymentStatus = "Completed";
            Console.WriteLine("?? No Invoice ID - Standalone payment");
        }

        // ? STEP 5: DEDUCT from bank account
        if (bankAccount != null)
        {
            var previousBalance = bankAccount.CurrentBalance;
            bankAccount.CurrentBalance -= request.AddDto.Amount;
            bankAccount.DateMod = DateTime.UtcNow;

            Console.WriteLine($"?? BANK ACCOUNT UPDATED:");
            Console.WriteLine($"   Previous Balance: {previousBalance:C}");
            Console.WriteLine($"   Payment Amount: {request.AddDto.Amount:C}");
            Console.WriteLine($"   New Balance: {bankAccount.CurrentBalance:C}");

            var transaction = new BankTransaction
            {
                Id = Guid.CreateVersion7(),
                BankAccountId = bankAccount.Id,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "Payment",
                Amount = -request.AddDto.Amount,
                Description = $"Payment {paymentNumber} - {request.AddDto.Description}",
                Reference = paymentNumber,
                BalanceAfter = bankAccount.CurrentBalance,
                DateAdd = DateTime.UtcNow,
                PeriodId = period.Id
            };
            await _context.BankTransactions.AddAsync(transaction, ct);
            Console.WriteLine($"?? Bank Transaction Recorded: {transaction.Id}");
        }

        // ? STEP 6: Create payment record with PeriodId
        var payment = new Payment
        {
            Id = Guid.CreateVersion7(),
            PaymentNumber = paymentNumber,
            PaymentDate = paymentDate,
            PaymentType = request.AddDto.PaymentType,
            PaymentMethod = request.AddDto.PaymentMethod,
            Amount = request.AddDto.Amount,
            Description = request.AddDto.Description,
            Reference = request.AddDto.Reference,
            Status = paymentStatus,
            PeriodId = period.Id,
            InvoiceId = request.AddDto.InvoiceId,
            CustomerId = request.AddDto.CustomerId,
            VendorId = request.AddDto.VendorId,
            BankAccountId = bankAccount?.Id,
            JournalEntryId = request.AddDto.JournalEntryId,
            BranchId = request.AddDto.BranchId,
            EmployeeId = request.AddDto.EmployeeId,
            DateAdd = DateTime.UtcNow,
            DateMod = null,
            IsDeleted = false
        };

        await _context.Payments.AddAsync(payment, ct);
        await _context.SaveChangesAsync(ct);

        // ? STEP 7: RELOAD payment with all navigation properties
        var paymentWithIncludes = await _context.Payments
            .Include(p => p.Customer)
            .Include(p => p.Vendor)
            .Include(p => p.BankAccount)
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.Id == payment.Id && !p.IsDeleted, ct);

        if (paymentWithIncludes == null)
            throw new DomainException("Payment not found after creation");

        Console.WriteLine("? PAYMENT SAVED SUCCESSFULLY!");
        Console.WriteLine($"   Payment ID: {paymentWithIncludes.Id}");
        Console.WriteLine($"   Period: {period.Name}");
        Console.WriteLine($"   CustomerId: {paymentWithIncludes.CustomerId}");
        Console.WriteLine($"   InvoiceId: {paymentWithIncludes.InvoiceId}");
        Console.WriteLine($"   Final Status: {paymentWithIncludes.Status}");
        Console.WriteLine($"   Account Balance: {bankAccount?.CurrentBalance:C}");
        Console.WriteLine($"   Remaining Invoice Balance: {(invoice != null ? invoice.TotalAmount - invoice.PaidAmount : 0):C}");
        Console.WriteLine("========================================");

        // ? STEP 8: Return complete PaymentDto with all fields
        return new PaymentDto
        {
            Id = paymentWithIncludes.Id,
            PaymentNumber = paymentWithIncludes.PaymentNumber,
            PaymentDate = paymentWithIncludes.PaymentDate,
            PaymentType = paymentWithIncludes.PaymentType,
            PaymentMethod = paymentWithIncludes.PaymentMethod,
            Amount = paymentWithIncludes.Amount,
            Description = paymentWithIncludes.Description,
            Status = paymentWithIncludes.Status,
            Reference = paymentWithIncludes.Reference,
            PeriodId = paymentWithIncludes.PeriodId,
            PeriodName = period.Name,
            InvoiceId = paymentWithIncludes.InvoiceId,
            InvoiceNumber = paymentWithIncludes.Invoice?.InvoiceNumber,
            CustomerId = paymentWithIncludes.CustomerId,
            CustomerName = paymentWithIncludes.Customer?.Name,
            VendorId = paymentWithIncludes.VendorId,
            VendorName = paymentWithIncludes.Vendor?.Name,
            BankAccountId = paymentWithIncludes.BankAccountId,
            BankAccountName = paymentWithIncludes.BankAccount?.AccountName,
            JournalEntryId = paymentWithIncludes.JournalEntryId,
            BranchId = paymentWithIncludes.BranchId,
            EmployeeId = paymentWithIncludes.EmployeeId,
            DateAdd = paymentWithIncludes.DateAdd,
            DateMod = paymentWithIncludes.DateMod
        };
    }
}
// ==================== EDIT PAYMENT HANDLER ====================

public class EditPaymentHandler : IRequestHandler<EditPaymentCmd, PaymentDto>
{
    private readonly FinanceDbContext _context;

    public EditPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDto> Handle(EditPaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (payment == null)
            throw new DomainException($"Payment with ID '{request.EditDto.Id}' not found");

        if (payment.Status == "Completed" || payment.Status == "Cancelled")
            throw new DomainException($"Cannot edit a {payment.Status} payment.");

        // ? VALIDATE PERIOD EXISTS AND IS OPEN (if PeriodId changed)
        if (payment.PeriodId != request.EditDto.PeriodId)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == request.EditDto.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new DomainException($"Period with ID {request.EditDto.PeriodId} not found");

            if (period.IsClosed)
                throw new DomainException($"Cannot move payment to a closed period: {period.Name}");

            // ? VALIDATE PAYMENT DATE IS WITHIN NEW PERIOD RANGE
            if (request.EditDto.PaymentDate < period.StartDate || request.EditDto.PaymentDate > period.EndDate)
                throw new DomainException(
                    $"Payment date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

            payment.PeriodId = period.Id;
        }
        else
        {
            // ? VALIDATE CURRENT PERIOD IS STILL OPEN
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == payment.PeriodId && !p.IsDeleted, ct);

            if (period != null && period.IsClosed)
                throw new DomainException($"Cannot update payment in a closed period: {period.Name}");
        }

        // Update payment fields
        payment.PaymentDate = request.EditDto.PaymentDate;
        payment.PaymentMethod = request.EditDto.PaymentMethod;
        payment.Amount = request.EditDto.Amount;
        payment.Description = request.EditDto.Description;
        payment.Reference = request.EditDto.Reference;
        payment.InvoiceId = request.EditDto.InvoiceId;
        payment.CustomerId = request.EditDto.CustomerId;
        payment.VendorId = request.EditDto.VendorId;
        payment.BankAccountId = request.EditDto.BankAccountId;
        payment.JournalEntryId = request.EditDto.JournalEntryId;
        payment.BranchId = request.EditDto.BranchId;
        payment.EmployeeId = request.EditDto.EmployeeId;
        payment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await MapToDto(payment, ct);
    }

    private async Task<PaymentDto> MapToDto(Payment payment, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == payment.PeriodId && !p.IsDeleted, ct);

        return new PaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            PaymentDate = payment.PaymentDate,
            PaymentType = payment.PaymentType,
            PaymentMethod = payment.PaymentMethod,
            Amount = payment.Amount,
            Description = payment.Description,
            Status = payment.Status,
            PeriodId = payment.PeriodId,
            PeriodName = period?.Name,
            InvoiceId = payment.InvoiceId,
            CustomerId = payment.CustomerId,
            VendorId = payment.VendorId,
            BankAccountId = payment.BankAccountId,
            JournalEntryId = payment.JournalEntryId,
            BranchId = payment.BranchId,
            EmployeeId = payment.EmployeeId,
            DateAdd = payment.DateAdd,
            DateMod = payment.DateMod
        };
    }
}

// ==================== PROCESS PAYMENT HANDLER ====================

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCmd, bool>
{
    private readonly FinanceDbContext _context;

    public ProcessPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ProcessPaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            return false;

        if (payment.Status == "Completed")
            throw new DomainException("Payment is already completed.");

        if (payment.Status == "Cancelled")
            throw new DomainException("Cannot process a cancelled payment.");

        // ? VALIDATE PERIOD IS OPEN BEFORE PROCESSING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == payment.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new DomainException($"Period with ID {payment.PeriodId} not found");

        if (period.IsClosed)
            throw new DomainException($"Cannot process payment in a closed period: {period.Name}");

        payment.Status = "Completed";
        payment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ==================== CANCEL PAYMENT HANDLER ====================

public class CancelPaymentHandler : IRequestHandler<CancelPaymentCmd, bool>
{
    private readonly FinanceDbContext _context;

    public CancelPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CancelPaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            return false;

        if (payment.Status == "Cancelled")
            throw new DomainException("Payment is already cancelled.");

        if (payment.Status == "Completed")
            throw new DomainException("Cannot cancel a completed payment.");

        // ? VALIDATE PERIOD IS OPEN BEFORE CANCELLING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == payment.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new DomainException($"Period with ID {payment.PeriodId} not found");

        if (period.IsClosed)
            throw new DomainException($"Cannot cancel payment in a closed period: {period.Name}");

        payment.Status = "Cancelled";
        payment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ==================== DELETE PAYMENT HANDLER ====================

public class DeletePaymentHandler : IRequestHandler<DeletePaymentCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeletePaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePaymentCmd request, CancellationToken ct)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (payment == null)
            return false;

        if (payment.Status == "Completed")
            throw new DomainException("Cannot delete a completed payment.");

        // ? VALIDATE PERIOD IS OPEN BEFORE DELETING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == payment.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new DomainException($"Period with ID {payment.PeriodId} not found");

        if (period.IsClosed)
            throw new DomainException($"Cannot delete payment in a closed period: {period.Name}");

        payment.IsDeleted = true;
        payment.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ==================== BULK ADD PAYMENT HANDLER ====================

public class BulkAddPaymentHandler : IRequestHandler<BulkAddPaymentCmd, List<PaymentDto>>
{
    private readonly FinanceDbContext _context;

    public BulkAddPaymentHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentDto>> Handle(BulkAddPaymentCmd request, CancellationToken ct)
    {
        var results = new List<PaymentDto>();

        foreach (var dto in request.AddDtos)
        {
            // ? VALIDATE PERIOD FOR EACH PAYMENT
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == dto.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new DomainException($"Period with ID {dto.PeriodId} not found");

            if (period.IsClosed)
                throw new DomainException($"Cannot create payment in a closed period: {period.Name}");

            // Convert date to UTC
            var paymentDate = dto.PaymentDate;
            if (paymentDate.Kind != DateTimeKind.Utc)
                paymentDate = DateTime.SpecifyKind(paymentDate, DateTimeKind.Utc);

            // ? VALIDATE PAYMENT DATE IS WITHIN PERIOD RANGE
            if (paymentDate < period.StartDate || paymentDate > period.EndDate)
                throw new DomainException(
                    $"Payment date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                PaymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
                PaymentDate = paymentDate,
                PaymentType = dto.PaymentType,
                PaymentMethod = dto.PaymentMethod,
                Amount = dto.Amount,
                Description = dto.Description,
                Status = "Pending",
                // ? PeriodId - REQUIRED
                PeriodId = period.Id,
                InvoiceId = dto.InvoiceId,
                CustomerId = dto.CustomerId,
                VendorId = dto.VendorId,
                JournalEntryId = dto.JournalEntryId,
                BankAccountId = dto.BankAccountId,
                BranchId = dto.BranchId,
                EmployeeId = dto.EmployeeId,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(ct);

            // Update invoice paid amount if invoice exists
            if (dto.InvoiceId.HasValue)
            {
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(x => x.Id == dto.InvoiceId.Value && !x.IsDeleted, ct);

                if (invoice != null)
                {
                    invoice.PaidAmount += dto.Amount;
                    if (invoice.PaidAmount >= invoice.TotalAmount)
                    {
                        invoice.Status = "Paid";
                    }
                    invoice.DateMod = DateTime.UtcNow;
                    await _context.SaveChangesAsync(ct);
                }
            }

            results.Add(new PaymentDto
            {
                Id = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                PaymentDate = payment.PaymentDate,
                PaymentType = payment.PaymentType,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Description = payment.Description,
                Status = payment.Status,
                PeriodId = payment.PeriodId,
                PeriodName = period.Name,
                InvoiceId = payment.InvoiceId,
                CustomerId = payment.CustomerId,
                VendorId = payment.VendorId,
                BankAccountId = payment.BankAccountId,
                JournalEntryId = payment.JournalEntryId,
                BranchId = payment.BranchId,
                EmployeeId = payment.EmployeeId,
                DateAdd = payment.DateAdd,
                DateMod = payment.DateMod
            });
        }

        return results;
    }
}