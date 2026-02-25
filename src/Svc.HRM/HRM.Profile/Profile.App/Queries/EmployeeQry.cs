using Common;
using Dapper;
using EthiopianCalendar;
using Helpers;
using MediatR;
using Npgsql;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using System.Data;
using System.Data.Common;

namespace Profile.App.Queries;

public class EmployeeAllQry : IRequest<List<EmployeeListDto>> { }
public class EmployeeByIdQry : IRequest<EmployeeListDto?> { public Guid Id { get; set; } }
public class Step5Qry : IRequest<Step5Dto?> { public Guid Id { get; set; } }
public class Step2Qry : IRequest<BasicInfoDto?> { public Guid Id { get; set; } }
public class EmpCodeByIdQry : IRequest<string?> { public Guid Id { get; set; } }



public class EmployeeAllQryHandler : IRequestHandler<EmployeeAllQry, List<EmployeeListDto>>
{
    private readonly DapperCxtHelper _db;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public EmployeeAllQryHandler(DapperCxtHelper db, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _db = db;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<List<EmployeeListDto>> Handle(EmployeeAllQry request, CancellationToken ct)
    {
        using var conn = await _db.GetOpenConnectionAsync(ct: ct);
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
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.RowVersion)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id);
        qb.OrderBy<Employee>(e, x => x.DateAdd, desc: true)
          .OrderBy<Employee>(e, x => x.Id, desc: true);

        var (sql, parameters) = qb.Build();
        var result = new List<EmployeeListDto>(10000);
        await using var reader = (DbDataReader)await conn.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct), CommandBehavior.SequentialAccess);
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
                RowVersion = Convert.ToBase64String(row.RowVersion)
            });
        }

        await _db.DisposeAsync();
        return result;
    }
}

public class EmployeeByIdQryHandler : IRequestHandler<EmployeeByIdQry, EmployeeListDto?>
{
    private readonly DapperCxtHelper _db;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public EmployeeByIdQryHandler(DapperCxtHelper db, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _db = db;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<EmployeeListDto?> Handle(EmployeeByIdQry request, CancellationToken ct)
    {
        using var conn = await _db.GetOpenConnectionAsync(ct: ct);
        const string e = "e";
        const string p = "p";
        const string ph = "ph";
        const string th = "th";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.RowVersion)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .SelectAs<EmpPhotoThumbnail>(th, asName: "PhotoThumbnail", x => x.Data)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpPhoto>(e, ph, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpPhoto, EmpPhotoThumbnail>(ph, th, x => x.ThumbnailId, x => x.FileMetaDataId)
            .Where<Employee>(e, x => x.Id, "=", request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        EmpJoinRow? row = null;

        await using var reader = (DbDataReader)await conn.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct), CommandBehavior.SequentialAccess);
        var parser = reader.GetRowParser<EmpJoinRow>();
        if (await reader.ReadAsync(ct)) { row = parser(reader); }
        await _db.DisposeAsync();
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
            RowVersion = Convert.ToBase64String(row.RowVersion)
        };

        return dto;
    }
}

