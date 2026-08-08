using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.HRMM.Commands;

public class PosEduAddCmd : IRequest<PositionEduListDto> { public PositionEduAddDto AddDto { get; set; } = default!; }
public class PosEduModCmd : IRequest<PositionEduListDto> { public PositionEduModDto ModDto { get; set; } = default!; }
public class PosEduDelCmd : IRequest { public Guid Id { get; set; } }

public class PosEduAddHandler : IRequestHandler<PosEduAddCmd, PositionEduListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosEduAddHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PositionEduListDto> Handle(PosEduAddCmd request, CancellationToken ct)
    {
        PositionEducation data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new PositionEducation
            {
                PositionId = request.AddDto.PositionId,
                EducationQualId = request.AddDto.EducationQualId,
                EducationLevel = request.AddDto.EducationLevel
            };
            await _uow.AddAsync(data, token);
        }, ct: ct);

        var response = await _med.Send(new PositionEduByIdQry { Id = data.Id }, ct);
        return response ?? new PositionEduListDto();
    }
}

public class PosEduModHandler : IRequestHandler<PosEduModCmd, PositionEduListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosEduModHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PositionEduListDto> Handle(PosEduModCmd request, CancellationToken ct)
    {
        PositionEducation? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<PositionEducation>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"POSITION EDUCATION with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.PositionId = request.ModDto.PositionId;
            oldData.EducationQualId = request.ModDto.EducationQualId;
            oldData.EducationLevel = request.ModDto.EducationLevel;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            _uow.Update(oldData);
        }, ct: ct);

        var response = await _med.Send(new PositionEduByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new PositionEduListDto();
    }
}

public class PosEduDelHandler : IRequestHandler<PosEduDelCmd>
{
    private readonly IUnitOfWork _uow;

    public PosEduDelHandler(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task Handle(PosEduDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<PositionEducation>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"POSITION EDUCATION with id [{request.Id}] NOT FOUND.");
            }

            _uow.Delete(data);
        }, ct: ct);
    }
}