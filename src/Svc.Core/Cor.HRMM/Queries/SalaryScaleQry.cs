using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class SalaryScaleAllQry : IRequest<List<SalaryScaleListDto>> { }

public class SalaryScaleByIdQry : IRequest<SalaryScaleListDto?> { public Guid Id { get; set; } }

public class SalaryScaleAllQryHandler : IRequestHandler<SalaryScaleAllQry, List<SalaryScaleListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public SalaryScaleAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<SalaryScaleListDto>> Handle(SalaryScaleAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<SalaryScale>().GetAll();
        var dataL = new List<SalaryScaleListDto>();
        var jobGradeL = await _unitOfWork.Repository<JobGrade>().GetAll();

        foreach (var data in dbData)
        {
            var jobGrade = jobGradeL.FirstOrDefault(t => t.Id == data.JobGradeId);
            var c = new SalaryScaleListDto
            {
                Id = data.Id,
                JobGradeId = data.JobGradeId,
                Salary = data.Salary,
                JobGrade = jobGrade != null ? jobGrade.Name : "JOB GRADE NOT AVAILABLE",
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

public class SalaryScaleByIdQryHandler : IRequestHandler<SalaryScaleByIdQry, SalaryScaleListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public SalaryScaleByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<SalaryScaleListDto?> Handle(SalaryScaleByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<SalaryScale>().GetById(request.Id);
        if (data == null) { return null; }
        var jobGrade = await _unitOfWork.Repository<JobGrade>().GetById(data.JobGradeId);

        var c = new SalaryScaleListDto
        {
            Id = data.Id,
            JobGradeId = data.JobGradeId,
            Salary = data.Salary,
            JobGrade = jobGrade != null ? jobGrade.Name : "JOB GRADE NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}