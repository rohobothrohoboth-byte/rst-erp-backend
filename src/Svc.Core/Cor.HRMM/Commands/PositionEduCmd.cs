using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;

namespace Cor.HRMM.Commands;

public class PositionEduAddCmd : IRequest<PositionEduListDto> { public PositionEduAddDto AddDto { get; set; } = default!; }
public class PositionEduModCmd : IRequest<PositionEduListDto> { public PositionEduModDto ModDto { get; set; } = default!; }
public class PositionEduDelCmd : IRequest { public Guid Id { get; set; } }

public class PositionEduAddCmdHandler : IRequestHandler<PositionEduAddCmd, PositionEduListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionEduAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionEduListDto> Handle(PositionEduAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new PositionEducation
            {
                PositionId = request.AddDto.PositionId,
                EducationQualId = request.AddDto.EducationQualId,
                EducationLevelId = request.AddDto.EducationLevelId
            };
            await _unitOfWork.Repository<PositionEducation>().Add(data);
            await _unitOfWork.Commit();

            var res = new PositionEduListDto();
            var response = await _med.Send(new PositionEduByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionEduModCmdHandler : IRequestHandler<PositionEduModCmd, PositionEduListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PositionEduModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PositionEduListDto> Handle(PositionEduModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PositionEducation>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"POSITION EDUCATION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.PositionId = request.ModDto.PositionId;
            oldData.EducationQualId = request.ModDto.EducationQualId;
            oldData.EducationLevelId = request.ModDto.EducationLevelId;
            var data = await _unitOfWork.Repository<PositionEducation>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PositionEduListDto();
            var response = await _med.Send(new PositionEduByIdQry { Id = data.Id }, cancellationToken);
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

public class PositionEduDelCmdHandler : IRequestHandler<PositionEduDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionEduDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PositionEduDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PositionEducation>().GetById(request.Id);
            if (data == null) { throw new DomainException($"POSITION EDUCATION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PositionEducation>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}