using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.HRMM.Commands;

public class PosBenefitAddCmd : IRequest<PosBenefitListDto> { public PosBenefitAddDto AddDto { get; set; } = default!; }
public class PosBenefitModCmd : IRequest<PosBenefitListDto> { public PosBenefitModDto ModDto { get; set; } = default!; }
public class PosBenefitDelCmd : IRequest { public Guid Id { get; set; } }

public class PosBenefitAddHandler : IRequestHandler<PosBenefitAddCmd, PosBenefitListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosBenefitAddHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PosBenefitListDto> Handle(PosBenefitAddCmd request, CancellationToken ct)
    {
        PositionBenefit data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new PositionBenefit
            {
                BenefitSettingId = request.AddDto.BenefitSettingId,
                PositionId = request.AddDto.PositionId
            };
            await _uow.AddAsync(data, token);
        }, ct: ct);

        var response = await _med.Send(new PosBenefitByIdQry { Id = data.Id }, ct);
        return response ?? new PosBenefitListDto();
    }
}

public class PosBenefitModHandler : IRequestHandler<PosBenefitModCmd, PosBenefitListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosBenefitModHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PosBenefitListDto> Handle(PosBenefitModCmd request, CancellationToken ct)
    {
        PositionBenefit? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<PositionBenefit>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"POSITION BENEFIT with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.BenefitSettingId = request.ModDto.BenefitSettingId;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            _uow.Update(oldData);
        }, ct: ct);

        var response = await _med.Send(new PosBenefitByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new PosBenefitListDto();
    }
}

public class PosBenefitDelHandler : IRequestHandler<PosBenefitDelCmd>
{
    private readonly IUnitOfWork _uow;

    public PosBenefitDelHandler(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task Handle(PosBenefitDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<PositionBenefit>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"POSITION BENEFIT with id [{request.Id}] NOT FOUND.");
            }

            _uow.Delete(data);
        }, ct: ct);
    }
}