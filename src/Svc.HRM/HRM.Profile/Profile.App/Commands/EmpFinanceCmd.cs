using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpFinanceAddCmd : IRequest<EmpFinanceListDto> { public EmpFinanceAddDto AddDto { get; set; } = default!; }
public class EmpFinanceModCmd : IRequest<EmpFinanceListDto> { public EmpFinanceModDto ModDto { get; set; } = default!; }
public class EmpFinanceDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpFinanceAddCmdHandler : IRequestHandler<EmpFinanceAddCmd, EmpFinanceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpFinanceAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpFinanceListDto> Handle(EmpFinanceAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EmpFinance
            {
                Tin = request.AddDto.Tin,
                BankAccountNo = request.AddDto.BankAccountNo,
                PensionNumber = request.AddDto.PensionNumber,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpFinance>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpFinanceListDto();
            var response = await _med.Send(new EmpFinanceByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpFinanceModCmdHandler : IRequestHandler<EmpFinanceModCmd, EmpFinanceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpFinanceModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpFinanceListDto> Handle(EmpFinanceModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpFinance>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"EMPLOYEE FINANCE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();

        try
        {
            oldData.Tin = request.ModDto.Tin;
            oldData.BankAccountNo = request.ModDto.BankAccountNo;
            oldData.PensionNumber = request.ModDto.PensionNumber;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            var data = await _unitOfWork.Repository<EmpFinance>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpFinanceListDto();
            var response = await _med.Send(new EmpFinanceByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpFinanceDelCmdHandler : IRequestHandler<EmpFinanceDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpFinanceDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpFinanceDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EmpFinance>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}