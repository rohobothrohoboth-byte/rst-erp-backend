using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpGuarantorModCmd : IRequest<EmpGuarantorListDto> { public EmpGuarantorModDto ModDto { get; set; } = default!; }
public class EmpGuarantorDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpGuarantorModCmdHandler : IRequestHandler<EmpGuarantorModCmd, EmpGuarantorListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpGuarantorModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpGuarantorListDto> Handle(EmpGuarantorModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpGuarantor>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EMPLOYEE GUARANTOR with Id {request.ModDto.Id} NOT FOUND."); }

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
            var data = await _unitOfWork.Repository<EmpGuarantor>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpGuarantorListDto();
            var response = await _med.Send(new EmpGuarantorByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpGuarantorDelCmdHandler : IRequestHandler<EmpGuarantorDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpGuarantorDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpGuarantorDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<EmpGuarantor>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EMPLOYEE GUARANTOR with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<EmpGuarantor>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}