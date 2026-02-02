using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class AddressNameAllQry : IRequest<List<NameList>> { }
public class AddressNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class EmpNameAllQry : IRequest<List<NameList>> { }
public class EmpNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class EmpPolicyAllQry : IRequest<List<EmpPolicyCtx>> { }
public class EmpPolicyByIdQry : IRequest<EmpPolicyCtx?> { public Guid Id { get; set; } }


public class AddressNameAllHandler : IRequestHandler<AddressNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressNameAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(AddressNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Address>().GetAll();
        var dataL = new List<NameList>();

        foreach (var data in dbData)
        {
            var c = new NameList
            {
                Id = data.Id,
                Name = $"{((AddressType)Enum.Parse(typeof(AddressType), data.AddressType)).ToDisplayName()}: {data.Region} | {data.Zone}({data.Subcity}) | {data.Woreda} | {data.Kebele})"
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class AddressNameByIdHandler : IRequestHandler<AddressNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddressNameByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(AddressNameByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Address>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new NameList
        {
            Id = data.Id,
            Name = $"{((AddressType)Enum.Parse(typeof(AddressType), data.AddressType)).ToDisplayName()}: {data.Region} | {data.Zone}({data.Subcity}) | {data.Woreda} | {data.Kebele})"
        };
        return c;
    }
}

public class EmpNameAllHandler : IRequestHandler<EmpNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpNameAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(EmpNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Employee>().GetAll();
        var dataL = new List<NameList>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(p => p.Id == data.PersonId);
            if (per == null) continue;
            var c = new NameList
            {
                Id = data.Id,
                Name = $"{per.FirstName} {per.MiddleName} {per.LastName}"
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class EmpNameByIdHandler : IRequestHandler<EmpNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpNameByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(EmpNameByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }

        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        if (per == null) { return null; }

        var c = new NameList
        {
            Id = data.Id,
            Name = $"{per.FirstName} {per.MiddleName} {per.LastName}"
        };
        return c;
    }
}

public class EmpPolicyAllHandler : IRequestHandler<EmpPolicyAllQry, List<EmpPolicyCtx>>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpPolicyAllHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<EmpPolicyCtx>> Handle(EmpPolicyAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Employee>().GetAll();
        var dataL = new List<EmpPolicyCtx>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(p => p.Id == data.PersonId);
            if (per == null) continue;
            var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);

            var c = new EmpPolicyCtx
            {
                EmployeeId = data.Id,
                Name = $"{per.FirstName} {per.MiddleName} {per.LastName}",
                Gender = per.Gender,
                EmpType = data.EmploymentType,
                Jg = data.JobGradeId.ToString(),
                WorkAr = data.WorkArrangement,
                SerYear = ser
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class EmpPolicyByIdHandler : IRequestHandler<EmpPolicyByIdQry, EmpPolicyCtx?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpPolicyByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpPolicyCtx?> Handle(EmpPolicyByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }

        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        if (per == null) { return null; }
        var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);

        var c = new EmpPolicyCtx
        {
            EmployeeId = data.Id,
            Name = $"{per.FirstName} {per.MiddleName} {per.LastName}",
            Gender = per.Gender,
            EmpType = data.EmploymentType,
            Jg = data.JobGradeId.ToString(),
            WorkAr = data.WorkArrangement,
            SerYear = ser
        };
        return c;
    }
}