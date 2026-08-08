using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ==================== INVOICE COMMANDS ====================

public class AddInvoiceCmd : IRequest<InvoiceDto>
{
    public AddInvoiceDto AddDto { get; set; } = default!;
}

public class EditInvoiceCmd : IRequest<InvoiceDto>
{
    public EditInvoiceDto EditDto { get; set; } = default!;
}

public class DeleteInvoiceCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class UpdateInvoiceStatusCmd : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
}

// Add to Cor.Finance/Commands/InvoiceCmd.cs

public class BulkAddInvoiceCmd : IRequest<List<InvoiceDto>>
{
    public List<AddInvoiceDto> AddDtos { get; set; } = new();
}

public class BulkUpdateInvoiceStatusCmd : IRequest<bool>
{
    public List<InvoiceStatusUpdateDto> Updates { get; set; } = new();
}

// ==================== BULK HANDLERS ====================

// In InvoiceCmd.cs - Update BulkAddInvoiceHandler

public class BulkAddInvoiceHandler : IRequestHandler<BulkAddInvoiceCmd, List<InvoiceDto>>
{
    private readonly FinanceDbContext _context;

    public BulkAddInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceDto>> Handle(BulkAddInvoiceCmd request, CancellationToken ct)
    {
        var results = new List<InvoiceDto>();

        foreach (var dto in request.AddDtos)
        {
            // ? Validate based on InvoiceType
            if (dto.InvoiceType == "Sales" && !dto.CustomerId.HasValue)
            {
                throw new InvalidOperationException($"CustomerId is required for Sales invoice: {dto.InvoiceNumber}");
            }

            if (dto.InvoiceType == "Purchase" && !dto.VendorId.HasValue)
            {
                throw new InvalidOperationException($"VendorId is required for Purchase invoice: {dto.InvoiceNumber}");
            }

            var invoiceNumber = string.IsNullOrEmpty(dto.InvoiceNumber)
                ? $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}"
                : dto.InvoiceNumber;

            var totalAmount = dto.SubTotal + dto.TaxAmount - dto.DiscountAmount;

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                DueDate = dto.DueDate,
                SubTotal = dto.SubTotal,
                TaxAmount = dto.TaxAmount,
                DiscountAmount = dto.DiscountAmount,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                Status = "Draft",
                Notes = dto.Notes,
                InvoiceType = dto.InvoiceType,
                VendorId = dto.InvoiceType == "Purchase" ? dto.VendorId : null,
                CustomerId = dto.InvoiceType == "Sales" ? dto.CustomerId : null,
                SalesRep = dto.InvoiceType == "Sales" ? dto.SalesRep : null,
                DeliveryDate = dto.InvoiceType == "Sales" ? dto.DeliveryDate : null,
                PurchaseOrderId = dto.InvoiceType == "Purchase" ? dto.PurchaseOrderId : null,
                ReceivedDate = dto.InvoiceType == "Purchase" ? dto.ReceivedDate : null,
                BranchId = dto.BranchId,
                DepartmentId = dto.DepartmentId,
                EmployeeId = dto.EmployeeId,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false,
                PeriodId = dto.PeriodId
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(ct);

            // Add lines
            if (dto.Lines != null && dto.Lines.Any())
            {
                foreach (var lineDto in dto.Lines)
                {
                    var lineTotal = (lineDto.Quantity * lineDto.UnitPrice) - lineDto.Discount;
                    var lineTax = lineTotal * (lineDto.TaxRate / 100);

                    var line = new InvoiceLine
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = invoice.Id,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        Discount = lineDto.Discount,
                        TaxRate = lineDto.TaxRate,
                        TotalAmount = lineTotal + lineTax,
                        DateAdd = DateTime.UtcNow,
                        DateMod = null,
                        IsDeleted = false
                    };
                    _context.InvoiceLines.Add(line);
                }
                await _context.SaveChangesAsync(ct);
            }

