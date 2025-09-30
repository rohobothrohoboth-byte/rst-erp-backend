using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using MediatR;

namespace Cor.HRMM.Commands;

public class EducationQualAddCmd : IRequest<EducationQualListDto> { public EducationQualAddDto AddDto { get; set; } = default!; }

public class EducationQualModCmd : IRequest<EducationQualListDto> { public EducationQualModDto ModDto { get; set; } = default!; }

public class EducationQualDelCmd : IRequest { public Guid Id { get; set; } }

public class EducationQualAddCmdHandler : IRequestHandler<EducationQualAddCmd, EducationQualListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EducationQualAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EducationQualListDto> Handle(EducationQualAddCmd request, CancellationToken cancellationToken)
    {
        var data = new EducationQual
        {
            Name = request.AddDto.Name
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EducationQual>().Add(data);
            await _unitOfWork.Commit();
            
            var res = new EducationQualListDto();
            var response = await _med.Send(new EducationQualByIdQry { Id = data.Id }, cancellationToken);
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

public class EducationQualModCmdHandler : IRequestHandler<EducationQualModCmd, EducationQualListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EducationQualModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EducationQualListDto> Handle(EducationQualModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EducationQual>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new KeyNotFoundException($"EDUCATION QUALIFICATION with Id {request.ModDto.Id} NOT FOUND."); }

        oldData.Name = request.ModDto.Name;
        await _unitOfWork.Begin();

        try
        {
            var data = await _unitOfWork.Repository<EducationQual>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EducationQualListDto();
            var response = await _med.Send(new EducationQualByIdQry { Id = data.Id }, cancellationToken);
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

public class EducationQualDelCmdHandler : IRequestHandler<EducationQualDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EducationQualDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EducationQualDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<EducationQual>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}