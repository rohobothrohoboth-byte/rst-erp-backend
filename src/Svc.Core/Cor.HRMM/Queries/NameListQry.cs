using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Services;
using MediatR;

namespace Cor.HRMM.Queries;

public class AddressNameAllQry : IRequest<List<NameList>> { }
public class AddressNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class BenefitSetNameAllQry : IRequest<List<NameList>> { }
public class BenefitSetNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class EducationQualNameAllQry : IRequest<List<NameList>> { }
public class EducationQualNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class JgStepNameAllQry : IRequest<List<NameList>> { }
public class JgStepNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class JobGradeNameAllQry : IRequest<List<NameList>> { }
public class JobGradeNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class PositionNameAllQry : IRequest<List<NameList>> { }
public class PositionNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }

public class AddressNameAllQryHandler : IRequestHandler<AddressNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AddressNameAllQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<List<NameList>> Handle(AddressNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<NameList>();
        var regionL = await _lupClient.RegionList(cancellationToken);
        var aTypeL = await _lupClient.AddressTypeList(cancellationToken);

        foreach (var data in dbData)
        {
            var region = regionL!.FirstOrDefault(t => t.Id == data.RegionId);
            var aType = aTypeL!.FirstOrDefault(t => t.Id == data.AddressTypeId);
            var c = new NameList
            {
                Id = data.Id,
                Name = $"{aType!.Name}: {region!.Name} | {data.Zone}({data.Subcity} | {data.Woreda} | {data.Kebele})"
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class AddressNameByIdQryHandler : IRequestHandler<AddressNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AddressNameByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _lupClient = lupClient;
    }

    public async Task<NameList?> Handle(AddressNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<Address>().GetById(request.Id);
        if (nData == null) { return null; }
        var region = await _lupClient.Region(nData.RegionId, cancellationToken);
        var aType = await _lupClient.AddressType(nData.AddressTypeId, cancellationToken);

        var c = new NameList
        {
            Id = nData.Id,
            Name = $"{aType!.Name}: {region!.Name} | {nData.Zone}({nData.Subcity} | {nData.Woreda} | {nData.Kebele})"
        };
        return c;
    }
}

public class BenefitSetNameAllQryHandler : IRequestHandler<BenefitSetNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BenefitSetNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(BenefitSetNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<BenefitSetting>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Name }).ToList();
    }
}

public class BenefitSetNameByIdQryHandler : IRequestHandler<BenefitSetNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public BenefitSetNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(BenefitSetNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<BenefitSetting>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new NameList
        {
            Id = nData.Id,
            Name = nData.Name
        };
        return c;
    }
}

public class EducationQualNameAllQryHandler : IRequestHandler<EducationQualNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EducationQualNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(EducationQualNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<EducationQual>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Name }).ToList();
    }
}

public class EducationQualNameByIdQryHandler : IRequestHandler<EducationQualNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EducationQualNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(EducationQualNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<EducationQual>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new NameList
        {
            Id = nData.Id,
            Name = nData.Name
        };
        return c;
    }
}

public class JgStepNameAllQryHandler : IRequestHandler<JgStepNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;

    public JgStepNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(JgStepNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<JgStep>().GetAll();
        var jobGradeL = await _unitOfWork.Repository<JobGrade>().GetAll();

        return (from data in dbData let jobGrade = jobGradeL.FirstOrDefault(t => t.Id == data.JobGradeId) select new NameList { Id = data.Id, Name = jobGrade != null ? $"{data.Name} => {jobGrade.Name}" : $"{data.Name} => JOB GRADE NOT AVAILABLE" }).ToList();
    }
}

public class JgStepNameByIdQryHandler : IRequestHandler<JgStepNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;

    public JgStepNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(JgStepNameByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<JgStep>().GetById(request.Id);
        if (data == null) { return null; }
        var jobGrade = await _unitOfWork.Repository<JobGrade>().GetById(data.JobGradeId);

        var c = new NameList
        {
            Id = data.Id,
            Name = jobGrade != null ? $"{data.Name} => {jobGrade.Name}" : $"{data.Name} => JOB GRADE NOT AVAILABLE"
        };
        return c;
    }
}

public class JobGradeNameAllQryHandler : IRequestHandler<JobGradeNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobGradeNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(JobGradeNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<JobGrade>().GetAll();
        var dataL = new List<NameList>();

        foreach (var data in dbData)
        {
            var c = new NameList
            {
                Id = data.Id,
                Name = data.Name
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class JobGradeNameByIdQryHandler : IRequestHandler<JobGradeNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobGradeNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(JobGradeNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<JobGrade>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new NameList
        {
            Id = nData.Id,
            Name = nData.Name
        };
        return c;
    }
}

public class PositionNameAllQryHandler : IRequestHandler<PositionNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PositionNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(PositionNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Position>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Name }).ToList();
    }
}

public class PositionNameByIdQryHandler : IRequestHandler<PositionNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PositionNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(PositionNameByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Position>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new NameList
        {
            Id = data.Id,
            Name = data.Name
        };
        return c;
    }
}