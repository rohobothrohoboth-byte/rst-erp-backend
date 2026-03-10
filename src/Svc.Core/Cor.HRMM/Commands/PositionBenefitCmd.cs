using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class PosBenefitAddCmd : IRequest<PosBenefitListDto> { public PosBenefitAddDto AddDto { get; set; } = default!; }
public class PosBenefitModCmd : IRequest<PosBenefitListDto> { public PosBenefitModDto ModDto { get; set; } = default!; }
public class PosBenefitDelCmd : IRequest { public Guid Id { get; set; } }



public class PosBenefitAddHandler : IRequestHandler<PosBenefitAddCmd, PosBenefitListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosBenefitAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PosBenefitListDto> Handle(PosBenefitAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new PositionBenefit
            {
                BenefitSettingId = request.AddDto.BenefitSettingId,
                PositionId = request.AddDto.PositionId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new PosBenefitListDto();
            var response = await _med.Send(new PosBenefitByIdQry { Id = data.Id }, ct);
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

public class PosBenefitModHandler : IRequestHandler<PosBenefitModCmd, PosBenefitListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosBenefitModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PosBenefitListDto> Handle(PosBenefitModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PositionBenefit>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"POSITION BENEFIT with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.BenefitSettingId = request.ModDto.BenefitSettingId;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PosBenefitListDto();
            var response = await _med.Send(new PosBenefitByIdQry { Id = request.ModDto.Id }, ct);
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

public class PosBenefitDelHandler : IRequestHandler<PosBenefitDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PosBenefitDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(PosBenefitDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PositionBenefit>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POSITION BENEFIT with id [{request.Id}] NOT FOUND."); }
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