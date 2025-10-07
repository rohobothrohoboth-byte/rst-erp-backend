using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpPensionCardAddCmd : IRequest<EmpPensionCardListDto> { public EmpPensionCardAddDto AddDto { get; set; } = default!; }
public class EmpPensionCardModCmd : IRequest<EmpPensionCardListDto> { public EmpPensionCardModDto ModDto { get; set; } = default!; }
public class EmpPensionCardDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpPensionCardAddCmdHandler : IRequestHandler<EmpPensionCardAddCmd, EmpPensionCardListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpPensionCardAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpPensionCardListDto> Handle(EmpPensionCardAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EmpPensionCard
            {
                RegistrationDate = request.AddDto.RegistrationDate,
                SentDate = request.AddDto.SentDate,
                ReceivedDate = request.AddDto.ReceivedDate,
                IsReceived = request.AddDto.IsReceived,
                IsSent = request.AddDto.IsSent,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpPensionCard>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpPensionCardListDto();
            var response = await _med.Send(new EmpPensionCardByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpPensionCardModCmdHandler : IRequestHandler<EmpPensionCardModCmd, EmpPensionCardListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpPensionCardModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpPensionCardListDto> Handle(EmpPensionCardModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpPensionCard>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"EMPLOYEE PENSION CARD with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();

        try
        {
            oldData.RegistrationDate = request.ModDto.RegistrationDate;
            oldData.SentDate = request.ModDto.SentDate;
            oldData.ReceivedDate = request.ModDto.ReceivedDate;
            oldData.IsReceived = request.ModDto.IsReceived;
            oldData.IsSent = request.ModDto.IsSent;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            var data = await _unitOfWork.Repository<EmpPensionCard>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpPensionCardListDto();
            var response = await _med.Send(new EmpPensionCardByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpPensionCardDelCmdHandler : IRequestHandler<EmpPensionCardDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpPensionCardDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpPensionCardDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EmpPensionCard>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}