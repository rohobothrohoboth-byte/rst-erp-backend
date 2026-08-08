// Cor.CRM/Commands/CompanyCommands.cs

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

public class CompanyAddCmd : IRequest<CompanyDto>
{
    public CreateCompanyDto Dto { get; set; } = default!;
}

public class CompanyModCmd : IRequest<CompanyDto>
{
    public Guid Id { get; set; }
    public UpdateCompanyDto Dto { get; set; } = default!;
}

public class CompanyDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class CompanyAddHandler : IRequestHandler<CompanyAddCmd, CompanyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CompanyAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CompanyDto> Handle(CompanyAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var company = new Company
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                LegalName = request.Dto.LegalName,
                Email = request.Dto.Email,
                Phone = request.Dto.Phone,
                Website = request.Dto.Website,
                Industry = request.Dto.Industry,
                Size = request.Dto.Size,
                Status = request.Dto.Status ?? "Active",
                Address = request.Dto.Address,
                City = request.Dto.City,
                State = request.Dto.State,
                Country = request.Dto.Country,
                PostalCode = request.Dto.PostalCode,
                Description = request.Dto.Description,
                FoundedYear = request.Dto.FoundedYear,
                Revenue = request.Dto.Revenue,
                EmployeeCount = request.Dto.EmployeeCount,
                TaxId = request.Dto.TaxId,
                RegistrationNumber = request.Dto.RegistrationNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(company, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Company created: {CompanyId} - {CompanyName}", company.Id, company.Name);

            return MapToDto(company);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            LegalName = company.LegalName,
            Email = company.Email,
            Phone = company.Phone,
            Website = company.Website,
            Industry = company.Industry,
            Size = company.Size,
            Status = company.Status,
            Address = company.Address,
            City = company.City,
            State = company.State,
            Country = company.Country,
            PostalCode = company.PostalCode,
            Description = company.Description,
            FoundedYear = company.FoundedYear,
            Revenue = company.Revenue,
            EmployeeCount = company.EmployeeCount,
            TaxId = company.TaxId,
            RegistrationNumber = company.RegistrationNumber,
            ContactCount = company.ContactCount,
            LeadCount = company.LeadCount,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt
        };
    }
}

public class CompanyModHandler : IRequestHandler<CompanyModCmd, CompanyDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CompanyModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<CompanyDto> Handle(CompanyModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var company = await _uow.Set<Company>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (company == null)
                throw new DomainException($"Company with id [{request.Id}] NOT FOUND.");

            if (request.Dto.Name != null)
                company.Name = request.Dto.Name;
            if (request.Dto.LegalName != null)
                company.LegalName = request.Dto.LegalName;
            if (request.Dto.Email != null)
                company.Email = request.Dto.Email;
            if (request.Dto.Phone != null)
                company.Phone = request.Dto.Phone;
            if (request.Dto.Website != null)
                company.Website = request.Dto.Website;
            if (request.Dto.Industry != null)
                company.Industry = request.Dto.Industry;
            if (request.Dto.Size != null)
                company.Size = request.Dto.Size;
            if (request.Dto.Status != null)
                company.Status = request.Dto.Status;
            if (request.Dto.Address != null)
                company.Address = request.Dto.Address;
            if (request.Dto.City != null)
                company.City = request.Dto.City;
            if (request.Dto.State != null)
                company.State = request.Dto.State;
            if (request.Dto.Country != null)
                company.Country = request.Dto.Country;
            if (request.Dto.PostalCode != null)
                company.PostalCode = request.Dto.PostalCode;
            if (request.Dto.Description != null)
                company.Description = request.Dto.Description;
            if (request.Dto.FoundedYear.HasValue)
                company.FoundedYear = request.Dto.FoundedYear;
            if (request.Dto.Revenue.HasValue)
                company.Revenue = request.Dto.Revenue;
            if (request.Dto.EmployeeCount.HasValue)
                company.EmployeeCount = request.Dto.EmployeeCount;
            if (request.Dto.TaxId != null)
                company.TaxId = request.Dto.TaxId;
            if (request.Dto.RegistrationNumber != null)
                company.RegistrationNumber = request.Dto.RegistrationNumber;
            if (request.Dto.IsActive.HasValue)
                company.IsActive = request.Dto.IsActive.Value;

            company.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(company);
            await _uow.Commit(ct);

            _logger.LogInformation("Company updated: {CompanyId} - {CompanyName}", company.Id, company.Name);

            return MapToDto(company);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            LegalName = company.LegalName,
            Email = company.Email,
            Phone = company.Phone,
            Website = company.Website,
            Industry = company.Industry,
            Size = company.Size,
            Status = company.Status,
            Address = company.Address,
            City = company.City,
            State = company.State,
            Country = company.Country,
            PostalCode = company.PostalCode,
            Description = company.Description,
            FoundedYear = company.FoundedYear,
            Revenue = company.Revenue,
            EmployeeCount = company.EmployeeCount,
            TaxId = company.TaxId,
            RegistrationNumber = company.RegistrationNumber,
            ContactCount = company.ContactCount,
            LeadCount = company.LeadCount,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt
        };
    }
}

public class CompanyDelHandler : IRequestHandler<CompanyDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public CompanyDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(CompanyDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var company = await _uow.Set<Company>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (company == null)
                throw new DomainException($"Company with id [{request.Id}] NOT FOUND.");

            company.IsDeleted = true;
            company.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(company);
            await _uow.Commit(ct);

            _logger.LogInformation("Company deleted: {CompanyId} - {CompanyName}", company.Id, company.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}