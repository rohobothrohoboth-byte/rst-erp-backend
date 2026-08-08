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

// ==================== ADD BRANCH ====================
public class AddBranchCmd : IRequest<BranchDto>
{
    public AddBranchDto AddDto { get; set; } = default!;
}

public class AddBranchHandler : IRequestHandler<AddBranchCmd, BranchDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AddBranchHandler> _logger;

    public AddBranchHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<AddBranchHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<BranchDto> Handle(AddBranchCmd request, CancellationToken ct)
    {
        var branch = new Branch
        {
            Id = Guid.CreateVersion7(),
            Name = request.AddDto.Name,
            NameAm = request.AddDto.NameAm,
            Location = request.AddDto.Location,
            OpenDate = request.AddDto.OpenDate,
            BranchType = request.AddDto.BranchType,
            BranchStat = request.AddDto.BranchStat,
            CompId = request.AddDto.CompId,
            DateAdd = DateTime.UtcNow
        };

        // If Code is provided, set it; otherwise let DB auto-generate
        if (!string.IsNullOrEmpty(request.AddDto.Code))
        {
            branch.Code = request.AddDto.Code;
        }

        await _uow.Add(branch, ct);
        await _uow.Commit(ct);

        // Publish event for real-time sync
        await _eventPublisher.PublishAsync("Branch", "CREATED", new BranchEventData
        {
            Id = branch.Id,
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location,
            OpenDate = branch.OpenDate,
            BranchType = branch.BranchType,
            BranchStat = branch.BranchStat,
            CompId = branch.CompId,
            IsActive = true,
            IsDeleted = false
        }, ct);

        _logger.LogInformation("Branch created and event published: {BranchId}", branch.Id);

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location,
            OpenDate = branch.OpenDate,
            BranchType = branch.BranchType,
            BranchStat = branch.BranchStat,
            CompId = branch.CompId,
            IsActive = true,
            IsDeleted = false,
            DateAdd = branch.DateAdd,
            DateMod = branch.DateMod
        };
    }
}

// ==================== MODIFY BRANCH ====================
public class ModBranchCmd : IRequest<BranchDto>
{
    public EditBranchDto ModDto { get; set; } = default!;
}

public class ModBranchHandler : IRequestHandler<ModBranchCmd, BranchDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ModBranchHandler> _logger;

    public ModBranchHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<ModBranchHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<BranchDto> Handle(ModBranchCmd request, CancellationToken ct)
    {
        var branch = await _uow.Set<Branch>().FindAsync(new object[] { request.ModDto.Id }, ct);
        if (branch == null)
            throw new DomainException($"Branch with ID '{request.ModDto.Id}' not found");

        // Update properties
        branch.Name = request.ModDto.Name;
        branch.NameAm = request.ModDto.NameAm;
        branch.Location = request.ModDto.Location;
        branch.BranchType = request.ModDto.BranchType;
        branch.BranchStat = request.ModDto.BranchStat;
        branch.DateMod = DateTime.UtcNow;

        // Update Code if provided
        if (!string.IsNullOrEmpty(request.ModDto.Code))
        {
            branch.Code = request.ModDto.Code;
        }

        await _uow.Update(branch);
        await _uow.Commit(ct);

        // Publish update event
        await _eventPublisher.PublishAsync("Branch", "UPDATED", new BranchEventData
        {
            Id = branch.Id,
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location,
            OpenDate = branch.OpenDate,
            BranchType = branch.BranchType,
            BranchStat = branch.BranchStat,
            CompId = branch.CompId,
            IsActive = true,
            IsDeleted = false
        }, ct);

        _logger.LogInformation("Branch updated and event published: {BranchId}", branch.Id);

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location,
            OpenDate = branch.OpenDate,
            BranchType = branch.BranchType,
            BranchStat = branch.BranchStat,
            CompId = branch.CompId,
            IsActive = true,
            IsDeleted = false,
            DateAdd = branch.DateAdd,
            DateMod = branch.DateMod
        };
    }
}

// ==================== DELETE BRANCH ====================
public class DelBranchCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DelBranchHandler : IRequestHandler<DelBranchCmd, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<DelBranchHandler> _logger;

    public DelBranchHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<DelBranchHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<bool> Handle(DelBranchCmd request, CancellationToken ct)
    {
        var branch = await _uow.Set<Branch>().FindAsync(new object[] { request.Id }, ct);
        if (branch == null)
            throw new DomainException($"Branch with ID '{request.Id}' not found");

        // Soft delete
        branch.IsDeleted = true;
        branch.DateMod = DateTime.UtcNow;

        await _uow.Update(branch);
        await _uow.Commit(ct);

        // Publish delete event
        await _eventPublisher.PublishAsync("Branch", "DELETED", new BranchEventData
        {
            Id = branch.Id,
            Name = branch.Name,
            NameAm = branch.NameAm,
            Code = branch.Code,
            Location = branch.Location,
            OpenDate = branch.OpenDate,
            BranchType = branch.BranchType,
            BranchStat = branch.BranchStat,
            CompId = branch.CompId,
            IsActive = false,
            IsDeleted = true
        }, ct);

        _logger.LogInformation("Branch deleted and event published: {BranchId}", branch.Id);

        return true;
    }
}