            results.Add(await MapToDto(invoice, ct));
        }

        return results;
    }

    private async Task<InvoiceDto> MapToDto(Invoice invoice, CancellationToken ct)
    {
        // ... same mapping as AddInvoiceHandler
        var lines = await _context.InvoiceLines
            .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
            .ToListAsync(ct);

        var payments = await _context.Payments
            .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
            .ToListAsync(ct);

        string vendorName = "";
        string customerName = "";

        if (invoice.VendorId.HasValue)
        {
            var vendor = await _context.Vendors
                .Where(x => x.Id == invoice.VendorId && !x.IsDeleted)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(ct);
            vendorName = vendor ?? "";
        }

        if (invoice.CustomerId.HasValue)
        {
            var customer = await _context.Customers
                .Where(x => x.Id == invoice.CustomerId && !x.IsDeleted)
                .Select(x => x.Name)
                .FirstOrDefaultAsync(ct);
            customerName = customer ?? "";
        }

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            BalanceDue = invoice.TotalAmount - invoice.PaidAmount,
            Status = invoice.Status,
            Notes = invoice.Notes,
            InvoiceType = invoice.InvoiceType,
            VendorId = invoice.VendorId,
            VendorName = vendorName,
            CustomerId = invoice.CustomerId,
            CustomerName = customerName,
            SalesRep = invoice.SalesRep,
            DeliveryDate = invoice.DeliveryDate,
            PurchaseOrderId = invoice.PurchaseOrderId,
            ReceivedDate = invoice.ReceivedDate,
            BranchId = invoice.BranchId,
            DepartmentId = invoice.DepartmentId,
            EmployeeId = invoice.EmployeeId,
            Lines = lines.Select(line => new InvoiceLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                TotalAmount = line.TotalAmount,
                PeriodId = line.PeriodId
            }).ToList(),
            Payments = payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                Status = payment.Status,
                PeriodId = payment.PeriodId
            }).ToList(),
            DateAdd = invoice.DateAdd,
            DateMod = invoice.DateMod,
            PeriodId = invoice.PeriodId
        };
    }
}
public class BulkUpdateInvoiceStatusHandler : IRequestHandler<BulkUpdateInvoiceStatusCmd, bool>
{
    private readonly FinanceDbContext _context;

    public BulkUpdateInvoiceStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(BulkUpdateInvoiceStatusCmd request, CancellationToken ct)
    {
        foreach (var update in request.Updates)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == update.Id && !x.IsDeleted, ct);

            if (invoice == null)
                continue;

            invoice.Status = update.Status;
            invoice.DateMod = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
// InvoiceCmd.cs - Only the AddInvoiceHandler update (the rest stays the same)

// ==================== ADD INVOICE HANDLER ====================
public class AddInvoiceHandler : IRequestHandler<AddInvoiceCmd, InvoiceDto>
{
    private readonly FinanceDbContext _context;

    public AddInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

