using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class ProfileInfoQry : IRequest<ProfileInfo?> { public Guid Id { get; set; } }
public class ProfileCardQry : IRequest<ProfileCard?> { public Guid Id { get; set; } }
public class ProBasicQry : IRequest<ProBasicInfo?> { public Guid Id { get; set; } }
public class ProSalaryQry : IRequest<ProSalary?> { public Guid Id { get; set; } }
public class ProAddressQry : IRequest<ProBasicAddress?> { public Guid Id { get; set; } }
public class ProBioQry : IRequest<ProBasicBio?> { public Guid Id { get; set; } }



public class ProfileInfoHandler(IDapperHelper dapper, ICorHrmmClient corHrmm) : IRequestHandler<ProfileInfoQry, ProfileInfo?>
{
    public async Task<ProfileInfo?> Handle(ProfileInfoQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.EmpState, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await dapper.QueryFirstOrDefaultAsync<ProfileInfoJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var positionTask = await corHrmm.GetPosition(row.PositionId.ToString(), ct);
        var position = positionTask.Res;

        return new ProfileInfo
        {
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}".Trim(),
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}".Trim(),
            EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
            Position = position?.Name ?? "",
        };
    }
}

public class ProfileCardHandler(IDapperHelper dapper) : IRequestHandler<ProfileCardQry, ProfileCard?>
{
    public async Task<ProfileCard?> Handle(ProfileCardQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.EmploymentDate)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await dapper.QueryFirstOrDefaultAsync<ProfileCardJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var serStr = row.EmploymentDate.FullServDur();
        return new ProfileCard
        {
            Tenure = serStr,
            PerStr = $"{4.5} / {5}".Trim(),
            Training = "2",
            AttendPer = 78.0,
            AttendMonth = "May 2026",
            RepToName = "Sarah Johnson",
            RepToPos = "Team Lead"
        };
    }
}

public class ProBasicHandler(IDapperHelper dapper, ICorHrmmClient corHrmm, ICorModClient corMod) : IRequestHandler<ProBasicQry, ProBasicInfo?>
{
    public async Task<ProBasicInfo?> Handle(ProBasicQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string eb = "eb";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.EmploymentDate, x => x.DepartmentId, x => x.PositionId)
            .Select<Person>(p, x => x.Gender, x => x.Nationality)
            .Select<EmpBio>(eb, x => x.BirthDate, x => x.MaritalStatus)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpBio>(e, eb, x => x.Id, x => x.EmployeeId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await dapper.QueryFirstOrDefaultAsync<ProBasicJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var deptTask = corMod.GetDept(row.DepartmentId.ToString(), ct);
        var positionTask = corHrmm.GetPosition(row.PositionId.ToString(), ct);
        await Task.WhenAll(deptTask, positionTask);
        var dept = deptTask.Result.Res;
        var position = positionTask.Result.Res;

        return new ProBasicInfo
        {
            Code = row.Code,
            Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
            Nationality = row.Nationality,
            EmpDate = row.EmploymentDate.ToString("MMMM dd, yyyy"),
            Branch = dept?.NameAm ?? "",
            Department = dept?.Name ?? "",
            Position = position?.Name ?? "",
            EmpType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
            EmpNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
            WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
            BirthDate = row.BirthDate.ToString("MMMM dd, yyyy"),
            MaritalStatus = MyEnumHelper.FormatEnum<MaritalStatus>(row.MaritalStatus)
        };
    }
}

public class ProSalaryHandler(IDapperHelper dapper, ICorHrmmClient corHrmm) : IRequestHandler<ProSalaryQry, ProSalary?>
{
    public async Task<ProSalary?> Handle(ProSalaryQry request, CancellationToken ct)
    {
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<EmpSalary>(p, x => x.BaseSalary, x => x.Currency, x => x.SalaryPayFreq, x => x.EffectiveFrom, x => x.JgStepId)
            .From<EmpSalary>(p)
            .Where<EmpSalary>(p, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<ProSalary>(sql, parameters, ct);
        if (data is null) { return null; }

        var jgs = await corHrmm.GetSalaryJgs(data.JgStepId.ToString(), ct);
        data.Salary = $"{data.BaseSalary:#,##0.##} {data.Currency}";
        data.JgStep = jgs.Name ?? "";
        data.JobGrade = jgs.JobGrade ?? "";
        return data;
    }
}

public class ProAddressHandler(IDapperHelper dapper) : IRequestHandler<ProAddressQry, ProBasicAddress?>
{
    public async Task<ProBasicAddress?> Handle(ProAddressQry request, CancellationToken ct)
    {
        const string eb = "eb";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<EmpBio>(eb, x => x.AddressId)
            .Select<Address>(ad, x => x.AddressType, x => x.Country, x => x.Region, x => x.Subcity, x => x.Zone, x => x.Woreda, x => x.Kebele, x => x.HouseNo, x => x.Telephone, x => x.PoBox, x => x.Fax, x => x.Email, x => x.Website)
            .From<EmpBio>(eb)
            .Join<EmpBio, Address>(eb, ad, x => x.AddressId, x => x.Id)
            .Where<EmpBio>(eb, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<ProBasicAddress>(sql, parameters, ct);
        if (data is null) { return null; }

        data.AddressTypeStr = MyEnumHelper.FormatEnum<AddressType>(data.AddressType);
        return data;
    }
}

public class ProBioHandler(IDapperHelper dapper) : IRequestHandler<ProBioQry, ProBasicBio?>
{
    public async Task<ProBasicBio?> Handle(ProBioQry request, CancellationToken ct)
    {
        const string e = "e";
        const string eb = "eb";
        const string ef = "ef";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<EmpBio>(eb, x => x.BirthLocation, x => x.MotherFullName, x => x.HasBirthCert, x => x.HasMarriageCert)
            .Select<EmpFinance>(ef, x => x.Tin, x => x.BankAccountNo, x => x.PensionNumber)
            .From<Employee>(e)
            .Join<Employee, EmpBio>(e, eb, x => x.Id, x => x.EmployeeId)
            .LeftJoin<Employee, EmpFinance>(e, ef, x => x.Id, x => x.EmployeeId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<ProBasicBio>(sql, parameters, ct);
        if (data is null) { return null; }

        data.HasBirthCertStr = MyEnumHelper.FormatEnum<YesNo>(data.HasBirthCert);
        data.HasMarriageCertStr = MyEnumHelper.FormatEnum<YesNo>(data.HasMarriageCert);
        return data;
    }
}