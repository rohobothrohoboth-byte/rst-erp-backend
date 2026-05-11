using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpModBasicQry : IRequest<EmpModBasicDto> { public Guid Id { get; set; } }
public class EmpModBioQry : IRequest<EmpModBioDto> { public Guid Id { get; set; } }
public class EmpModGuarQry : IRequest<EmpModGuarDto> { public Guid Id { get; set; } }



public class EmpModBasicHandler(IDapperHelper _dapper) : IRequestHandler<EmpModBasicQry, EmpModBasicDto>
{
    public async Task<EmpModBasicDto> Handle(EmpModBasicQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string es = "es";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.EmploymentDate, x => x.JobGradeId, x => x.PositionId, x => x.DepartmentId, x => x.xmin)
            .Select<Person>(p, x => x.FirstName, x => x.FirstNameAm, x => x.MiddleName, x => x.MiddleNameAm, x => x.LastName, x => x.LastNameAm, x => x.Gender, x => x.Nationality)
            .Select<EmpSalary>(es, x => x.JgStepId)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpSalary>(e, es, x => x.Id, x => x.EmployeeId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpModBasicDto>(sql, parameters, ct);
        if (data == null) return new EmpModBasicDto();

        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class EmpModBioHandler(IDapperHelper _dapper) : IRequestHandler<EmpModBioQry, EmpModBioDto>
{
    public async Task<EmpModBioDto> Handle(EmpModBioQry request, CancellationToken ct)
    {
        const string eb = "eb";
        const string ad = "ad";
        const string ef = "ef";
        var qb = new QueryBuilder()
            .Select<EmpBio>(eb, x => x.Id, x => x.BirthDate, x => x.BirthLocation, x => x.MotherFullName, x => x.MaritalStatus, x => x.xmin)
            .Select<Address>(ad, x => x.AddressType, x => x.Country, x => x.Region, x => x.Subcity, x => x.Zone, x => x.Woreda, x => x.Kebele, x => x.HouseNo, x => x.Telephone, x => x.PoBox, x => x.Fax, x => x.Email, x => x.Website)
            .Select<EmpFinance>(ef, x => x.Tin, x => x.BankAccountNo, x => x.PensionNumber)
            .From<EmpBio>(eb)
            .LeftJoin<EmpBio, Address>(eb, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<EmpBio, EmpFinance>(eb, ef, x => x.EmployeeId, x => x.EmployeeId)
            .Where<EmpBio>(eb, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpModBioDto>(sql, parameters, ct);
        if (data == null) return new EmpModBioDto { EmployeeId = request.Id, HasData = false };

        data.EmployeeId = request.Id;
        data.HasData = true;
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class EmpModGuarHandler(IDapperHelper _dapper) : IRequestHandler<EmpModGuarQry, EmpModGuarDto>
{
    public async Task<EmpModGuarDto> Handle(EmpModGuarQry request, CancellationToken ct)
    {
        const string eg = "eg";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<EmpGuarantor>(eg, x => x.Relation, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality, x => x.xmin)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .From<EmpGuarantor>(eg)
            .Join<EmpGuarantor, Address>(eg, ad, x => x.AddressId, x => x.Id)
            .Where<EmpGuarantor>(eg, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpModGuarDto>(sql, parameters, ct);
        if (data == null) return new EmpModGuarDto { EmployeeId = request.Id, HasData = false };

        data.EmployeeId = request.Id;
        data.HasData = true;
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}