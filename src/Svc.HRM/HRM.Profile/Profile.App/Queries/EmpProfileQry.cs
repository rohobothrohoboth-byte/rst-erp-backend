using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class ProInfoQry : IRequest<ProInfo?> { public Guid Id { get; set; } }
public class ProOverviewQry : IRequest<ProOverview?> { public Guid Id { get; set; } }
public class ProBasicQry : IRequest<ProBasic?> { public Guid Id { get; set; } }
public class ProBioQry : IRequest<ProBio?> { public Guid Id { get; set; } }
public class ProEmContactQry : IRequest<ProEmContact?> { public Guid Id { get; set; } }
public class ProFamilyQry : IRequest<ProFamily?> { public Guid Id { get; set; } }
public class EmpGuarantyQry : IRequest<EmpGuaranty?> { public Guid Id { get; set; } }



public class ProInfoHandler(IDapperHelper dapper, ICorHrmmClient corHrmm) : IRequestHandler<ProInfoQry, ProInfo?>
{
    public async Task<ProInfo?> Handle(ProInfoQry request, CancellationToken ct)
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

        return new ProInfo
        {
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}".Trim(),
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}".Trim(),
            EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
            Position = position?.Name ?? "",
        };
    }
}

public class ProOverviewHandler(IDapperHelper dapper) : IRequestHandler<ProOverviewQry, ProOverview?>
{
    public async Task<ProOverview?> Handle(ProOverviewQry request, CancellationToken ct)
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
        return new ProOverview
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

public class ProBasicHandler(IDapperHelper dapper, ICorHrmmClient corHrmm, ICorModClient corMod) : IRequestHandler<ProBasicQry, ProBasic?>
{
    public async Task<ProBasic?> Handle(ProBasicQry request, CancellationToken ct)
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
            .Select<Address>(ad, x => x.AddressType, x => x.Country, x => x.Region, x => x.Subcity, x => x.Zone, x => x.Woreda, x => x.Kebele, x => x.HouseNo, x => x.Telephone, x => x.PoBox, x => x.Fax, x => x.Email, x => x.Website)
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

        return new ProBasic
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

            AddressTypeStr = MyEnumHelper.FormatEnum<AddressType>(row.AddressType),
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

public class ProBioHandler(IDapperHelper dapper) : IRequestHandler<ProBioQry, ProBio?>
{
    public async Task<ProBio?> Handle(ProBioQry request, CancellationToken ct)
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
        var data = await dapper.QueryFirstOrDefaultAsync<ProBio>(sql, parameters, ct);
        if (data is null) { return null; }

        data.HasBirthCertStr = MyEnumHelper.FormatEnum<YesNo>(data.HasBirthCert);
        data.HasMarriageCertStr = MyEnumHelper.FormatEnum<YesNo>(data.HasMarriageCert);
        return data;
    }
}

public class ProEmContactHandler(IDapperHelper dapper) : IRequestHandler<ProEmContactQry, ProEmContact?>
{
    public async Task<ProEmContact?> Handle(ProEmContactQry request, CancellationToken ct)
    {
        const string ec = "ec";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<EmergencyContact>(ec, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality, x => x.Id, x => x.Relation)
            .Select<Address>(ad, x => x.AddressType, x => x.Country, x => x.Region, x => x.Subcity, x => x.Zone, x => x.Woreda, x => x.Kebele, x => x.HouseNo, x => x.Telephone, x => x.PoBox, x => x.Fax, x => x.Email, x => x.Website)
            .From<EmergencyContact>(ec)
            .Join<EmergencyContact, Address>(ec, ad, x => x.AddressId, x => x.Id)
            .Where<EmergencyContact>(ec, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<ProEmContactDto>(sql, parameters, ct);

        var vm = new ProEmContact { EmployeeId = request.Id };
        if (data is null)
        {
            vm.HasContact = false;
            return vm;
        }

        data.RelationStr = MyEnumHelper.FormatEnum<Relation>(data.Relation);
        data.AddressTypeStr = MyEnumHelper.FormatEnum<AddressType>(data.AddressType);

        vm.HasContact = true;
        vm.Contact = data;
        return vm;
    }
}

public class ProFamilyHandler(IDapperHelper dapper) : IRequestHandler<ProFamilyQry, ProFamily?>
{
    public async Task<ProFamily?> Handle(ProFamilyQry request, CancellationToken ct)
    {
        const string ef = "ef";
        var qb = new QueryBuilder()
            .Select<EmpFamily>(ef, x => x.Id, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality, x => x.Relation)
            .From<EmpFamily>(ef)
            .Where<EmpFamily>(ef, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        await using var reader = await dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<ProFamilyList>(ct);

        foreach (var data in list)
        {
            data.RelationStr = MyEnumHelper.FormatEnum<BranchType>(data.Relation);
            data.FullName = $"{data.FirstName} {data.MiddleName} {data.LastName}";
        }

        return new ProFamily
        {
            EmployeeId = request.Id,
            Family = list
        };
    }
}

public class EmpGuarantyHandler(IDapperHelper dapper) : IRequestHandler<EmpGuarantyQry, EmpGuaranty?>
{
    public async Task<EmpGuaranty?> Handle(EmpGuarantyQry request, CancellationToken ct)
    {
        const string eg = "eg";
        const string ad = "ad";
        const string egf = "egf";
        const string fm = "fm";
        var qb = new QueryBuilder()
            .Select<EmpGuarantor>(eg, x => x.Relation, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .SelectAs<FileMetaData, EmpGuaranty>(fm, x => x.Id, x => x.FileId)
            .From<EmpGuarantor>(eg)
            .Join<EmpGuarantor, Address>(eg, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<EmpGuarantor, EmpGuarantorFile>(eg, egf, x => x.Id, x => x.EmpGuarantorId)
            .LeftJoin<EmpGuarantorFile, FileMetaData>(egf, fm, x => x.FileMetaDataId, x => x.Id)
            .Where<EmpGuarantor>(eg, x => x.EmployeeId == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await dapper.QueryFirstOrDefaultAsync<EmpGuaranty>(sql, parameters, ct);
        if (data is null) { return null; }

        data.RelationStr = MyEnumHelper.FormatEnum<Relation>(data.Relation);
        data.AddressTypeStr = MyEnumHelper.FormatEnum<AddressType>(data.AddressType);
        data.FileSizeStr = data.FileSize > 0 ? SizeFormatter.FormatBytes(data.FileSize) : "0";
        return data;
    }
}