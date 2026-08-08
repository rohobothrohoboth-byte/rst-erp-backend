// Cor.CRM/Commands/CustomerCmd.cs

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

public class CustomerAddCmd : IRequest<CustomerDto>
{
    public CreateCustomerDto Dto { get; set; } = default!;
}

public class CustomerModCmd : IRequest<CustomerDto>
{
    public Guid Id { get; set; }
    public UpdateCustomerDto Dto { get; set; } = default!;
}

public class CustomerDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// ADD CUSTOMER
// ============================================================

public class CustomerAddHandler : IRequestHandler<CustomerAddCmd, CustomerDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CustomerAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(CustomerAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var customer = new Customer
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                CompanyName = request.Dto.CompanyName,
                Email = request.Dto.Email,
                Phone = request.Dto.Phone,
                Mobile = request.Dto.Mobile,
                Address = request.Dto.Address,
                City = request.Dto.City,
                State = request.Dto.State,
                PostalCode = request.Dto.PostalCode,
                Country = request.Dto.Country,
                Type = string.IsNullOrEmpty(request.Dto.Type) ? CustomerType.Individual : Enum.Parse<CustomerType>(request.Dto.Type),
                Industry = string.IsNullOrEmpty(request.Dto.Industry) ? null : Enum.Parse<Industry>(request.Dto.Industry),
                Description = request.Dto.Description,
                AnnualRevenue = request.Dto.AnnualRevenue,
                EmployeeCount = request.Dto.EmployeeCount ?? 0,
                Website = request.Dto.Website,
                Tags = request.Dto.Tags,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(customer, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Customer created: {CustomerId} - {CustomerName}", customer.Id, customer.Name);

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                Email = customer.Email,
                Phone = customer.Phone,
                Mobile = customer.Mobile,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                Type = customer.Type.ToString(),
                Industry = customer.Industry?.ToString(),
                Description = customer.Description,
                AnnualRevenue = customer.AnnualRevenue,
                EmployeeCount = customer.EmployeeCount,
                Website = customer.Website,
                Tags = customer.Tags,
                CreatedAt = customer.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// UPDATE CUSTOMER
// ============================================================

public class CustomerModHandler : IRequestHandler<CustomerModCmd, CustomerDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CustomerModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(CustomerModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var customer = await _uow.Set<Customer>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (customer == null)
            {
                throw new DomainException($"Customer with id [{request.Id}] NOT FOUND.");
            }

            if (request.Dto.Name != null)
                customer.Name = request.Dto.Name;
            if (request.Dto.CompanyName != null)
                customer.CompanyName = request.Dto.CompanyName;
            if (request.Dto.Email != null)
                customer.Email = request.Dto.Email;
            if (request.Dto.Phone != null)
                customer.Phone = request.Dto.Phone;
            if (request.Dto.Mobile != null)
                customer.Mobile = request.Dto.Mobile;
            if (request.Dto.Address != null)
                customer.Address = request.Dto.Address;
            if (request.Dto.City != null)
                customer.City = request.Dto.City;
            if (request.Dto.State != null)
                customer.State = request.Dto.State;
            if (request.Dto.PostalCode != null)
                customer.PostalCode = request.Dto.PostalCode;
            if (request.Dto.Country != null)
                customer.Country = request.Dto.Country;
            if (request.Dto.Status != null)
                customer.Status = Enum.Parse<CustomerStatus>(request.Dto.Status);
            if (request.Dto.Type != null)
                customer.Type = Enum.Parse<CustomerType>(request.Dto.Type);
            if (request.Dto.Industry != null)
                customer.Industry = Enum.Parse<Industry>(request.Dto.Industry);
            if (request.Dto.Description != null)
                customer.Description = request.Dto.Description;
            if (request.Dto.AnnualRevenue.HasValue)
                customer.AnnualRevenue = request.Dto.AnnualRevenue;
            if (request.Dto.EmployeeCount.HasValue)
                customer.EmployeeCount = request.Dto.EmployeeCount.Value;
            if (request.Dto.Website != null)
                customer.Website = request.Dto.Website;
            if (request.Dto.Tags != null)
                customer.Tags = request.Dto.Tags;
            if (request.Dto.IsActive.HasValue)
                customer.IsActive = request.Dto.IsActive.Value;

            customer.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(customer);
            await _uow.Commit(ct);

            _logger.LogInformation("Customer updated: {CustomerId} - {CustomerName}", customer.Id, customer.Name);

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                Email = customer.Email,
                Phone = customer.Phone,
                Mobile = customer.Mobile,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                Status = customer.Status.ToString(),
                Type = customer.Type.ToString(),
                Industry = customer.Industry?.ToString(),
                Description = customer.Description,
                AnnualRevenue = customer.AnnualRevenue,
                EmployeeCount = customer.EmployeeCount,
                Website = customer.Website,
                Tags = customer.Tags,
                IsActive = customer.IsActive,
                UpdatedAt = customer.UpdatedAt,
                CreatedAt = customer.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// DELETE CUSTOMER
// ============================================================

public class CustomerDelHandler : IRequestHandler<CustomerDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CustomerDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(CustomerDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var customer = await _uow.Set<Customer>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (customer == null)
            {
                throw new DomainException($"Customer with id [{request.Id}] NOT FOUND.");
            }

            await _uow.Delete(customer);
            await _uow.Commit(ct);

            _logger.LogInformation("Customer deleted: {CustomerId} - {CustomerName}", customer.Id, customer.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}