using Common;
using Dapper;
using EthiopianCalendar;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpAllAdminQry : IRequest<List<EmployeeListDto>> { }
public class EmployeeAllQry : IRequest<List<EmployeeListDto>> { }
public class EmployeeByIdQry : IRequest<EmployeeListDto?> { public Guid Id { get; set; } }
public class EmpAddPrintQry : IRequest<EmpAddPrintDto?> { public Guid Id { get; set; } }
public class Step2Qry : IRequest<BasicInfoDto?> { public Guid Id { get; set; } }
public class EmpCodeByIdQry : IRequest<string?> { public Guid Id { get; set; } }



public class EmpAllAdminHandler : IRequestHandler<EmpAllAdminQry, List<EmployeeListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public EmpAllAdminHandler(IDapperHelper dapper, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<List<EmployeeListDto>> Handle(EmpAllAdminQry request, CancellationToken ct)
    {
        var deptTask = _corMod.GetListDept(ct);
        var posTask = _corHRMM.GetListPosition(ct);

        await Task.WhenAll(deptTask, posTask);
        var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmpState, x => x.DepartmentId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .OrderBy<Employee>(e, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var result = new List<EmployeeListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<EmpJoinRow>();

        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);
            deptDict.TryGetValue(row.DepartmentId, out var dept);
            posDict.TryGetValue(row.PositionId, out var pos);

            result.Add(new EmployeeListDto
            {
                Id = row.Id,
                Code = row.Code,
                EmpFullName = $"{row.FirstName} {row.MiddleName} {row.LastName}",
                EmpFullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}",
                EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
                Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
                Branch = dept?.NameAm ?? "",
                Department = dept?.Name ?? "",
                Position = pos?.Name ?? "",
                EmpType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
                EmpNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
                WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
                IsDeleted = false,
                DateAdd = row.DateAdd,
                DateMod = row.DateMod,
                RowVersion = row.xmin.ToString()
            });
        }

        return result;
    }
}

public class EmployeeAllHandler : IRequestHandler<EmployeeAllQry, List<EmployeeListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public EmployeeAllHandler(IDapperHelper dapper, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<List<EmployeeListDto>> Handle(EmployeeAllQry request, CancellationToken ct)
    {
        var deptTask = _corMod.GetListDept(ct);
        var jgTask = _corHRMM.GetListJobGrade(ct);
        var posTask = _corHRMM.GetListPosition(ct);

        await Task.WhenAll(deptTask, jgTask, posTask);
        var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var jobGradeDict = jgTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmpState, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .OrderBy<Employee>(e, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var result = new List<EmployeeListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<EmpJoinRow>();

        while (await reader.ReadAsync(ct))
        {
            var row = parser(reader);
            deptDict.TryGetValue(row.DepartmentId, out var dept);
            jobGradeDict.TryGetValue(row.JobGradeId, out var jg);
            posDict.TryGetValue(row.PositionId, out var pos);

            result.Add(new EmployeeListDto
            {
                Id = row.Id,
                Code = row.Code,
                EmpFullName = $"{row.FirstName} {row.MiddleName} {row.LastName}",
                EmpFullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}",
                EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
                Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
                Branch = dept?.NameAm ?? "",
                Department = dept?.Name ?? "",
                Position = pos?.Name ?? "",
                JobGrade = jg?.Name ?? "",
                EmpType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
                EmpNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
                WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
                IsDeleted = false,
                DateAdd = row.DateAdd,
                DateMod = row.DateMod,
                RowVersion = row.xmin.ToString()
            });
        }

        return result;
    }
}

