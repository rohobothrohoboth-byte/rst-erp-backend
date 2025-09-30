using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class JobGradeAllQry : IRequest<List<JobGradeListDto>> { }

public class JobGradeByIdQry : IRequest<JobGradeListDto?> { public Guid Id { get; set; } }

public class JobGradeAllQryHandler : IRequestHandler<JobGradeAllQry, List<JobGradeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobGradeAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<JobGradeListDto>> Handle(JobGradeAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<JobGrade>().GetAll();
        var dataL = new List<JobGradeListDto>();

        foreach (var data in dbData)
        {
            var c = new JobGradeListDto
            {
                Id = data.Id,
                Name = data.Name,
                Code = data.Code,
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

public class JobGradeByIdQryHandler : IRequestHandler<JobGradeByIdQry, JobGradeListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobGradeByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<JobGradeListDto?> Handle(JobGradeByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<JobGrade>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new JobGradeListDto
        {
            Id = nData.Id,
            Name = nData.Name,
            Code = nData.Code,
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };
        return c;
    }
}