     public async Task<InvoiceDto> Handle(AddInvoiceCmd request, CancellationToken ct)
       {
           // 1. Validate lines exist
           if (request.AddDto.Lines == null || !request.AddDto.Lines.Any())
               throw new InvalidOperationException("Invoice must have at least one line");

           // 2. Validate invoice type has correct party (Vendor or Customer)
           if (request.AddDto.InvoiceType == "Purchase" && !request.AddDto.VendorId.HasValue)
               throw new InvalidOperationException("Purchase invoice requires a VendorId");

           if (request.AddDto.InvoiceType == "Sales" && !request.AddDto.CustomerId.HasValue)
               throw new InvalidOperationException("Sales invoice requires a CustomerId");

           // 3. Validate Period exists and is OPEN
           var period = await _context.FinancialPeriods
               .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

           if (period == null)
               throw new InvalidOperationException($"Period with ID {request.AddDto.PeriodId} not found");

           if (period.IsClosed)
               throw new InvalidOperationException($"Cannot create invoice in a closed period: {period.Name}");

           // 4. Ensure InvoiceDate is UTC
           var invoiceDate = request.AddDto.InvoiceDate;
           if (invoiceDate.Kind == DateTimeKind.Unspecified)
               invoiceDate = DateTime.SpecifyKind(invoiceDate, DateTimeKind.Utc);
           else if (invoiceDate.Kind == DateTimeKind.Local)
               invoiceDate = invoiceDate.ToUniversalTime();

           // 5. Ensure Period dates are UTC for comparison
           var periodStartUtc = period.StartDate.Kind == DateTimeKind.Unspecified
               ? DateTime.SpecifyKind(period.StartDate, DateTimeKind.Utc)
               : period.StartDate.ToUniversalTime();

           var periodEndUtc = period.EndDate.Kind == DateTimeKind.Unspecified
               ? DateTime.SpecifyKind(period.EndDate, DateTimeKind.Utc)
               : period.EndDate.ToUniversalTime();

           // 6. Validate invoice date is within period range
           if (invoiceDate < periodStartUtc || invoiceDate > periodEndUtc)
               throw new InvalidOperationException(
                   $"Invoice date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

           // 7. Generate invoice number if not provided
           var invoiceNumber = request.AddDto.InvoiceNumber;
           if (string.IsNullOrEmpty(invoiceNumber))
           {
               var prefix = request.AddDto.InvoiceType == "Sales" ? "INV-S" : "INV-P";
               var lastInvoice = await _context.Invoices
                   .Where(x => x.InvoiceNumber.StartsWith(prefix) && !x.IsDeleted)
                   .OrderByDescending(x => x.InvoiceNumber)
                   .FirstOrDefaultAsync(ct);

               var lastNumber = 0;
               if (lastInvoice != null)
               {
                   var numberPart = lastInvoice.InvoiceNumber.Replace(prefix, "");
                   int.TryParse(numberPart, out lastNumber);
               }
               invoiceNumber = $"{prefix}{(lastNumber + 1):D6}";
           }

           // 8. Create the invoice
           var invoice = new Invoice
           {
               Id = Guid.NewGuid(),
               InvoiceNumber = invoiceNumber,
               InvoiceDate = invoiceDate,
               DueDate = request.AddDto.DueDate,
               SubTotal = request.AddDto.SubTotal,
               TaxAmount = request.AddDto.TaxAmount,
               DiscountAmount = request.AddDto.DiscountAmount,
               TotalAmount = request.AddDto.TotalAmount,
               PaidAmount = 0,
               Status = "Draft",
               Notes = request.AddDto.Notes,
               InvoiceType = request.AddDto.InvoiceType,
               VendorId = request.AddDto.VendorId,
               CustomerId = request.AddDto.CustomerId,
               PeriodId = period.Id,
               BranchId = request.AddDto.BranchId,
               DepartmentId = request.AddDto.DepartmentId,
               EmployeeId = request.AddDto.EmployeeId,
               SalesRep = request.AddDto.SalesRep,
               DeliveryDate = request.AddDto.DeliveryDate,
               PurchaseOrderId = request.AddDto.PurchaseOrderId,
               ReceivedDate = request.AddDto.ReceivedDate,
               DateAdd = DateTime.UtcNow,
               DateMod = null,
               IsDeleted = false
           };

           // 9. ? Add lines with PeriodId
           foreach (var lineDto in request.AddDto.Lines)
           {
               invoice.Lines.Add(new InvoiceLine
               {
                   Id = Guid.NewGuid(),
                   InvoiceId = invoice.Id,
                   Description = lineDto.Description,
                   Quantity = lineDto.Quantity,
                   UnitPrice = lineDto.UnitPrice,
                   Discount = lineDto.Discount,
                   TaxRate = lineDto.TaxRate,
                   TotalAmount = lineDto.TotalAmount,
                   PeriodId = period.Id,  // ? CRITICAL: Set PeriodId on each line
                   DateAdd = DateTime.UtcNow,
                   DateMod = null,
                   IsDeleted = false
               });
           }

           // 10. Save to database
           await _context.Invoices.AddAsync(invoice, ct);
           await _context.SaveChangesAsync(ct);

           // 11. Return the created invoice
           return await MapToDto(invoice, ct);
       }
    private async Task<string> GenerateInvoiceNumber(CancellationToken ct)
        {
            // Get the last invoice number
            var lastInvoice = await _context.Invoices
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.InvoiceNumber)
                .FirstOrDefaultAsync(ct);

            if (lastInvoice == null)
            {
                return "A000001";  // First invoice
            }

            var lastNumber = lastInvoice.InvoiceNumber;

            // Extract letter and number (e.g., "A000001" -> "A", "000001")
            var letter = lastNumber.Substring(0, 1);
            var numberPart = lastNumber.Substring(1);

            if (int.TryParse(numberPart, out int number))
            {
                number++;

                // If number exceeds 999999, move to next letter
                if (number > 999999)
                {
                    // Move to next letter (A -> B, B -> C, etc.)
                    char nextLetter = (char)(letter[0] + 1);
                    return $"{nextLetter}000001";
                }

                return $"{letter}{number:D6}";  // Format with 6 digits (e.g., A000001)
            }

            return "A000001";
        }

