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

// ==================== ADD DEPARTMENT ====================
public class AddDeptCmd : IRequest<DeptDto>
{
    public AddDeptDto AddDto { get; set; } = default!;
}

public class AddDeptHandler : IRequestHandler<AddDeptCmd, DeptDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AddDeptHandler> _logger;

    public AddDeptHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<AddDeptHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<DeptDto> Handle(AddDeptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var department = new Department
            {
                Id = Guid.CreateVersion7(),
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                DeptStat = "Active", // Default status
                BranchId = request.AddDto.BranchId,
                DateAdd = DateTime.UtcNow,
                DateMod = null
            };

            await _uow.Add(department, ct);
            await _uow.Commit(ct);

            // Publish event for real-time sync
            await _eventPublisher.PublishAsync("Department", "CREATED", new DepartmentEventData
            {
                Id = department.Id,
                Name = department.Name,
                NameAm = department.NameAm,
                BranchId = department.BranchId,
                DeptStat = department.DeptStat
            }, ct);

            _logger.LogInformation("Department created and event published: {DepartmentId}", department.Id);

            return new DeptDto
            {
                Id = department.Id,
                Name = department.Name,
                NameAm = department.NameAm,
                BranchId = department.BranchId,
                DeptStat = department.DeptStat,
                DateAdd = department.DateAdd,
                DateMod = department.DateMod
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== MODIFY DEPARTMENT ====================
public class ModDeptCmd : IRequest<DeptDto>
{
    public EditDeptDto ModDto { get; set; } = default!;
}

public class ModDeptHandler : IRequestHandler<ModDeptCmd, DeptDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ModDeptHandler> _logger;

    public ModDeptHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<ModDeptHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<DeptDto> Handle(ModDeptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var department = await _uow.Set<Department>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);

            if (department == null)
                throw new DomainException($"Department with ID '{request.ModDto.Id}' not found");

            // Update properties
            department.Name = request.ModDto.Name;
            department.NameAm = request.ModDto.NameAm;
            department.BranchId = request.ModDto.BranchId;
            department.DeptStat = request.ModDto.DeptStat ?? department.DeptStat;
            department.DateMod = DateTime.UtcNow;

            await _uow.Update(department);
            await _uow.Commit(ct);

            // Publish update event
            await _eventPublisher.PublishAsync("Department", "UPDATED", new DepartmentEventData
            {
                Id = department.Id,
                Name = department.Name,
                NameAm = department.NameAm,
                BranchId = department.BranchId,
                DeptStat = department.DeptStat
            }, ct);

            _logger.LogInformation("Department updated and event published: {DepartmentId}", department.Id);

            return new DeptDto
            {
                Id = department.Id,
                Name = department.Name,
                NameAm = department.NameAm,
                BranchId = department.BranchId,
                DeptStat = department.DeptStat,
                DateAdd = department.DateAdd,
                DateMod = department.DateMod
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ==================== DELETE DEPARTMENT ====================
public class DelDeptCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DelDeptHandler : IRequestHandler<DelDeptCmd, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<DelDeptHandler> _logger;

    public DelDeptHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<DelDeptHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<bool> Handle(DelDeptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var department = await _uow.Set<Department>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (department == null)
                throw new DomainException($"Department with ID '{request.Id}' not found");

            // ? Check if department has positions (since Department has no Position collection)


            // Soft delete - update status instead of hard delete
            department.DeptStat = "Deleted";
            department.DateMod = DateTime.UtcNow;

            await _uow.Update(department);
            await _uow.Commit(ct);

            // Publish delete event
            await _eventPublisher.PublishAsync("Department", "DELETED", new DepartmentEventData
            {
                Id = department.Id,
                Name = department.Name,
                NameAm = department.NameAm,
                BranchId = department.BranchId,
                DeptStat = department.DeptStat
            }, ct);

            _logger.LogInformation("Department deleted and event published: {DepartmentId}", department.Id);

            return true;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}