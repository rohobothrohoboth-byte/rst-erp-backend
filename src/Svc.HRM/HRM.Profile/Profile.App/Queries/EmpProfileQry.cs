using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class MyProInfoQry : IRequest<MyProInfo?> { public Guid Id { get; set; } }
public class MyProOverviewQry : IRequest<MyProOverview?> { public Guid Id { get; set; } }
public class MyProBasicQry : IRequest<MyProBasic?> { public Guid Id { get; set; } }
public class MyProBioQry : IRequest<MyProBio?> { public Guid Id { get; set; } }
public class MyProEmContQry : IRequest<MyProContact?> { public Guid Id { get; set; } }
public class MyProFamilyQry : IRequest<MyProFamily?> { public Guid Id { get; set; } }
public class MyEmpGuarQry : IRequest<MyEmpGuar?> { public Guid Id { get; set; } }



public class ProInfoHandler(IDapperHelper dapper, ICorHrmmClient corHrmm) : IRequestHandler<MyProInfoQry, MyProInfo?>
{
    public async Task<MyProInfo?> Handle(MyProInfoQry request, CancellationToken ct)
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
        var row = await dapper.QueryFirstOrDefaultAsync<ProInfoJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var positionTask = await corHrmm.GetPosition(row.PositionId.ToString(), ct);
        var position = positionTask.Res;

        return new MyProInfo
        {
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}".Trim(),
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}".Trim(),
            EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
            Position = position?.Name ?? "",
        };
    }
}

public class ProOverviewHandler(IDapperHelper dapper) : IRequestHandler<MyProOverviewQry, MyProOverview?>
{
    public async Task<MyProOverview?> Handle(MyProOverviewQry request, CancellationToken ct)
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
        var row = await dapper.QueryFirstOrDefaultAsync<ProOverviewJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var serStr = row.EmploymentDate.FullServDur();
        return new MyProOverview
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

public class ProBasicHandler(IDapperHelper dapper, ICorHrmmClient corHrmm, ICorModClient corMod) : IRequestHandler<MyProBasicQry, MyProBasic?>
{
    public async Task<MyProBasic?> Handle(MyProBasicQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string eb = "eb";
        const string es = "es";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.EmploymentDate, x => x.DepartmentId, x => x.PositionId)
            .Select<Person>(p, x => x.Gender, x => x.Nationality)
            .Select<EmpBio>(eb, x => x.BirthDate, x => x.MaritalStatus, x => x.AddressId)
            .Select<EmpSalary>(es, x => x.BaseSalary, x => x.Currency, x => x.SalaryPayFreq, x => x.EffectiveFrom, x => x.JgStepId)
            .Select<Address>(ad,  x => x.AddressType! , x => x.Country!, x => x.Region!, x => x.Subcity!, x => x.Zone!, x => x.Woreda!, x => x.Kebele!, x => x.HouseNo!, x => x.Telephone!, x => x.PoBox!, x => x.Fax!, x => x.Email!, x => x.Website!)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpBio>(e, eb, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpBio, Address>(eb, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<Employee, EmpSalary>(e, es, x => x.Id, x => x.EmployeeId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await dapper.QueryFirstOrDefaultAsync<ProBasicJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var deptTask = corMod.GetDept(row.DepartmentId.ToString(), ct);
        var jgsTask = corHrmm.GetSalaryJgs(row.JgStepId.ToString(), ct);
        await Task.WhenAll(deptTask, jgsTask);
        var dept = deptTask.Result.Res;
        var jgs = jgsTask.Result;

        return new MyProBasic
        {
            Code = row.Code,
            Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
            Nationality = row.Nationality,
            BirthDate = row.BirthDate.ToString("MMMM dd, yyyy"),
            MaritalStatus = MyEnumHelper.FormatEnum<MaritalStatus>(row.MaritalStatus),

            EmpDate = row.EmploymentDate.ToString("MMMM dd, yyyy"),
            Branch = dept?.NameAm ?? "",
            Department = dept?.Name ?? "",
            EmpType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
            EmpNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
            WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),

            Salary = $"{row.BaseSalary:#,##0.##} {row.Currency}",
            Currency = row.Currency,
            SalaryPayFreq = row.SalaryPayFreq,
            JgStep = jgs.Name ?? "",
            JobGrade = jgs.JobGrade ?? "",
            EffectiveFrom = row.EffectiveFrom,

            AddressType = MyEnumHelper.FormatEnum<AddressType>(row.AddressType),
            Country = row.Country,
            Region = row.Region,
            Subcity = row.Subcity,
            Zone = row.Zone,
            Woreda = row.Woreda,
            Kebele = row.Kebele,
            HouseNo = row.HouseNo,
            Telephone = row.Telephone,
            PoBox = row.PoBox,
            Fax = row.Fax,
            Email = row.Email,
            Website = row.Website
        };
    }
}

public class ProBioHandler(IDapperHelper dapper, IEmpCertService _iEmpCertSer) : IRequestHandler<MyProBioQry, MyProBio?>
{
    public async Task<MyProBio?> Handle(MyProBioQry request, CancellationToken ct)
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
        var data = await dapper.QueryFirstOrDefaultAsync<MyProBio>(sql, parameters, ct);
        if (data is null) { return null; }

