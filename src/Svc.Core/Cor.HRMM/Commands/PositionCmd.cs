using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Cor.HRMM.Services;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Events;
using System.Data;

namespace Cor.HRMM.Commands;

public class PositionAddCmd : IRequest<PositionListDto> { public PositionAddDto AddDto { get; set; } = default!; }
public class PositionModCmd : IRequest<PositionListDto> { public PositionModDto ModDto { get; set; } = default!; }
public class PositionDelCmd : IRequest { public Guid Id { get; set; } }

// ==================== ADD POSITION ====================
public class PositionAddHandler : IRequestHandler<PositionAddCmd, PositionListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<PositionAddHandler> _logger;

    public PositionAddHandler(
        IUnitOfWork unitOfWork,
        IMediator med,
        IEventPublisher eventPublisher,
        ILogger<PositionAddHandler> logger)
    {
        _uow = unitOfWork;
        _med = med;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<PositionListDto> Handle(PositionAddCmd request, CancellationToken ct)
    {
        Position data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new Position
            {
                Id = Guid.CreateVersion7(),
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                NoOfPosition = request.AddDto.NoOfPosition,
                IsVacant = request.AddDto.IsVacant,
                DepartmentId = request.AddDto.DepartmentId,
                IsDeleted = false,
                DateAdd = DateTime.UtcNow
            };

            await _uow.AddAsync(data, token);
        }, ct: ct);

        // Publish event for real-time sync
        await _eventPublisher.PublishAsync("Position", "CREATED", new PositionEventData
        {
            Id = data.Id,
            Name = data.Name,
            NameAm = data.NameAm,
            NoOfPosition = data.NoOfPosition,
            IsVacant = data.IsVacant,
            DepartmentId = data.DepartmentId,
            IsDeleted = false
        }, ct);

        _logger.LogInformation("Position created and event published: {PositionId}", data.Id);

        // Get the full DTO
        var response = await _med.Send(new PositionByIdQry { Id = data.Id }, ct);
        return response ?? new PositionListDto();
    }
}

// ==================== MODIFY POSITION ====================
public class PositionModHandler : IRequestHandler<PositionModCmd, PositionListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<PositionModHandler> _logger;

    public PositionModHandler(
        IUnitOfWork unitOfWork,
        IMediator med,
        IEventPublisher eventPublisher,
        ILogger<PositionModHandler> logger)
    {
        _uow = unitOfWork;
        _med = med;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<PositionListDto> Handle(PositionModCmd request, CancellationToken ct)
    {
        Position? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<Position>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"POSITION with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.NoOfPosition = request.ModDto.NoOfPosition;
            oldData.IsVacant = request.ModDto.IsVacant;
            oldData.DepartmentId = request.ModDto.DepartmentId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            oldData.DateMod = DateTime.UtcNow;

            _uow.Update(oldData);
        }, ct: ct);

        // Publish update event
        await _eventPublisher.PublishAsync("Position", "UPDATED", new PositionEventData
        {
            Id = oldData!.Id,
            Name = oldData.Name,
            NameAm = oldData.NameAm,
            NoOfPosition = oldData.NoOfPosition,
            IsVacant = oldData.IsVacant,
            DepartmentId = oldData.DepartmentId,
            IsDeleted = oldData.IsDeleted
        }, ct);

        _logger.LogInformation("Position updated and event published: {PositionId}", oldData.Id);

        // Get the full DTO
        var response = await _med.Send(new PositionByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new PositionListDto();
    }
}

// ==================== DELETE POSITION ====================
public class PositionDelHandler : IRequestHandler<PositionDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<PositionDelHandler> _logger;

    public PositionDelHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        ILogger<PositionDelHandler> logger)
    {
        _uow = unitOfWork;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(PositionDelCmd request, CancellationToken ct)
    {
        Position? data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = await _uow.Set<Position>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"POSITION with id [{request.Id}] NOT FOUND.");
            }

            data.IsDeleted = true;
            data.DateMod = DateTime.UtcNow;
            _uow.Update(data);
        }, ct: ct);

        // Publish delete event with actual data
        await _eventPublisher.PublishAsync("Position", "DELETED", new PositionEventData
        {
            Id = data!.Id,
            Name = data.Name,
            NameAm = data.NameAm,
            NoOfPosition = data.NoOfPosition,
            IsVacant = data.IsVacant,
            DepartmentId = data.DepartmentId ,
            IsDeleted = true
        }, ct);

        _logger.LogInformation("Position deleted and event published: {PositionId}", data.Id);
    }
}