  private async Task<InvoiceDto> MapToDto(Invoice invoice, CancellationToken ct)
  {
      var lines = await _context.InvoiceLines
          .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
          .ToListAsync(ct);

      var payments = await _context.Payments
          .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
          .ToListAsync(ct);

      // ? Load period for PeriodName
      var period = await _context.FinancialPeriods
          .FirstOrDefaultAsync(p => p.Id == invoice.PeriodId && !p.IsDeleted, ct);

      string vendorName = "";
      string customerName = "";

      if (invoice.VendorId.HasValue)
      {
          var vendor = await _context.Vendors
              .Where(x => x.Id == invoice.VendorId && !x.IsDeleted)
              .Select(x => x.Name)
              .FirstOrDefaultAsync(ct);
          vendorName = vendor ?? "";
      }

      if (invoice.CustomerId.HasValue)
      {
          var customer = await _context.Customers
              .Where(x => x.Id == invoice.CustomerId && !x.IsDeleted)
              .Select(x => x.Name)
              .FirstOrDefaultAsync(ct);
          customerName = customer ?? "";
      }

      // ? Build payment DTOs with all fields
      var paymentDtos = new List<PaymentDto>();
      foreach (var payment in payments)
      {
          // Get customer/vendor names for each payment
          string paymentCustomerName = "";
          string paymentVendorName = "";

          if (payment.CustomerId.HasValue)
          {
              var cust = await _context.Customers
                  .Where(x => x.Id == payment.CustomerId && !x.IsDeleted)
                  .Select(x => x.Name)
                  .FirstOrDefaultAsync(ct);
              paymentCustomerName = cust ?? "";
          }

          if (payment.VendorId.HasValue)
          {
              var vend = await _context.Vendors
                  .Where(x => x.Id == payment.VendorId && !x.IsDeleted)
                  .Select(x => x.Name)
                  .FirstOrDefaultAsync(ct);
              paymentVendorName = vend ?? "";
          }

          paymentDtos.Add(new PaymentDto
          {
              Id = payment.Id,
              PaymentNumber = payment.PaymentNumber,
              PaymentDate = payment.PaymentDate,
              PaymentType = payment.PaymentType,
              PaymentMethod = payment.PaymentMethod,
              Amount = payment.Amount,
              Description = payment.Description,
              Status = payment.Status,
              Reference = payment.Reference,
              VendorId = payment.VendorId,
              VendorName = paymentVendorName,
              CustomerId = payment.CustomerId,
              CustomerName = paymentCustomerName,
              InvoiceId = payment.InvoiceId,
              BankAccountId = payment.BankAccountId,
              BranchId = payment.BranchId,
              EmployeeId = payment.EmployeeId,
              PeriodId = payment.PeriodId,
              PeriodName = period?.Name ?? "Unknown Period",
              DateAdd = payment.DateAdd,
              DateMod = payment.DateMod
          });
      }

      return new InvoiceDto
      {
          Id = invoice.Id,
          InvoiceNumber = invoice.InvoiceNumber,
          InvoiceDate = invoice.InvoiceDate,
          DueDate = invoice.DueDate,
          SubTotal = invoice.SubTotal,
          TaxAmount = invoice.TaxAmount,
          DiscountAmount = invoice.DiscountAmount,
          TotalAmount = invoice.TotalAmount,
          PaidAmount = invoice.PaidAmount,
          BalanceDue = invoice.TotalAmount - invoice.PaidAmount,
          Status = invoice.Status,
          Notes = invoice.Notes,
          InvoiceType = invoice.InvoiceType,
          PeriodId = invoice.PeriodId,
          PeriodName = period?.Name ?? "Unknown Period",
          VendorId = invoice.VendorId,
          VendorName = vendorName,
          CustomerId = invoice.CustomerId,
          CustomerName = customerName,
          SalesRep = invoice.SalesRep,
          DeliveryDate = invoice.DeliveryDate,
          PurchaseOrderId = invoice.PurchaseOrderId,
          ReceivedDate = invoice.ReceivedDate,
          BranchId = invoice.BranchId,
          DepartmentId = invoice.DepartmentId,
          EmployeeId = invoice.EmployeeId,
          Lines = lines.Select(line => new InvoiceLineDto
          {
              Id = line.Id,
              Description = line.Description,
              Quantity = line.Quantity,
              UnitPrice = line.UnitPrice,
              Discount = line.Discount,
              TaxRate = line.TaxRate,
              TotalAmount = line.TotalAmount,
              PeriodId = line.PeriodId,
              PeriodName = period?.Name ?? "Unknown Period"
          }).ToList(),
          Payments = paymentDtos,
          DateAdd = invoice.DateAdd,
          DateMod = invoice.DateMod
      };
  }

}
// ==================== EDIT INVOICE HANDLER ====================

public class EditInvoiceHandler : IRequestHandler<EditInvoiceCmd, InvoiceDto>
{
    private readonly FinanceDbContext _context;

