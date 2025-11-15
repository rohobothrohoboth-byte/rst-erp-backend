using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpBioModCmd : IRequest<EmpBioListDto> { public EmpBioModDto ModDto { get; set; } = default!; }
public class EmpBioDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpBioModCmdHandler : IRequestHandler<EmpBioModCmd, EmpBioListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpBioModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpBioListDto> Handle(EmpBioModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpBio>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EMPLOYEE BIO with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.BirthDate = request.ModDto.BirthDate;
            oldData.BirthLocation = request.ModDto.BirthLocation;
            oldData.MotherFullName = request.ModDto.MotherFullName;
            oldData.HasBirthCert = request.ModDto.HasBirthCert;
            oldData.HasMarriageCert = request.ModDto.HasMarriageCert;
            oldData.MaritalStatus = request.ModDto.MaritalStatus;
            oldData.AddressId = request.ModDto.AddressId;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            var data = await _unitOfWork.Repository<EmpBio>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpBioListDto();
            var response = await _med.Send(new EmpBioByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpBioDelCmdHandler : IRequestHandler<EmpBioDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpBioDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpBioDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<EmpBio>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EMPLOYEE BIO with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<EmpBio>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}