public class EmployeeByIdHandler : IRequestHandler<EmployeeByIdQry, EmployeeListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public EmployeeByIdHandler(IDapperHelper dapper, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<EmployeeListDto?> Handle(EmployeeByIdQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string ph = "ph";
        const string th = "th";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmpState, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .SelectAs<EmpPhotoThumbnail, EmpJoinRow>(th, x => x.Data, x => x.PhotoThumbnail)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpPhoto>(e, ph, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpPhoto, EmpPhotoThumbnail>(ph, th, x => x.ThumbnailId, x => x.FileMetaDataId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<EmpJoinRow>(sql, parameters, ct);
        if (row == null) return null;

        var deptTask = _corMod.GetDept(row.DepartmentId.ToString(), ct);
        var jobGradeTask = _corHRMM.GetJobGrade(row.JobGradeId.ToString(), ct);
        var positionTask = _corHRMM.GetPosition(row.PositionId.ToString(), ct);
        await Task.WhenAll(deptTask, jobGradeTask, positionTask);

        var dto = new EmployeeListDto
        {
            Id = row.Id,
            EmpFullName = $"{row.FirstName} {row.MiddleName} {row.LastName}",
            EmpFullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}",
            Code = row.Code,
            EmpState = MyEnumHelper.FormatEnum<EmpState>(row.EmpState),
            Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
            Branch = deptTask.Result?.Res?.NameAm ?? "",
            Department = deptTask.Result?.Res?.Name ?? "",
            Position = positionTask.Result?.Res?.Name ?? "",
            JobGrade = jobGradeTask.Result?.Res?.Name ?? "",
            EmpType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
            EmpNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
            WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
            Photo = row.PhotoThumbnail != null ? Convert.ToBase64String(row.PhotoThumbnail) : "",
            IsDeleted = false,
            DateAdd = row.DateAdd,
            DateMod = row.DateMod,
            RowVersion = row.xmin.ToString()
        };

        return dto;
    }
}

public class EmpAddPrintHandler : IRequestHandler<EmpAddPrintQry, EmpAddPrintDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public EmpAddPrintHandler(IDapperHelper dapper, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    private static string BuildAddress(string type, string? region, string? zone, string? subcity, string? woreda, string? kebele)
    {
        if (string.IsNullOrWhiteSpace(region)) return "";
        return $"{MyEnumHelper.FormatEnum<AddressType>(type)}: {region} | {zone}({subcity}) | {woreda} | {kebele}";
    }

    private async Task<EmpBioJoin?> GetBio(Guid id, CancellationToken ct)
    {
        const string e = "e";
        const string eb = "eb";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<EmpBio>(eb, x => x.BirthDate, x => x.MaritalStatus)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .From<Employee>(e)
            .LeftJoin<Employee, EmpBio>(e, eb, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpBio, Address>(eb, ad, x => x.AddressId, x => x.Id)
            .Where<Employee>(e, x => x.Id == id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<EmpBioJoin>(sql, parameters, ct);
        return row;
    }

    private async Task<EmpGuaJoin?> GetGra(Guid id, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string eg = "eg";
        const string ad = "ad";
        const string egf = "egf";
        const string fm = "fm";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<EmpGuarantor>(eg, x => x.Relation)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender, x => x.Nationality)
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .From<Employee>(e)
            .Join<Employee, EmpGuarantor>(e, eg, x => x.Id, x => x.EmployeeId)
            .Join<EmpGuarantor, Person>(eg, p, x => x.PersonId, x => x.Id)
            .LeftJoin<EmpGuarantor, Address>(eg, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<EmpGuarantor, EmpGuarantorFile>(eg, egf, x => x.Id, x => x.EmpGuarantorId)
            .LeftJoin<EmpGuarantorFile, FileMetaData>(egf, fm, x => x.FileMetaDataId, x => x.Id)
            .Where<Employee>(e, x => x.Id == id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<EmpGuaJoin>(sql, parameters, ct);
        return row;
    }

    public async Task<EmpAddPrintDto?> Handle(EmpAddPrintQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string ph = "ph";
        const string th = "th";

        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.EmploymentDate, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender, x => x.Nationality)
            .SelectAs<EmpPhotoThumbnail, EmpJoinRow>(th, x => x.Data, x => x.PhotoThumbnail)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpPhoto>(e, ph, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpPhoto, EmpPhotoThumbnail>(ph, th, x => x.ThumbnailId, x => x.FileMetaDataId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<EmpJoinRow>(sql, parameters, ct);

        if (row is null) { return null; }

        var bioTask = await GetBio(request.Id, ct);
        var garTask = await GetGra(request.Id, ct);
        var deptTask = _corMod.GetDept(row.DepartmentId.ToString(), ct);
        var jobGradeTask = _corHRMM.GetJobGrade(row.JobGradeId.ToString(), ct);
        var positionTask = _corHRMM.GetPosition(row.PositionId.ToString(), ct);
        await Task.WhenAll(deptTask, jobGradeTask, positionTask);
        var eBio = bioTask ?? new EmpBioJoin();
        var eGar = garTask ?? new EmpGuaJoin();
        var dept = deptTask.Result?.Res;
        var jobGrade = jobGradeTask.Result?.Res;
        var position = positionTask.Result?.Res;

        return new EmpAddPrintDto
        {
            EmployeeId = row.Id,
            Code = row.Code,
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}".Trim(),
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}".Trim(),
            Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
            Nationality = row.Nationality,
            EmploymentDate = row.EmploymentDate.ToString("MMMM dd, yyyy"),
            EmploymentDateAm = row.EmploymentDate.ToEthiopianDateString("MMMM dd, yyyy"),
            Branch = dept?.NameAm ?? "",
            Department = dept?.Name ?? "",
            Position = position?.Name ?? "",
            JobGrade = jobGrade?.Name ?? "",
            EmploymentType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
            EmploymentNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
            WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
            Photo = row.PhotoThumbnail is not null ? Convert.ToBase64String(row.PhotoThumbnail) : "",
            // BIO
            BirthDate = eBio.BirthDate?.ToString("MMMM dd, yyyy") ?? "",
            BirthDateAm = eBio.BirthDate?.ToEthiopianDateString("MMMM dd, yyyy") ?? "",
            Address = BuildAddress(eBio.AddressType, eBio.Region, eBio.Zone, eBio.Subcity, eBio.Woreda, eBio.Kebele),
            Telephone = eBio.Telephone ?? "",
            // GUARANTOR
            GuaFullName = $"{eGar.FirstName} {eGar.MiddleName} {eGar.LastName}".Trim(),
            GuaNationality = eGar.Nationality ?? "",
            GuaGender = MyEnumHelper.FormatEnum<Gender>(eGar.Gender),
            GuaRelation = MyEnumHelper.FormatEnum<Relation>(eGar.Relation),
            GuaAddress = BuildAddress(eGar.AddressType, eGar.Region, eGar.Zone, eGar.Subcity, eGar.Woreda, eGar.Kebele),
            GuaTelephone = eGar.Telephone ?? "",
            GuaFileName = eGar.FileName ?? "",
            GuaFileType = eGar.ContentType ?? "",
            GuaFileSize = eGar.FileSize > 0 ? SizeFormatter.FormatBytes(eGar.FileSize) : ""
        };
    }
}

public class Step2Handler : IRequestHandler<Step2Qry, BasicInfoDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public Step2Handler(IDapperHelper dapper, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<BasicInfoDto?> Handle(Step2Qry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        const string ph = "ph";
        const string th = "th";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.EmploymentDate, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender, x => x.Nationality)
            .SelectAs<EmpPhotoThumbnail, EmpJoinRow>(th, x => x.Data, x => x.PhotoThumbnail)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpPhoto>(e, ph, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpPhoto, EmpPhotoThumbnail>(ph, th, x => x.ThumbnailId, x => x.FileMetaDataId)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<EmpJoinRow>(sql, parameters, ct);
        if (row == null) return null;

        var deptTask = _corMod.GetDept(row.DepartmentId.ToString(), ct);
        var jobGradeTask = _corHRMM.GetJobGrade(row.JobGradeId.ToString(), ct);
        var positionTask = _corHRMM.GetPosition(row.PositionId.ToString(), ct);
        await Task.WhenAll(deptTask, jobGradeTask, positionTask);

        var c = new BasicInfoDto
        {
            EmployeeId = row.Id,
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}",
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}",
            Code = row.Code,
            Gender = MyEnumHelper.FormatEnum<Gender>(row.Gender),
            Nationality = row.Nationality,
            EmploymentDate = $"{row.EmploymentDate:MMMM dd, yyyy}",
            EmploymentDateAm = row.EmploymentDate.ToEthiopianDateString("MMMM dd, yyyy"),
            Branch = deptTask.Result?.Res?.NameAm ?? "",
            Department = deptTask.Result?.Res?.Name ?? "",
            Position = positionTask.Result?.Res?.Name ?? "",
            JobGrade = jobGradeTask.Result?.Res?.Name ?? "",
            EmploymentType = MyEnumHelper.FormatEnum<EmpType>(row.EmploymentType),
            EmploymentNature = MyEnumHelper.FormatEnum<EmpNature>(row.EmploymentNature),
            WorkArrangement = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
            Photo = row.PhotoThumbnail != null ? Convert.ToBase64String(row.PhotoThumbnail) : ""
        };

        return c;
    }
}

public class EmpCodeByIdHandler : IRequestHandler<EmpCodeByIdQry, string?>
{
    private readonly IDapperHelper _dapper;
    public EmpCodeByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<string?> Handle(EmpCodeByIdQry request, CancellationToken ct)
    {
        const string e = "e";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Code)
            .From<Employee>(e)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<EmpCodeJoin>(sql, parameters, ct);
        if (row == null) return null;
        return row!.Code;
    }
}