    public EditInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDto> Handle(EditInvoiceCmd request, CancellationToken ct)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice with ID '{request.EditDto.Id}' not found");

        if (invoice.Status == "Paid")
            throw new InvalidOperationException("Cannot update a paid invoice.");

        // Update basic fields
        invoice.InvoiceDate = request.EditDto.InvoiceDate;
        invoice.DueDate = request.EditDto.DueDate;
        invoice.SubTotal = request.EditDto.SubTotal;
        invoice.TaxAmount = request.EditDto.TaxAmount;
        invoice.DiscountAmount = request.EditDto.DiscountAmount;
        invoice.TotalAmount = request.EditDto.SubTotal + request.EditDto.TaxAmount - request.EditDto.DiscountAmount;
        invoice.Notes = request.EditDto.Notes;
        invoice.VendorId = request.EditDto.VendorId;
        invoice.BranchId = request.EditDto.BranchId;
        invoice.DepartmentId = request.EditDto.DepartmentId;
        invoice.EmployeeId = request.EditDto.EmployeeId;
        invoice.PeriodId = request.EditDto.PeriodId;
        invoice.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        // Update lines if provided
        if (request.EditDto.Lines != null && request.EditDto.Lines.Any())
        {
            // Remove existing lines
            var existingLines = await _context.InvoiceLines
                .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var line in existingLines)
            {
                line.IsDeleted = true;
                line.DateMod = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync(ct);

            // Add new lines
            foreach (var lineDto in request.EditDto.Lines)
            {
                var lineTotal = (lineDto.Quantity * lineDto.UnitPrice) - lineDto.Discount;
                var lineTax = lineTotal * (lineDto.TaxRate / 100);

                var line = new InvoiceLine
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    Description = lineDto.Description,
                    Quantity = lineDto.Quantity,
                    UnitPrice = lineDto.UnitPrice,
                    Discount = lineDto.Discount,
                    TaxRate = lineDto.TaxRate,
                    TotalAmount = lineTotal + lineTax,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false,
                    PeriodId = lineDto.PeriodId
                };
                _context.InvoiceLines.Add(line);
            }
            await _context.SaveChangesAsync(ct);
        }

        return await MapToDto(invoice, ct);
    }

    private async Task<InvoiceDto> MapToDto(Invoice invoice, CancellationToken ct)
    {
        var lines = await _context.InvoiceLines
            .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
            .ToListAsync(ct);

        var payments = await _context.Payments
            .Where(x => x.InvoiceId == invoice.Id && !x.IsDeleted)
            .ToListAsync(ct);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            BalanceDue = invoice.TotalAmount - invoice.PaidAmount,
            Status = invoice.Status,
            Notes = invoice.Notes,
            VendorId = invoice.VendorId,
            BranchId = invoice.BranchId,
            DepartmentId = invoice.DepartmentId,
            EmployeeId = invoice.EmployeeId,
            Lines = lines.Select(line => new InvoiceLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                TotalAmount = line.TotalAmount,
                PeriodId = line.PeriodId
            }).ToList(),
            Payments = payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                Status = payment.Status,
                PeriodId = payment.PeriodId
            }).ToList(),
            DateAdd = invoice.DateAdd,
            DateMod = invoice.DateMod,
            PeriodId = invoice.PeriodId
        };
    }
}

// ==================== DELETE INVOICE HANDLER ====================

public class DeleteInvoiceHandler : IRequestHandler<DeleteInvoiceCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteInvoiceCmd request, CancellationToken ct)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (invoice == null)
            return false;

        if (invoice.Status == "Paid")
            throw new InvalidOperationException("Cannot delete a paid invoice.");

        invoice.IsDeleted = true;
        invoice.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ==================== UPDATE INVOICE STATUS HANDLER ====================

public class UpdateInvoiceStatusHandler : IRequestHandler<UpdateInvoiceStatusCmd, bool>
{
    private readonly FinanceDbContext _context;

    public UpdateInvoiceStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateInvoiceStatusCmd request, CancellationToken ct)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (invoice == null)
            return false;

        invoice.Status = request.Status;
        invoice.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}