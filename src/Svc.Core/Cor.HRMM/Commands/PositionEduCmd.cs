using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class PosEduAddCmd : IRequest<PositionEduListDto> { public PositionEduAddDto AddDto { get; set; } = default!; }
public class PosEduModCmd : IRequest<PositionEduListDto> { public PositionEduModDto ModDto { get; set; } = default!; }
public class PosEduDelCmd : IRequest { public Guid Id { get; set; } }



public class PosEduAddHandler : IRequestHandler<PosEduAddCmd, PositionEduListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosEduAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionEduListDto> Handle(PosEduAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new PositionEducation
            {
                PositionId = request.AddDto.PositionId,
                EducationQualId = request.AddDto.EducationQualId,
                EducationLevel = request.AddDto.EducationLevel
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new PositionEduListDto();
            var response = await _med.Send(new PositionEduByIdQry { Id = data.Id }, ct);
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

public class PosEduModHandler : IRequestHandler<PosEduModCmd, PositionEduListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosEduModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionEduListDto> Handle(PosEduModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PositionEducation>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"POSITION EDUCATION with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.PositionId = request.ModDto.PositionId;
            oldData.EducationQualId = request.ModDto.EducationQualId;
            oldData.EducationLevel = request.ModDto.EducationLevel;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PositionEduListDto();
            var response = await _med.Send(new PositionEduByIdQry { Id = request.ModDto.Id }, ct);
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

public class PosEduDelHandler : IRequestHandler<PosEduDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PosEduDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(PosEduDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PositionEducation>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POSITION EDUCATION with id [{request.Id}] NOT FOUND."); }
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