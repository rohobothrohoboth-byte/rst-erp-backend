// Queries/CustomerQueries.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using System.Text.Json;

namespace Cor.Finance.Queries;

// ============================================================
// QUERY DEFINITIONS
// ============================================================

public class CustomerAllQry : IRequest<List<CustomerDto>>
{
    public string? Status { get; set; }
    public string? CustomerType { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
}

public class CustomerByIdQry : IRequest<CustomerDto?>
{
    public Guid Id { get; set; }
}

public class CustomerByCodeQry : IRequest<CustomerDto?>
{
    public string Code { get; set; } = string.Empty;
}

public class CustomerByTypeQry : IRequest<List<CustomerDto>>
{
    public string CustomerType { get; set; } = string.Empty;
}

public class CustomerActiveQry : IRequest<List<CustomerDto>>
{
    public bool? IsActive { get; set; } = true;
}

public class CustomerSearchQry : IRequest<List<CustomerDto>>
{
    public string? SearchTerm { get; set; }
    public string? CustomerType { get; set; }
    public bool? IsActive { get; set; }
}

public class CustomerSummaryQry : IRequest<CustomerSummaryDto>
{
    public Guid? CustomerId { get; set; }
}

public class CustomerSummaryDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalInvoices { get; set; }
    public decimal AverageCreditLimit { get; set; }
    public Dictionary<string, int> CustomersByType { get; set; } = new();
    public Dictionary<string, int> CustomersByStatus { get; set; } = new();
}

// ============================================================
// QUERY HANDLERS
// ============================================================

public class CustomerAllHandler : IRequestHandler<CustomerAllQry, List<CustomerDto>>
{
    private readonly FinanceDbContext _context;

    public CustomerAllHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(CustomerAllQry request, CancellationToken ct)
    {
        var query = _context.Customers
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.CustomerType))
            query = query.Where(x => x.CustomerType == request.CustomerType);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                (x.NameAm != null && x.NameAm.ToLower().Contains(search)) ||
                (x.Code != null && x.Code.ToLower().Contains(search)) ||
                (x.Email != null && x.Email.ToLower().Contains(search)));
        }

        var customers = await query
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return customers.Select(MapToDto).ToList();
    }

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
            DateAdd = customer.DateAdd,
            DateMod = customer.DateMod
        };
    }
}

public class CustomerByIdHandler : IRequestHandler<CustomerByIdQry, CustomerDto?>
{
    private readonly FinanceDbContext _context;

    public CustomerByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto?> Handle(CustomerByIdQry request, CancellationToken ct)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (customer == null)
            return null;

        return CustomerAllHandler.MapToDto(customer);
    }
}

public class CustomerByCodeHandler : IRequestHandler<CustomerByCodeQry, CustomerDto?>
{
    private readonly FinanceDbContext _context;

    public CustomerByCodeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto?> Handle(CustomerByCodeQry request, CancellationToken ct)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.Code == request.Code && !x.IsDeleted, ct);

        if (customer == null)
            return null;

        return CustomerAllHandler.MapToDto(customer);
    }
}

public class CustomerByTypeHandler : IRequestHandler<CustomerByTypeQry, List<CustomerDto>>
{
    private readonly FinanceDbContext _context;

    public CustomerByTypeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(CustomerByTypeQry request, CancellationToken ct)
    {
        var customers = await _context.Customers
            .Where(x => x.CustomerType == request.CustomerType && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return customers.Select(CustomerAllHandler.MapToDto).ToList();
    }
}

public class CustomerActiveHandler : IRequestHandler<CustomerActiveQry, List<CustomerDto>>
{
    private readonly FinanceDbContext _context;

    public CustomerActiveHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(CustomerActiveQry request, CancellationToken ct)
    {
        var customers = await _context.Customers
            .Where(x => x.IsActive == request.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return customers.Select(CustomerAllHandler.MapToDto).ToList();
    }
}

public class CustomerSearchHandler : IRequestHandler<CustomerSearchQry, List<CustomerDto>>
{
    private readonly FinanceDbContext _context;

    public CustomerSearchHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(CustomerSearchQry request, CancellationToken ct)
    {
        var query = _context.Customers
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Code.ToLower().Contains(term) ||
                x.Name.ToLower().Contains(term) ||
                (x.NameAm != null && x.NameAm.ToLower().Contains(term)) ||
                (x.Email != null && x.Email.ToLower().Contains(term)) ||
                (x.Phone != null && x.Phone.Contains(term)) ||
                (x.Mobile != null && x.Mobile.Contains(term)));
        }

        if (!string.IsNullOrEmpty(request.CustomerType))
            query = query.Where(x => x.CustomerType == request.CustomerType);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var customers = await query
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return customers.Select(CustomerAllHandler.MapToDto).ToList();
    }
}

public class CustomerSummaryHandler : IRequestHandler<CustomerSummaryQry, CustomerSummaryDto>
{
    private readonly FinanceDbContext _context;

    public CustomerSummaryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerSummaryDto> Handle(CustomerSummaryQry request, CancellationToken ct)
    {
        var query = _context.Customers
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.CustomerId.HasValue)
            query = query.Where(x => x.Id == request.CustomerId.Value);

        var customers = await query.ToListAsync(ct);

        var totalCustomers = customers.Count;
        var activeCustomers = customers.Count(x => x.IsActive);
        var inactiveCustomers = totalCustomers - activeCustomers;

        // Get financial data from related tables
        var customerIds = customers.Select(x => x.Id).ToList();
        var invoices = await _context.Invoices
           .Where(x => x.CustomerId.HasValue && customerIds.Contains(x.CustomerId.Value) && !x.IsDeleted)
            .ToListAsync(ct);

        var totalRevenue = invoices.Sum(x => x.TotalAmount);
        var totalInvoices = invoices.Count;
        var avgCreditLimit = customers.Average(x => x.CreditLimit ?? 0);

        return new CustomerSummaryDto
        {
            TotalCustomers = totalCustomers,
            ActiveCustomers = activeCustomers,
            InactiveCustomers = inactiveCustomers,
            TotalRevenue = totalRevenue,
            TotalInvoices = totalInvoices,
            AverageCreditLimit = avgCreditLimit,
            CustomersByType = customers.GroupBy(x => x.CustomerType)
                .ToDictionary(g => g.Key, g => g.Count()),
            CustomersByStatus = customers.GroupBy(x => x.Status)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}