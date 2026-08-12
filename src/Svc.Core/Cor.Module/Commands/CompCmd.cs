using Cor.Module.Services;
using Shared.Helpers.Events;
using MediatR;
using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Commands;

// ==================== ADD COMPANY ====================
public class AddCompCmd : IRequest<CompDto>
{
    public AddCompDto AddDto { get; set; } = default!;
}

public class AddCompHandler : IRequestHandler<AddCompCmd, CompDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AddCompHandler> _logger;

    public AddCompHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<AddCompHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<CompDto> Handle(AddCompCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var company = new Company
            {
                Id = Guid.CreateVersion7(),
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                TaxId = request.AddDto.TaxId,
                Phone = request.AddDto.Phone,
                Email = request.AddDto.Email,
                Address = request.AddDto.Address,
                Website = request.AddDto.Website,
                LogoUrl = request.AddDto.LogoUrl,
                Motto = request.AddDto.Motto,
                Mission = request.AddDto.Mission,
                Vision = request.AddDto.Vision,
                Values = request.AddDto.Values,
                Structure = request.AddDto.Structure,
                IsDeleted = false,
                DateAdd = DateTime.UtcNow
            };

            await _uow.Add(company, ct);
            await _uow.Commit(ct);

            // ? Publish event for real-time sync
            await _eventPublisher.PublishAsync("Company", "CREATED", new CompanyEventData
            {
                Id = company.Id,
                Name = company.Name,
                NameAm = company.NameAm,
                TaxId = company.TaxId,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                LogoUrl = company.LogoUrl,
                IsDeleted = company.IsDeleted
            }, ct);

            _logger.LogInformation("Company created and event published: {CompanyId}", company.Id);

            return new CompDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAm = company.NameAm,
                TaxId = company.TaxId,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                Website = company.Website,
                LogoUrl = company.LogoUrl,
                Motto = company.Motto,
                Mission = company.Mission,
                Vision = company.Vision,
                Values = company.Values,
                Structure = company.Structure,
                IsDeleted = company.IsDeleted,
                DateAdd = company.DateAdd,
                DateMod = company.DateMod
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== MODIFY COMPANY ====================
public class ModCompCmd : IRequest<CompDto>
{
    public EditCompDto ModDto { get; set; } = default!;
}

public class ModCompHandler : IRequestHandler<ModCompCmd, CompDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ModCompHandler> _logger;

    public ModCompHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<ModCompHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<CompDto> Handle(ModCompCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var company = await _uow.Set<Company>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);

            if (company == null)
                throw new DomainException($"Company with ID '{request.ModDto.Id}' not found");

            // Update properties
            company.Name = request.ModDto.Name;
            company.NameAm = request.ModDto.NameAm;
            company.TaxId = request.ModDto.TaxId ?? company.TaxId;
            company.Phone = request.ModDto.Phone ?? company.Phone;
            company.Email = request.ModDto.Email ?? company.Email;
            company.Address = request.ModDto.Address ?? company.Address;
            company.Website = request.ModDto.Website ?? company.Website;
            company.LogoUrl = request.ModDto.LogoUrl ?? company.LogoUrl;
            company.Motto = request.ModDto.Motto ?? company.Motto;
            company.Mission = request.ModDto.Mission ?? company.Mission;
            company.Vision = request.ModDto.Vision ?? company.Vision;
            company.Values = request.ModDto.Values ?? company.Values;
            company.Structure = request.ModDto.Structure ?? company.Structure;
            company.DateMod = DateTime.UtcNow;

            await _uow.Update(company);
            await _uow.Commit(ct);

            // ? Publish update event
            await _eventPublisher.PublishAsync("Company", "UPDATED", new CompanyEventData
            {
                Id = company.Id,
                Name = company.Name,
                NameAm = company.NameAm,
                TaxId = company.TaxId,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                LogoUrl = company.LogoUrl,
                IsDeleted = company.IsDeleted
            }, ct);

            _logger.LogInformation("Company updated and event published: {CompanyId}", company.Id);

            return new CompDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAm = company.NameAm,
                TaxId = company.TaxId,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                Website = company.Website,
                LogoUrl = company.LogoUrl,
                Motto = company.Motto,
                Mission = company.Mission,
                Vision = company.Vision,
                Values = company.Values,
                Structure = company.Structure,
                IsDeleted = company.IsDeleted,
                DateAdd = company.DateAdd,
                DateMod = company.DateMod
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== DELETE COMPANY ====================
public class DelCompCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DelCompHandler : IRequestHandler<DelCompCmd, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<DelCompHandler> _logger;
    private readonly IDapperHelper _dapper;

    public DelCompHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<DelCompHandler> logger, IDapperHelper dapper)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _dapper = dapper;
    }

    public async Task<bool> Handle(DelCompCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // ? Check if company has branches
            const string v = "v";
            var qb = new QueryBuilder()
                .SelectDto<Branch, NameList>(v)
                .From<Branch>(v)
                .Where<Branch>(v, x => x.CompId == request.Id);
            var (sql, parameters) = qb.Build();

            await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
            var branches = await reader.ToListAsync<NameList>(ct);

            if (branches.Count != 0)
                throw new DomainException($"Company with ID '{request.Id}' has branches and cannot be deleted.");

            // ? Soft delete the company
            var company = await _uow.Set<Company>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (company == null)
                throw new DomainException($"Company with ID '{request.Id}' not found");

            company.IsDeleted = true;
            company.DateMod = DateTime.UtcNow;

            await _uow.Update(company);
            await _uow.Commit(ct);

            // ? Publish delete event
            await _eventPublisher.PublishAsync("Company", "DELETED", new CompanyEventData
            {
                Id = company.Id,
                Name = company.Name,
                NameAm = company.NameAm,
                TaxId = company.TaxId,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                LogoUrl = company.LogoUrl,
                IsDeleted = true
            }, ct);

            _logger.LogInformation("Company deleted and event published: {CompanyId}", company.Id);

            return true;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}