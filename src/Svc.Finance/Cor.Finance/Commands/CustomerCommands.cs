// Commands/CustomerCommands.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using System.Text.Json;

namespace Cor.Finance.Commands;

// ============================================================
// COMMAND DEFINITIONS
// ============================================================

public class CreateCustomerCmd : IRequest<CustomerDto>
{
    public CustomerCreateDto Customer { get; set; } = new();
}

public class UpdateCustomerCmd : IRequest<CustomerDto>
{
    public CustomerUpdateDto Customer { get; set; } = new();
}

public class DeleteCustomerCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleCustomerStatusCmd : IRequest<CustomerDto>
{
    public Guid Id { get; set; }
}

public class BulkCreateCustomersCmd : IRequest<BulkOperationResultDto>
{
    public List<CustomerCreateDto> Customers { get; set; } = new();
}

// ============================================================
// COMMAND HANDLERS
// ============================================================

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCmd, CustomerDto>
{
    private readonly FinanceDbContext _context;

    public CreateCustomerHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCmd request, CancellationToken ct)
    {
        // Generate customer code if not provided
        if (string.IsNullOrEmpty(request.Customer.Code))
        {
            request.Customer.Code = await GenerateCustomerCode(ct);
        }

        // Check for duplicate code
        var exists = await _context.Customers
            .AnyAsync(x => x.Code == request.Customer.Code && !x.IsDeleted, ct);

        if (exists)
            throw new InvalidOperationException($"Customer with code '{request.Customer.Code}' already exists");

        // Validate required fields
        if (string.IsNullOrEmpty(request.Customer.Name))
            throw new ArgumentException("Customer name is required");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Code = request.Customer.Code,
            Name = request.Customer.Name,
            NameAm = request.Customer.NameAm,
            Email = request.Customer.Email,
            Phone = request.Customer.Phone,
            Mobile = request.Customer.Mobile,
            Address = request.Customer.Address,
            City = request.Customer.City,
            Country = request.Customer.Country,
            TaxId = request.Customer.TaxId,
            CustomerType = request.Customer.CustomerType ?? "Regular",
            Status = request.Customer.Status ?? "Active",
            PaymentTerms = request.Customer.PaymentTerms ?? "Net 30",
            Currency = request.Customer.Currency ?? "USD",
            CreditLimit = request.Customer.CreditLimit,
            SalesRep = request.Customer.SalesRep,
            ContactPerson = request.Customer.ContactPerson != null
                ? JsonSerializer.Serialize(request.Customer.ContactPerson)
                : null,
            IsActive = request.Customer.IsActive,
            IsDeleted = false,
            DateAdd = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(ct);

        return MapToDto(customer);
    }

    private async Task<string> GenerateCustomerCode(CancellationToken ct)
    {
        var lastCustomer = await _context.Customers
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(ct);

        if (lastCustomer == null)
            return "CUST-001";

        var lastNumber = int.Parse(lastCustomer.Code.Split('-')[1]);
        return $"CUST-{(lastNumber + 1):D3}";
    }

    // ✅ PUBLIC STATIC - accessible from other handlers
    public static CustomerDto MapToDto(Customer customer)
    {
        CustomerContactPersonDto? contactPerson = null;
        if (!string.IsNullOrEmpty(customer.ContactPerson))
        {
            try
            {
                contactPerson = JsonSerializer.Deserialize<CustomerContactPersonDto>(customer.ContactPerson);
            }
            catch { /* Ignore deserialization errors */ }
        }

        return new CustomerDto
        {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            NameAm = customer.NameAm,
            Email = customer.Email,
            Phone = customer.Phone,
            Mobile = customer.Mobile,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            TaxId = customer.TaxId,
            CustomerType = customer.CustomerType,
            Status = customer.Status,
            PaymentTerms = customer.PaymentTerms,
            Currency = customer.Currency,
            CreditLimit = customer.CreditLimit,
            SalesRep = customer.SalesRep,
            ContactPerson = contactPerson,
            IsActive = customer.IsActive,
            IsDeleted = customer.IsDeleted,
            DateAdd = customer.DateAdd,
            DateMod = customer.DateMod
        };
    }
}

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCmd, CustomerDto>
{
    private readonly FinanceDbContext _context;

    public UpdateCustomerHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> Handle(UpdateCustomerCmd request, CancellationToken ct)
    {
        // ✅ FIX: Use Id directly (not nullable)
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == request.Customer.Id && !x.IsDeleted, ct);

        if (customer == null)
            throw new InvalidOperationException($"Customer with ID '{request.Customer.Id}' not found");

        // Check for duplicate code (excluding this customer)
        if (!string.IsNullOrEmpty(request.Customer.Code))
        {
            var exists = await _context.Customers
                .AnyAsync(x => x.Code == request.Customer.Code && x.Id != request.Customer.Id && !x.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Customer with code '{request.Customer.Code}' already exists");
        }

        // Update fields
        if (!string.IsNullOrEmpty(request.Customer.Code))
            customer.Code = request.Customer.Code;

        customer.Name = request.Customer.Name;
        customer.NameAm = request.Customer.NameAm;
        customer.Email = request.Customer.Email;
        customer.Phone = request.Customer.Phone;
        customer.Mobile = request.Customer.Mobile;
        customer.Address = request.Customer.Address;
        customer.City = request.Customer.City;
        customer.Country = request.Customer.Country;
        customer.TaxId = request.Customer.TaxId;
        customer.CustomerType = request.Customer.CustomerType ?? customer.CustomerType;
        customer.Status = request.Customer.Status ?? customer.Status;
        customer.PaymentTerms = request.Customer.PaymentTerms ?? customer.PaymentTerms;
        customer.Currency = request.Customer.Currency ?? customer.Currency;
        customer.CreditLimit = request.Customer.CreditLimit;
        customer.SalesRep = request.Customer.SalesRep;

        if (request.Customer.ContactPerson != null)
        {
            customer.ContactPerson = JsonSerializer.Serialize(request.Customer.ContactPerson);
        }

        customer.IsActive = request.Customer.IsActive;
        customer.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return CreateCustomerHandler.MapToDto(customer);
    }
}

