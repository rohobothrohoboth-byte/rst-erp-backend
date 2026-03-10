using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmployeeModCmd : IRequest<EmployeeListDto> { public EmployeeModDto ModDto { get; set; } = default!; }
public class EmployeeDelCmd : IRequest { public Guid Id { get; set; } }

public class EmployeeModCmdHandler : IRequestHandler<EmployeeModCmd, EmployeeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EmployeeModCmdHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
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
            await _uow.Update(oldData);
            await _uow.Commit(ct);

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
    public EmployeeDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(EmployeeDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"EMPLOYEE with id [{request.Id}] NOT FOUND."); }
            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}