public class Step5QryHandler : IRequestHandler<Step5Qry, Step5Dto?>
{
    private readonly DapperCxtHelper _db;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public Step5QryHandler(DapperCxtHelper db, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _db = db;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    private async Task<EmpBioJoin?> GetBio(Guid id, CancellationToken ct)
    {
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync(ct);
        const string e = "e";
        const string ef = "ef";
        const string eb = "eb";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<EmpBio>(eb, x => x.BirthDate, x => x.BirthLocation, x => x.MotherFullName, x => x.HasBirthCert, x => x.HasMarriageCert, x => x.MaritalStatus)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .Select<EmpFinance>(ef, x => x.Tin, x => x.BankAccountNo, x => x.PensionNumber)
            .From<Employee>(e)
            .LeftJoin<Employee, EmpBio>(e, eb, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpBio, Address>(eb, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<Employee, EmpFinance>(e, ef, x => x.Id, x => x.EmployeeId)
            .Where<Employee>(e, x => x.Id, "=", id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await conn.QueryFirstOrDefaultAsync<EmpBioJoin>(sql, parameters);
        return row;
    }

    private async Task<EmpContJoin?> GetCon(Guid id, CancellationToken ct)
    {
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync(ct);
        const string e = "e";
        const string p = "p";
        const string ec = "ec";
        const string ad = "ad";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<EmergencyContact>(ec, x => x.RelationId)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender, x => x.Nationality)
            .From<Employee>(e)
            .Join<Employee, EmergencyContact>(e, ec, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmergencyContact, Address>(ec, ad, x => x.AddressId, x => x.Id)
            .Join<EmergencyContact, Person>(ec, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.Id, "=", id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await conn.QueryFirstOrDefaultAsync<EmpContJoin>(sql, parameters);
        return row;
    }

    private async Task<EmpGuaJoin?> GetGra(Guid id, CancellationToken ct)
    {
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync(ct);
        const string e = "e";
        const string p = "p";
        const string eg = "eg";
        const string ad = "ad";
        const string egf = "egf";
        const string fm = "fm";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<EmpGuarantor>(eg, x => x.RelationId)
            .Select<Address>(ad, x => x.AddressType, x => x.Zone, x => x.Region, x => x.Subcity, x => x.Woreda, x => x.Kebele, x => x.Telephone)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender, x => x.Nationality)
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType, x => x.FileSize)
            .From<Employee>(e)
            .Join<Employee, EmpGuarantor>(e, eg, x => x.Id, x => x.EmployeeId)
            .Join<EmpGuarantor, Person>(eg, p, x => x.PersonId, x => x.Id)
            .LeftJoin<EmpGuarantor, Address>(eg, ad, x => x.AddressId, x => x.Id)
            .LeftJoin<EmpGuarantor, EmpGuarantorFile>(eg, egf, x => x.Id, x => x.EmpGuarantorId)
            .LeftJoin<EmpGuarantorFile, FileMetaData>(egf, fm, x => x.FileMetaDataId, x => x.Id)
            .Where<Employee>(e, x => x.Id, "=", id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await conn.QueryFirstOrDefaultAsync<EmpGuaJoin>(sql, parameters);
        return row;
    }


    public async Task<Step5Dto?> Handle(Step5Qry request, CancellationToken ct)
    {
        await using var conn = new NpgsqlConnection(_db.ConnectionString);
        await conn.OpenAsync(ct);
        const string e = "e";
        const string p = "p";
        const string ph = "ph";
        const string th = "th";

        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmploymentType, x => x.EmploymentNature, x => x.WorkArrangement, x => x.EmploymentDate, x => x.DepartmentId, x => x.JobGradeId, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender, x => x.Nationality)
            .SelectAs<EmpPhotoThumbnail>(th, asName: "PhotoThumbnail", x => x.Data)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .LeftJoin<Employee, EmpPhoto>(e, ph, x => x.Id, x => x.EmployeeId)
            .LeftJoin<EmpPhoto, EmpPhotoThumbnail>(ph, th, x => x.ThumbnailId, x => x.FileMetaDataId)
            .Where<Employee>(e, x => x.Id, "=", request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        EmpJoinRow? row = null;
        await using var reader = (DbDataReader)await conn.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct), CommandBehavior.SequentialAccess);
        var parser = reader.GetRowParser<EmpJoinRow>();
        if (await reader.ReadAsync(ct)) { row = parser(reader); }
        if (row == null) return null;

        var eBioTask = GetBio(request.Id, ct);
        var eConTask = GetCon(request.Id, ct);
        var eGarTask = GetGra(request.Id, ct);
        var deptTask = _corMod.GetDept(row.DepartmentId.ToString(), ct);
        var jobGradeTask = _corHRMM.GetJobGrade(row.JobGradeId.ToString(), ct);
        var positionTask = _corHRMM.GetPosition(row.PositionId.ToString(), ct);

        await Task.WhenAll(eBioTask, eConTask, eGarTask, deptTask, jobGradeTask, positionTask);
        var eBio = eBioTask.Result;
        var eCon = eConTask.Result;
        var eGar = eGarTask.Result;

        var dto = new Step5Dto
        {
            EmployeeId = row.Id,
            Code = row.Code,
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}",
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}",
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
            WorkArr = MyEnumHelper.FormatEnum<WorkArrangement>(row.WorkArrangement),
            Photo = row.PhotoThumbnail != null ? Convert.ToBase64String(row.PhotoThumbnail) : "",

            BirthDate = eBio != null ? $"{eBio.BirthDate:MMMM dd, yyyy}" : "",
            BirthDateAm = eBio?.BirthDate.HasValue == true ? eBio.BirthDate.Value.ToEthiopianDateString("MMMM dd, yyyy") : "",
            BirthLocation = eBio?.BirthLocation ?? "",
            MotherFullName = eBio?.MotherFullName ?? "",
            HasBirthCert = eBio != null ? MyEnumHelper.FormatEnum<YesNo>(eBio.HasBirthCert) : "",
            HasMarriageCert = eBio != null ? MyEnumHelper.FormatEnum<YesNo>(eBio.HasMarriageCert) : "",
            MaritalStatus = eBio != null ? MyEnumHelper.FormatEnum<MaritalStat>(eBio.MaritalStatus) : "",
            Address = eBio != null ? $"{MyEnumHelper.FormatEnum<AddressType>(eBio.AddressType)}: {eBio.Region} | {eBio.Zone}({eBio.Subcity}) | {eBio.Woreda} | {eBio.Kebele}" : "",
            Telephone = eBio?.Telephone ?? "",
            Tin = eBio?.Tin ?? "",
            BankAccountNo = eBio?.BankAccountNo ?? "",
            PensionNumber = eBio?.PensionNumber ?? "",

            ConFullName = eCon != null ? $"{eCon.FirstName} {eCon.MiddleName} {eCon.LastName}" : "",
            ConFullNameAm = eCon != null ? $"{eCon.FirstNameAm} {eCon.MiddleNameAm} {eCon.LastNameAm}" : "",
            ConNationality = eCon?.Nationality ?? "",
            ConGender = eCon != null ? MyEnumHelper.FormatEnum<Gender>(eCon.Gender) : "",
            ConRelation = "Convert to enum",
            ConAddress = eCon != null ? $"{MyEnumHelper.FormatEnum<AddressType>(eCon.AddressType)}: {eCon.Region} | {eCon.Zone}({eCon.Subcity}) | {eCon.Woreda} | {eCon.Kebele}" : "",
            ConTelephone = eCon?.Telephone ?? "",

            GuaFullName = eGar != null ? $"{eGar.FirstName} {eGar.MiddleName} {eGar.LastName}" : "",
            GuaFullNameAm = eGar != null ? $"{eGar.FirstNameAm} {eGar.MiddleNameAm} {eGar.LastNameAm}" : "",
            GuaNationality = eGar?.Nationality ?? "",
            GuaGender = eGar != null ? MyEnumHelper.FormatEnum<Gender>(eGar.Gender) : "",
            GuaRelation = "Convert to enum",
            GuaAddress = eGar != null ? $"{MyEnumHelper.FormatEnum<AddressType>(eGar.AddressType)}: {eGar.Region} | {eGar.Zone}({eGar.Subcity}) | {eGar.Woreda} | {eGar.Kebele}" : "",
            GuaTelephone = eGar?.Telephone ?? "",
            GuaFileName = eGar?.FileName ?? "",
            GuaFileType = eGar?.ContentType ?? "",
            GuaFileSize = eGar != null ? SizeFormatter.FormatBytes(eGar.FileSize) : ""
        };

        return dto;
    }
}

