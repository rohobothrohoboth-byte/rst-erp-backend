using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class BenefitSetAddCmd : IRequest<BenefitSetListDto> { public BenefitSetAddDto AddDto { get; set; } = default!; }
public class BenefitSetModCmd : IRequest<BenefitSetListDto> { public BenefitSetModDto ModDto { get; set; } = default!; }
public class BenefitSetDelCmd : IRequest { public Guid Id { get; set; } }



public class BenefitSetAddHandler : IRequestHandler<BenefitSetAddCmd, BenefitSetListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public BenefitSetAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<BenefitSetListDto> Handle(BenefitSetAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new BenefitSetting
            {
                Name = request.AddDto.Name,
                BenefitValue = request.AddDto.BenefitValue,
                Per = request.AddDto.Per
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new BenefitSetListDto();
            var response = await _med.Send(new BenefitSetByIdQry { Id = data.Id }, ct);
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

public class BenefitSetModHandler : IRequestHandler<BenefitSetModCmd, BenefitSetListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public BenefitSetModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<BenefitSetListDto> Handle(BenefitSetModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<BenefitSetting>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"BENEFIT SETTING with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.BenefitValue = request.ModDto.BenefitValue;
            oldData.Per = request.ModDto.Per;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new BenefitSetListDto();
            var response = await _med.Send(new BenefitSetByIdQry { Id = request.ModDto.Id }, ct);
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

public class BenefitSetDelHandler : IRequestHandler<BenefitSetDelCmd>
{
    private readonly IUnitOfWork _uow;
    public BenefitSetDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(BenefitSetDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<BenefitSetting>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"BENEFIT SETTING with id [{request.Id}] NOT FOUND."); }
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