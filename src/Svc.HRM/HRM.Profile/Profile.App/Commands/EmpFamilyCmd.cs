using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpFamilyAddCmd : IRequest<EmpFamilyListDto> { public EmpFamilyAddDto AddDto { get; set; } = default!; }
public class EmpFamilyModCmd : IRequest<EmpFamilyListDto> { public EmpFamilyModDto ModDto { get; set; } = default!; }
public class EmpFamilyDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpFamilyAddCmdHandler : IRequestHandler<EmpFamilyAddCmd, EmpFamilyListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpFamilyAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpFamilyListDto> Handle(EmpFamilyAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var per = new Person
            {
                FirstName = request.AddDto.FirstName,
                FirstNameAm = request.AddDto.FirstNameAm,
                MiddleName = request.AddDto.MiddleName,
                MiddleNameAm = request.AddDto.MiddleNameAm,
                LastName = request.AddDto.LastName,
                LastNameAm = request.AddDto.LastNameAm,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality
            };
            await _unitOfWork.Repository<Person>().Add(per);

            var data = new EmpFamily
            {
                RelationId = request.AddDto.RelationId,
                EmployeeId = request.AddDto.EmployeeId,
                PersonId = per.Id
            };
            await _unitOfWork.Repository<EmpFamily>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpFamilyListDto();
            var response = await _med.Send(new EmpFamilyByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpFamilyModCmdHandler : IRequestHandler<EmpFamilyModCmd, EmpFamilyListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpFamilyModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpFamilyListDto> Handle(EmpFamilyModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpFamily>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"EMPLOYEE FAMILY with Id {request.ModDto.Id} NOT FOUND."); }

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

            oldData.RelationId = request.ModDto.RelationId;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            oldData.PersonId = per.Id;
            var data = await _unitOfWork.Repository<EmpFamily>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpFamilyListDto();
            var response = await _med.Send(new EmpFamilyByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpFamilyDelCmdHandler : IRequestHandler<EmpFamilyDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpFamilyDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpFamilyDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EmpFamily>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}