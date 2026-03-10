using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class EducationQualAddCmd : IRequest<EducationQualListDto> { public EducationQualAddDto AddDto { get; set; } = default!; }
public class EducationQualModCmd : IRequest<EducationQualListDto> { public EducationQualModDto ModDto { get; set; } = default!; }
public class EducationQualDelCmd : IRequest { public Guid Id { get; set; } }



public class EducationQualAddHandler : IRequestHandler<EducationQualAddCmd, EducationQualListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public EducationQualAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<EducationQualListDto> Handle(EducationQualAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EducationQual
            {
                Name = request.AddDto.Name
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);
            
            var res = new EducationQualListDto();
            var response = await _med.Send(new EducationQualByIdQry { Id = data.Id }, ct);
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

public class EducationQualModHandler : IRequestHandler<EducationQualModCmd, EducationQualListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EducationQualModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<EducationQualListDto> Handle(EducationQualModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EducationQual>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"EDUCATION QUALIFICATION with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EducationQualListDto();
            var response = await _med.Send(new EducationQualByIdQry { Id = request.ModDto.Id }, ct);
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

public class EducationQualDelHandler : IRequestHandler<EducationQualDelCmd>
{
    private readonly IUnitOfWork _uow;
    public EducationQualDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(EducationQualDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EducationQual>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"EDUCATION QUALIFICATION with id [{request.Id}] NOT FOUND."); }
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