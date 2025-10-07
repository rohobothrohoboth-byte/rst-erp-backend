using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmContactAddCmd : IRequest<EmContactListDto> { public EmContactAddDto AddDto { get; set; } = default!; }
public class EmContactModCmd : IRequest<EmContactListDto> { public EmContactModDto ModDto { get; set; } = default!; }
public class EmContactDelCmd : IRequest { public Guid Id { get; set; } }

public class EmContactAddCmdHandler : IRequestHandler<EmContactAddCmd, EmContactListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmContactAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmContactListDto> Handle(EmContactAddCmd request, CancellationToken cancellationToken)
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

            var data = new EmergencyContact
            {
                AddressId = request.AddDto.AddressId,
                RelationId = request.AddDto.RelationId,
                EmployeeId = request.AddDto.EmployeeId,
                PersonId = per.Id
            };
            await _unitOfWork.Repository<EmergencyContact>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmContactListDto();
            var response = await _med.Send(new EmContactByIdQry { Id = data.Id }, cancellationToken);
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

public class EmContactModCmdHandler : IRequestHandler<EmContactModCmd, EmContactListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmContactModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmContactListDto> Handle(EmContactModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmergencyContact>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"EMERGENCY CONTACT with Id {request.ModDto.Id} NOT FOUND."); }

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

            oldData.AddressId = request.ModDto.AddressId;
            oldData.RelationId = request.ModDto.RelationId;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            oldData.PersonId = per.Id;
            var data = await _unitOfWork.Repository<EmergencyContact>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmContactListDto();
            var response = await _med.Send(new EmContactByIdQry { Id = data.Id }, cancellationToken);
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

public class EmContactDelCmdHandler : IRequestHandler<EmContactDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmContactDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmContactDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EmergencyContact>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}