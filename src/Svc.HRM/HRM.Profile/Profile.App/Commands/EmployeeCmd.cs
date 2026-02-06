using Helpers;
using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmployeeModCmd : IRequest<EmployeeListDto> { public EmployeeModDto ModDto { get; set; } = default!; }
public class EmployeeDelCmd : IRequest { public Guid Id { get; set; } }

public class EmployeeModCmdHandler : IRequestHandler<EmployeeModCmd, EmployeeListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmployeeModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmployeeListDto> Handle(EmployeeModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<Employee>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EMPLOYEE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();

        try
        {
            var oldPer = await _unitOfWork.Repository<Person>().GetById(oldData.PersonId);
            oldPer!.FirstName = request.ModDto.FirstName;
            oldPer.FirstNameAm = request.ModDto.FirstNameAm;
            oldPer.MiddleName = request.ModDto.MiddleName;
            oldPer.MiddleNameAm = request.ModDto.MiddleNameAm;
            oldPer.LastName = request.ModDto.LastName;
            oldPer.LastNameAm = request.ModDto.LastNameAm;
            oldPer.Gender = request.ModDto.Gender;
            oldPer.Nationality = request.ModDto.Nationality;
            var per = await _unitOfWork.Repository<Person>().Update(oldPer);

            oldData.EmploymentDate = request.ModDto.EmploymentDate;
            oldData.JobGradeId = request.ModDto.JobGradeId;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.DepartmentId = request.ModDto.DepartmentId;
            oldData.EmploymentType = request.ModDto.EmploymentType;
            oldData.EmploymentNature = request.ModDto.EmploymentNature;
            oldData.PersonId = per.Id;
            var data = await _unitOfWork.Repository<Employee>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmployeeListDto();
            var response = await _med.Send(new EmployeeByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class EmployeeDelCmdHandler : IRequestHandler<EmployeeDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmployeeDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmployeeDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EMPLOYEE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Employee>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}