public class Step2QryHandler : IRequestHandler<Step2Qry, BasicInfoDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public Step2QryHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<BasicInfoDto?> Handle(Step2Qry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var dept = await _corMod.GetDept(data.DepartmentId.ToString(), cancellationToken);
        var jg = await _corHRMM.GetJobGrade(data.JobGradeId.ToString(), cancellationToken);
        var pos = await _corHRMM.GetPosition(data.PositionId.ToString(), cancellationToken);
        var ePhoto = await _unitOfWork.Repository<EmpPhoto>().GetFoD(b => b.EmployeeId == request.Id);
        var photo = "";

        if (ePhoto != null)
        {
            var ePhotoB = await _unitOfWork.Repository<EmpPhotoBlob>().GetFoD(t => t.FileMetaDataId == ePhoto.FileMetaDataId);
            photo = Convert.ToBase64String(ePhotoB!.Data);
        }

        var c = new BasicInfoDto
        {
            EmployeeId = data.Id,
            Photo = photo,
            FullName = $"{per!.FirstName} {per.MiddleName} {per.LastName}",
            FullNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            Code = data.Code,
            Gender = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Nationality = per.Nationality,
            EmploymentDate = $"{data.EmploymentDate:MMMM dd, yyyy}",
            EmploymentDateAm = data.EmploymentDate.ToEthiopianDateString("MMMM dd, yyyy"),
            JobGrade = jg.Res.Name != null ? jg.Res.Name : "NOT AVAILABLE",
            Position = pos.Res != null ? pos.Res.Name : "NOT AVAILABLE",
            Department = dept.Res.Name != null ? dept.Res.Name : "NOT AVAILABLE",
            Branch = dept.Res.NameAm != null ? dept.Res.NameAm : "NOT AVAILABLE",
            EmploymentType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
            EmploymentNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName(),
            WorkArrangement = ((WorkArrangement)Enum.Parse(typeof(WorkArrangement), data.WorkArrangement)).ToDisplayName()
        };

        return c;
    }
}

public class EmpCodeByIdQryHandler : IRequestHandler<EmpCodeByIdQry, string?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpCodeByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<string?> Handle(EmpCodeByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        return data.Code;
    }
}