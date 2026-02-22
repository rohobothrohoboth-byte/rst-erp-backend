using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class EvalTypeAllQry : IRequest<List<EvalTypeListDto>> { }
public class EvalTypeActiveQry : IRequest<List<EvalTypeListDto>> { }
public class EvalTypeByIdQry : IRequest<EvalTypeListDto?> { public Guid Id { get; set; } }
public class EvalFlowAllQry : IRequest<List<EvalFlowListDto>> { }
public class EvalFlowActiveQry : IRequest<List<EvalFlowListDto>> { }
public class EvalFlowByIdQry : IRequest<EvalFlowListDto?> { public Guid Id { get; set; } }
public class EvalStepAllQry : IRequest<List<EvalStepListDto>> { public Guid Id { get; set; } }
public class EvalStepByIdQry : IRequest<EvalStepListDto?> { public Guid Id { get; set; } }
public class JobEvalFlowAllQry : IRequest<List<JobEvalFlowListDto>> { }
public class JobEvalFlowByIdQry : IRequest<JobEvalFlowListDto?> { public Guid Id { get; set; } }



public class EvalTypeAllHandler : IRequestHandler<EvalTypeAllQry, List<EvalTypeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalTypeAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EvalTypeListDto>> Handle(EvalTypeAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EvaluationType>().GetAll();
        var dataL = new List<EvalTypeListDto>();

        foreach (var data in dbData)
        {
            var c = new EvalTypeListDto
            {
                Id = data.Id,
                Name = data.Name,
                MaxScore = data.MaxScore,
                IsActive = data.IsActive,
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
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

public class EvalTypeActiveHandler : IRequestHandler<EvalTypeActiveQry, List<EvalTypeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalTypeActiveHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EvalTypeListDto>> Handle(EvalTypeActiveQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EvaluationType>().Find(t => t.IsActive);
        var dataL = new List<EvalTypeListDto>();

        foreach (var data in dbData)
        {
            var c = new EvalTypeListDto
            {
                Id = data.Id,
                Name = data.Name,
                MaxScore = data.MaxScore,
                IsActive = data.IsActive,
                IsActiveStr = "Active",
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

public class EvalTypeByIdHandler : IRequestHandler<EvalTypeByIdQry, EvalTypeListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalTypeByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EvalTypeListDto?> Handle(EvalTypeByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EvaluationType>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new EvalTypeListDto
        {
            Id = data.Id,
            Name = data.Name,
            MaxScore = data.MaxScore,
            IsActive = data.IsActive,
            IsActiveStr = BoolToStr.FormatStat(data.IsActive),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class EvalFlowAllHandler : IRequestHandler<EvalFlowAllQry, List<EvalFlowListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalFlowAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EvalFlowListDto>> Handle(EvalFlowAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EvaluationFlow>().GetAll();
        var dataL = new List<EvalFlowListDto>();

        foreach (var data in dbData)
        {
            var c = new EvalFlowListDto
            {
                Id = data.Id,
                Name = data.Name,
                IsGlobal = data.IsGlobal,
                IsActive = data.IsActive,
                IsGlobalStr = BoolToStr.FormatBool(data.IsGlobal),
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
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

public class EvalFlowActiveHandler : IRequestHandler<EvalFlowActiveQry, List<EvalFlowListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalFlowActiveHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EvalFlowListDto>> Handle(EvalFlowActiveQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EvaluationFlow>().Find(t => t.IsActive);
        var dataL = new List<EvalFlowListDto>();

        foreach (var data in dbData)
        {
            var c = new EvalFlowListDto
            {
                Id = data.Id,
                Name = data.Name,
                IsGlobal = data.IsGlobal,
                IsActive = data.IsActive,
                IsGlobalStr = BoolToStr.FormatBool(data.IsGlobal),
                IsActiveStr = "Active",
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

public class EvalFlowByIdHandler : IRequestHandler<EvalFlowByIdQry, EvalFlowListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalFlowByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EvalFlowListDto?> Handle(EvalFlowByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EvaluationFlow>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new EvalFlowListDto
        {
            Id = data.Id,
            Name = data.Name,
            IsGlobal = data.IsGlobal,
            IsActive = data.IsActive,
            IsGlobalStr = BoolToStr.FormatBool(data.IsGlobal),
            IsActiveStr = BoolToStr.FormatStat(data.IsActive),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class EvalStepAllHandler : IRequestHandler<EvalStepAllQry, List<EvalStepListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalStepAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EvalStepListDto>> Handle(EvalStepAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EvaluationStep>().Find(s => s.EvaluationFlowId == request.Id);
        var dataL = new List<EvalStepListDto>();
        var eTypes = await _unitOfWork.Repository<EvaluationType>().GetAll();
        var eFlow = await _unitOfWork.Repository<EvaluationFlow>().GetById(request.Id);

        foreach (var data in dbData)
        {
            var eType = eTypes.FirstOrDefault(t => t.Id == data.EvalTypeId);
            var c = new EvalStepListDto
            {
                Id = data.Id,
                StepName = data.StepName,
                StepOrder = data.StepOrder,
                EvaluationFlow = eFlow != null ? eFlow.Name : "NOT AVAILABLE",
                EvalType = eType != null ? eType.Name : "NOT AVAILABLE",
                IsFinal = data.IsFinal,
                IsFinalStr = BoolToStr.FormatBool(data.IsFinal),
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

public class EvalStepByIdHandler : IRequestHandler<EvalStepByIdQry, EvalStepListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EvalStepByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EvalStepListDto?> Handle(EvalStepByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<EvaluationStep>().GetById(request.Id);
        if (data == null) { return null; }
        var eFlow = await _unitOfWork.Repository<EvaluationFlow>().GetById(data.EvaluationFlowId);
        var eType = await _unitOfWork.Repository<EvaluationType>().GetById(data.EvalTypeId);

        var c = new EvalStepListDto
        {
            Id = data.Id,
            StepName = data.StepName,
            StepOrder = data.StepOrder,
            EvaluationFlow = eFlow != null ? eFlow.Name : "NOT AVAILABLE",
            EvalType = eType != null ? eType.Name : "NOT AVAILABLE",
            IsFinal = data.IsFinal,
            IsFinalStr = BoolToStr.FormatBool(data.IsFinal),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class JobEvalFlowAllHandler : IRequestHandler<JobEvalFlowAllQry, List<JobEvalFlowListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobEvalFlowAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<JobEvalFlowListDto>> Handle(JobEvalFlowAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<JobPostEvalFlow>().GetAll();
        var dataL = new List<JobEvalFlowListDto>();
        var eFlowL = await _unitOfWork.Repository<EvaluationFlow>().GetAll();
        var jPostL = await _unitOfWork.Repository<JobPosting>().GetAll();

        foreach (var data in dbData)
        {
            var eFlow = eFlowL.FirstOrDefault(t => t.Id == data.EvaluationFlowId);
            var jPost = jPostL.FirstOrDefault(t => t.Id == data.JobPostingId);
            var c = new JobEvalFlowListDto
            {
                Id = data.Id,
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

public class JobEvalFlowByIdHandler : IRequestHandler<JobEvalFlowByIdQry, JobEvalFlowListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobEvalFlowByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<JobEvalFlowListDto?> Handle(JobEvalFlowByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JobPostEvalFlow>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new JobEvalFlowListDto
        {
            Id = data.Id,

            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}