public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteCustomerHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCustomerCmd request, CancellationToken ct)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (customer == null)
            return false;

        // Check if customer has related records
        var hasInvoices = await _context.Invoices
            .AnyAsync(x => x.CustomerId == request.Id && !x.IsDeleted, ct);

        if (hasInvoices)
            throw new InvalidOperationException("Cannot delete customer with existing invoices. Please archive instead.");

        customer.IsDeleted = true;
        customer.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class ToggleCustomerStatusHandler : IRequestHandler<ToggleCustomerStatusCmd, CustomerDto>
{
    private readonly FinanceDbContext _context;

    public ToggleCustomerStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto> Handle(ToggleCustomerStatusCmd request, CancellationToken ct)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (customer == null)
            throw new InvalidOperationException($"Customer with ID '{request.Id}' not found");

        customer.IsActive = !customer.IsActive;
        customer.Status = customer.IsActive ? "Active" : "Inactive";
        customer.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return CreateCustomerHandler.MapToDto(customer);
    }
}

public class BulkCreateCustomersHandler : IRequestHandler<BulkCreateCustomersCmd, BulkOperationResultDto>
{
    private readonly FinanceDbContext _context;

    public BulkCreateCustomersHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BulkOperationResultDto> Handle(BulkCreateCustomersCmd request, CancellationToken ct)
    {
        var result = new BulkOperationResultDto
        {
            TotalProcessed = request.Customers.Count
        };

        var customersToAdd = new List<Customer>();
        var errors = new List<BulkOperationErrorDto>();

        for (int i = 0; i < request.Customers.Count; i++)
        {
            var dto = request.Customers[i];
            try
            {
                // Generate code if not provided
                if (string.IsNullOrEmpty(dto.Code))
                {
                    dto.Code = await GenerateCustomerCode(ct);
                }

                // Check for duplicate code
                var exists = await _context.Customers
                    .AnyAsync(x => x.Code == dto.Code && !x.IsDeleted, ct);

                if (exists)
                {
                    errors.Add(new BulkOperationErrorDto
                    {
                        RowIndex = i,
                        Code = dto.Code,
                        ErrorMessage = $"Customer with code '{dto.Code}' already exists"
                    });
                    continue;
                }

                if (string.IsNullOrEmpty(dto.Name))
                {
                    errors.Add(new BulkOperationErrorDto
                    {
                        RowIndex = i,
                        Code = dto.Code ?? "N/A",
                        ErrorMessage = "Customer name is required"
                    });
                    continue;
                }

                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Code = dto.Code,
                    Name = dto.Name,
                    NameAm = dto.NameAm,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Mobile = dto.Mobile,
                    Address = dto.Address,
                    City = dto.City,
                    Country = dto.Country,
                    TaxId = dto.TaxId,
                    CustomerType = dto.CustomerType ?? "Regular",
                    Status = dto.Status ?? "Active",
                    PaymentTerms = dto.PaymentTerms ?? "Net 30",
                    Currency = dto.Currency ?? "USD",
                    CreditLimit = dto.CreditLimit,
                    SalesRep = dto.SalesRep,
                    ContactPerson = dto.ContactPerson != null
                        ? JsonSerializer.Serialize(dto.ContactPerson)
                        : null,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    DateAdd = DateTime.UtcNow
                };

                customersToAdd.Add(customer);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkOperationErrorDto
                {
                    RowIndex = i,
                    Code = dto.Code ?? "N/A",
                    ErrorMessage = ex.Message
                });
            }
        }

        if (customersToAdd.Any())
        {
            await _context.Customers.AddRangeAsync(customersToAdd, ct);
            await _context.SaveChangesAsync(ct);
        }

        result.FailedCount = errors.Count;
        result.Errors = errors;

        return result;
    }

    private async Task<string> GenerateCustomerCode(CancellationToken ct)
    {
        var lastCustomer = await _context.Customers
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(ct);

        if (lastCustomer == null)
            return "CUST-001";

        var lastNumber = int.Parse(lastCustomer.Code.Split('-')[1]);
        return $"CUST-{(lastNumber + 1):D3}";
    }
}