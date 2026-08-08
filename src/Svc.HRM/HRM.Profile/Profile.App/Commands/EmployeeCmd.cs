using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.App.Services;  // ? ADD THIS for IEventPublisher
using Shared.Helpers.Events;  // ? ADD THIS
using Microsoft.Extensions.Logging;  // ? ADD THIS

namespace Profile.App.Commands;

public class EmployeeModCmd : IRequest<EmployeeListDto> { public EmployeeModDto ModDto { get; set; } = default!; }
public class EmployeeDelCmd : IRequest { public Guid Id { get; set; } }

public class EmployeeModCmdHandler : IRequestHandler<EmployeeModCmd, EmployeeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<EmployeeModCmdHandler> _logger;

    public EmployeeModCmdHandler(
        IUnitOfWork uow,
        IMediator med,
        IEventPublisher eventPublisher,
        ILogger<EmployeeModCmdHandler> logger)
    {
        _uow = uow;
        _med = med;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<EmployeeListDto> Handle(EmployeeModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"EMPLOYEE with Id {request.ModDto.Id} NOT FOUND."); }

            var oldPer = await _uow.Set<Person>().FirstOrDefaultAsync(x => x.Id == oldData.PersonId, cancellationToken: ct);
            oldPer!.FirstName = request.ModDto.FirstName;
            oldPer.FirstNameAm = request.ModDto.FirstNameAm;
            oldPer.MiddleName = request.ModDto.MiddleName;
            oldPer.MiddleNameAm = request.ModDto.MiddleNameAm;
            oldPer.LastName = request.ModDto.LastName;
            oldPer.LastNameAm = request.ModDto.LastNameAm;
            oldPer.Gender = request.ModDto.Gender;
            oldPer.Nationality = request.ModDto.Nationality;
            await _uow.Update(oldPer);

            oldData.EmploymentDate = request.ModDto.EmploymentDate;
            oldData.JobGradeId = request.ModDto.JobGradeId;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.DepartmentId = request.ModDto.DepartmentId;
            oldData.EmploymentType = request.ModDto.EmploymentType;
            oldData.EmploymentNature = request.ModDto.EmploymentNature;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            oldData.DateMod = DateTime.UtcNow;
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            // ? Publish update event
            var eventData = new EmployeeEventData
            {
                Id = oldData.Id,
                Code = oldData.Code,
                EmpState = oldData.EmpState,
                EmploymentType = oldData.EmploymentType,
                EmploymentNature = oldData.EmploymentNature,
                WorkArrangement = oldData.WorkArrangement,
                EmploymentDate = oldData.EmploymentDate,
                PersonId = oldData.PersonId,
                JobGradeId = oldData.JobGradeId,
                PositionId = oldData.PositionId,
                DepartmentId = oldData.DepartmentId,
                AppUserId = oldData.AppUserId,
                FirstName = oldPer.FirstName,
                FirstNameAm = oldPer.FirstNameAm,
                MiddleName = oldPer.MiddleName,
                MiddleNameAm = oldPer.MiddleNameAm,
                LastName = oldPer.LastName,
                LastNameAm = oldPer.LastNameAm,
                Gender = oldPer.Gender,
                Nationality = oldPer.Nationality,

                IsDeleted = oldData.IsDeleted,
                IsActive = false,
                BranchId = null
            };

            await _eventPublisher.PublishAsync("Employee", "UPDATED", eventData, ct);
            _logger.LogInformation("Employee updated and event published: {EmployeeId}", oldData.Id);

            var res = new EmployeeListDto();
            var response = await _med.Send(new EmployeeByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmployeeDelCmdHandler : IRequestHandler<EmployeeDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<EmployeeDelCmdHandler> _logger;

    public EmployeeDelCmdHandler(
        IUnitOfWork uow,
        IEventPublisher eventPublisher,
        ILogger<EmployeeDelCmdHandler> logger)
    {
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(EmployeeDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"EMPLOYEE with id [{request.Id}] NOT FOUND."); }

            data.IsDeleted = true;
            data.DateMod = DateTime.UtcNow;
            await _uow.Update(data);
            await _uow.Commit(ct);

            // ? Publish delete event
            await _eventPublisher.PublishAsync("Employee", "DELETED", new EmployeeEventData
            {
                Id = data.Id,
                Code = data.Code,
                IsDeleted = true
            }, ct);

            _logger.LogInformation("Employee deleted and event published: {EmployeeId}", data.Id);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}