        var cert = await _iEmpCertSer.GetCerts(request.Id, ct);
        data.BiCertId = cert.BiCertId;
        data.BiCertName = cert.BiCertName;
        data.BiCertType = cert.BiCertType;
        data.BiCertSize = cert.BiCertSize > 0 ? SizeFormatter.FormatBytes(cert.BiCertSize) : "0";
        data.MaCertId = cert.MaCertId;
        data.MaCertName = cert.MaCertName;
        data.MaCertType = cert.MaCertType;
        data.MaCertSize = cert.MaCertSize > 0 ? SizeFormatter.FormatBytes(cert.MaCertSize) : "0";
        data.HasBirthCert = MyEnumHelper.FormatEnum<YesNo>(data.HasBirthCert);
        data.HasMarriageCert = MyEnumHelper.FormatEnum<YesNo>(data.HasMarriageCert);
        return data;
    }
}

public class ProEmContactHandler(IDapperHelper dapper) : IRequestHandler<MyProEmContQry, MyProContact?>
{
    public async Task<MyProContact?> Handle(MyProEmContQry request, CancellationToken ct)
    {
        const string ec = "ec";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<EmergencyContact>(ec, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality, x => x.Id, x => x.Relation)
            .Select<Address>(ad, x => x.AddressType!, x => x.Country!, x => x.Region!, x => x.Subcity!, x => x.Zone!, x => x.Woreda!, x => x.Kebele!, x => x.HouseNo!, x => x.Telephone!, x => x.PoBox!, x => x.Fax!, x => x.Email!, x => x.Website!)
            .From<EmergencyContact>(ec)
            .Join<EmergencyContact, Address>(ec, ad, x => x.AddressId, x => x.Id)
            .Where<EmergencyContact>(ec, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<MyProContList>(sql, parameters, ct);

        var vm = new MyProContact { EmployeeId = request.Id };
        if (data is null)
        {
            vm.HasContact = false;
            return vm;
        }

        data.Gender = MyEnumHelper.FormatEnum<Gender>(data.Gender);
        data.Relation = MyEnumHelper.FormatEnum<Relation>(data.Relation);
        data.AddressType = MyEnumHelper.FormatEnum<AddressType>(data.AddressType);
        vm.HasContact = true;
        vm.Contact = data;
        return vm;
    }
}

public class ProFamilyHandler(IDapperHelper dapper) : IRequestHandler<MyProFamilyQry, MyProFamily?>
{
    public async Task<MyProFamily?> Handle(MyProFamilyQry request, CancellationToken ct)
    {
        const string ef = "ef";
        var qb = new QueryBuilder()
            .Select<EmpFamily>(ef, x => x.Id, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality, x => x.Relation)
            .From<EmpFamily>(ef)
            .Where<EmpFamily>(ef, x => x.EmployeeId == request.Id);
        var (sql, parameters) = qb.Build();
        await using var reader = await dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<MyProFamilyList>(ct);

        foreach (var data in list)
        {
            data.Relation = MyEnumHelper.FormatEnum<Relation>(data.Relation);
            data.Gender = MyEnumHelper.FormatEnum<Gender>(data.Gender);
            data.FullName = $"{data.FirstName} {data.MiddleName} {data.LastName}";
        }

        return new MyProFamily
        {
            EmployeeId = request.Id,
            Family = list
        };
    }
}

public class EmpGuarantyHandler(IDapperHelper dapper) : IRequestHandler<MyEmpGuarQry, MyEmpGuar?>
{
    public async Task<MyEmpGuar?> Handle(MyEmpGuarQry request, CancellationToken ct)
    {
        const string eg = "eg";
        const string ad = "ad";
        const string egf = "egf";
        const string fm = "fm";
        var qb = new QueryBuilder()
            .Select<EmpGuarantor>(eg, x => x.Relation, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality)
            .Select<Address>(ad, x => x.AddressType!, x => x.Zone!, x => x.Region!, x => x.Subcity!, x => x.Woreda!, x => x.Kebele!, x => x.Telephone!)
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .SelectAs<FileMetaData, MyEmpGuar>(fm, x => x.Id, x => x.FileId)
            .From<EmpGuarantor>(eg)
            .Join<EmpGuarantor, Address>(eg, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<EmpGuarantor, EmpGuarantorFile>(eg, egf, x => x.Id, x => x.EmpGuarantorId)
            .LeftJoin<EmpGuarantorFile, FileMetaData>(egf, fm, x => x.FileMetaDataId, x => x.Id)
            .Where<EmpGuarantor>(eg, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<MyEmpGuar>(sql, parameters, ct);
        if (data is null) { return null; }

        data.Gender = MyEnumHelper.FormatEnum<Gender>(data.Gender);
        data.Relation = MyEnumHelper.FormatEnum<Relation>(data.Relation);
        data.AddressType = MyEnumHelper.FormatEnum<AddressType>(data.AddressType);
        data.FileSizeStr = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "0";
        return data;
    }
}