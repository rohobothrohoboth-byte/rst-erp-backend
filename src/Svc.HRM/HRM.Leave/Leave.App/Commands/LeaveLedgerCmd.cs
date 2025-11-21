using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeaveLedgerAddCmd : IRequest<LeaveLedgerListDto> { public LeaveLedgerAddDto AddDto { get; set; } = default!; }
public class LeaveLedgerModCmd : IRequest<LeaveLedgerListDto> { public LeaveLedgerModDto ModDto { get; set; } = default!; }
public class LeaveLedgerDelCmd : IRequest { public Guid Id { get; set; } }

public class LeaveLedgerAddCmdHandler : IRequestHandler<LeaveLedgerAddCmd, LeaveLedgerListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveLedgerAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveLedgerListDto> Handle(LeaveLedgerAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeaveLedger
            {
                EmployeeId = request.AddDto.EmployeeId,
                LeavePolicyId = request.AddDto.LeavePolicyId,
                Date = DateTime.UtcNow,
                Amount = request.AddDto.Amount,
                EntryType = request.AddDto.EntryType,
                SourceType = request.AddDto.SourceType,
                //BalanceAfter = request.AddDto.BalanceAfter
            };
            await _unitOfWork.Repository<LeaveLedger>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeaveLedgerListDto();
            var response = await _med.Send(new LeaveLedgerByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveLedgerModCmdHandler : IRequestHandler<LeaveLedgerModCmd, LeaveLedgerListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveLedgerModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveLedgerListDto> Handle(LeaveLedgerModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveLedger>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE LEDGER with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.EmployeeId = request.ModDto.EmployeeId;
            oldData.LeavePolicyId = request.ModDto.LeavePolicyId;
            //oldData.Date = DateTime.UtcNow;
            oldData.Amount = request.ModDto.Amount;
            oldData.EntryType = request.ModDto.EntryType;
            oldData.SourceType = request.ModDto.SourceType;
            //oldData.BalanceAfter = request.ModDto.BalanceAfter;
            var data = await _unitOfWork.Repository<LeaveLedger>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeaveLedgerListDto();
            var response = await _med.Send(new LeaveLedgerByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveLedgerDelCmdHandler : IRequestHandler<LeaveLedgerDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveLedgerDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeaveLedgerDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeaveLedger>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE LEDGER with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeaveLedger>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}