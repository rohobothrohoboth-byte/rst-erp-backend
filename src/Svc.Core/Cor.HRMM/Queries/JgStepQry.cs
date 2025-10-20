using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using MediatR;

namespace Cor.HRMM.Queries;

public class JgStepAllQry : IRequest<List<JgStepListDto>> { public Guid Id { get; set; } }
public class JgStepByIdQry : IRequest<JgStepListDto?> { public Guid Id { get; set; } }

public class JgStepAllQryHandler : IRequestHandler<JgStepAllQry, List<JgStepListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public JgStepAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<JgStepListDto>> Handle(JgStepAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<JgStep>().Find(c => c.JobGradeId == request.Id);
        var dataL = new List<JgStepListDto>();
        var nData = dbData.ToList();
        if (nData.Count <= 0) return dataL;
        var jobGradeL = await _unitOfWork.Repository<JobGrade>().GetAll();
        foreach (var data in nData)
        {
            var jobGrade = jobGradeL.FirstOrDefault(t => t.Id == data.JobGradeId);
            var c = new JgStepListDto
            {
                Id = data.Id,
                Name = data.Name,
                Salary = data.Salary,
                JobGradeId = data.JobGradeId,
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

public class JgStepByIdQryHandler : IRequestHandler<JgStepByIdQry, JgStepListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public JgStepByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<JgStepListDto?> Handle(JgStepByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JgStep>().GetById(request.Id);
        if (data == null) { return null; }
        var jobGrade = await _unitOfWork.Repository<JobGrade>().GetById(data.JobGradeId);

        var c = new JgStepListDto
        {
            Id = data.Id,
            Name = data.Name,
            Salary = data.Salary,
            JobGradeId = data.JobGradeId,
            JobGrade = jobGrade != null ? jobGrade.Name : "JOB GRADE NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}