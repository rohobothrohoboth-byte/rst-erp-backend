using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionExpAllQry : IRequest<List<PositionExpListDto>> { public Guid Id { get; set; } }

public class PositionExpByIdQry : IRequest<PositionExpListDto?> { public Guid Id { get; set; } }

public class PositionExpAllQryHandler : IRequestHandler<PositionExpAllQry, List<PositionExpListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionExpAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PositionExpListDto>> Handle(PositionExpAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PositionExp>().Find(c => c.PositionId == request.Id);
        var dataL = new List<PositionExpListDto>();
        var nData = dbData.ToList();
        if (nData.Count <= 0) return dataL;

        foreach (var data in dbData)
        {
            var c = new PositionExpListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                SamePosExp = data.SamePosExp,
                OtherPosExp = data.OtherPosExp,
                MinAge = data.MinAge,
                MaxAge = data.MaxAge,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class PositionExpByIdQryHandler : IRequestHandler<PositionExpByIdQry, PositionExpListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionExpByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PositionExpListDto?> Handle(PositionExpByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PositionExp>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new PositionExpListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            SamePosExp = data.SamePosExp,
            OtherPosExp = data.OtherPosExp,
            MinAge = data.MinAge,
            MaxAge = data.MaxAge,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion),
        };
        return c;
    }
}