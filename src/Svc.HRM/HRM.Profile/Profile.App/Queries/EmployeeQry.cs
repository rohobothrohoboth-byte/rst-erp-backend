using Common;
using Dapper;
using EthiopianCalendar;
using Helpers;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        var conn = await _db.GetOpenConnectionAsync(ct: ct);
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

        //// Dynamic search
        //if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        //{
        //    qb.SearchILike<Person>(p, request.SearchTerm,
        //        x => x.FirstName, x => x.LastName,
        //        x => x.FirstNameAm, x => x.LastNameAm);
        //}

        //// Keyset pagination
        //if (request.LastDate.HasValue && request.LastId.HasValue)
        //{
        //    qb.KeysetAfter<Employee>(e, new[]
        //    {
        //        (x => x.DateAdd, request.LastDate.Value),
        //        (x => x.Id, request.LastId.Value)
        //    }, desc: true);
        //}

        //// Order & limit
        //qb.OrderBy<Employee>(e, x => x.DateAdd, desc: true)
        //  .OrderBy<Employee>(e, x => x.Id, desc: true)
        //  .Limit(request.PageSize);

        var (sql, parameters) = qb.Build();
        var result = new List<EmployeeListDto>(10000);
        using var reader = (DbDataReader)await conn.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct), CommandBehavior.SequentialAccess);
        var parser = reader.GetRowParser<EmployeeJoinRow>();

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
                Gender = MyEnumHelper.TryParseEnum<Gender>(row.Gender)?.ToDisplayName() ?? "",
                Branch = dept?.NameAm ?? "",
                Department = dept?.Name ?? "",
                Position = pos?.Name ?? "",
                JobGrade = jg?.Name ?? "",
                EmpType = MyEnumHelper.TryParseEnum<EmpType>(row.EmploymentType)?.ToDisplayName() ?? "",
                EmpNature = MyEnumHelper.TryParseEnum<EmpNature>(row.EmploymentNature)?.ToDisplayName() ?? "",
                WorkArr = MyEnumHelper.TryParseEnum<WorkArrangement>(row.WorkArrangement)?.ToDisplayName() ?? "",
                IsDeleted = false,  // handled by QueryBuilder soft-delete
                DateAdd = row.DateAdd,
                DateMod = row.DateMod,
                RowVersion = Convert.ToBase64String(row.RowVersion)
            });
        }

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
        var conn = await _db.GetOpenConnectionAsync(ct: ct);
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
            .Join<Employee, EmpPhoto>(e, ph, x => x.Id, x => x.EmployeeId, "LEFT JOIN")
            .Join<EmpPhoto, EmpPhotoThumbnail>(ph, th, x => x.ThumbnailId, x => x.FileMetaDataId, "LEFT JOIN")
            .And<Employee>(e, x => x.Id, "=", request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        EmployeeJoinRow? row = null;

        using var reader = (DbDataReader)await conn.ExecuteReaderAsync(new CommandDefinition(sql, parameters, cancellationToken: ct), CommandBehavior.SequentialAccess);
        var parser = reader.GetRowParser<EmployeeJoinRow>();
        if (await reader.ReadAsync(ct)) { row = parser(reader); }
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
            Gender = MyEnumHelper.TryParseEnum<Gender>(row.Gender)?.ToDisplayName() ?? "",
            Branch = deptTask.Result?.Res?.NameAm ?? "",
            Department = deptTask.Result?.Res?.Name ?? "",
            Position = positionTask.Result?.Res?.Name ?? "",
            JobGrade = jobGradeTask.Result?.Res?.Name ?? "",
            EmpType = MyEnumHelper.TryParseEnum<EmpType>(row.EmploymentType)?.ToDisplayName() ?? "",
            EmpNature = MyEnumHelper.TryParseEnum<EmpNature>(row.EmploymentNature)?.ToDisplayName() ?? "",
            WorkArr = MyEnumHelper.TryParseEnum<WorkArrangement>(row.WorkArrangement)?.ToDisplayName() ?? "",
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;
    private readonly ILupClient _gRPC;

    public Step5QryHandler(IUnitOfWork unitOfWork, ICorHrmmClient corHRMM, ICorModClient corMod, ILupClient gRPC)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
        _gRPC = gRPC;
    }

    public async Task<Step5Dto?> Handle(Step5Qry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var rel = await _gRPC.GetRelList(cancellationToken);
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

        var eBio = await _unitOfWork.Repository<EmpBio>().GetFoD(b => b.EmployeeId == request.Id);
        var eFin = await _unitOfWork.Repository<EmpFinance>().GetFoD(b => b.EmployeeId == request.Id);
        var eCon = await _unitOfWork.Repository<EmergencyContact>().GetFoD(b => b.EmployeeId == request.Id);
        var eGua = await _unitOfWork.Repository<EmpGuarantor>().GetFoD(b => b.EmployeeId == request.Id);

        var c = new Step5Dto
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
            Position = pos.Res.Name != null ? pos.Res.Name : "NOT AVAILABLE",
            Department = dept.Res.Name != null ? dept.Res.Name : "NOT AVAILABLE",
            Branch = dept.Res.NameAm != null ? dept.Res.NameAm : "NOT AVAILABLE",
            EmploymentType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
            EmploymentNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName(),
            WorkArr = ((WorkArrangement)Enum.Parse(typeof(WorkArrangement), data.WorkArrangement)).ToDisplayName(),
        };

        if (eBio != null)
        {
            c.BirthDate = $"{eBio.BirthDate:MMMM dd, yyyy}";
            c.BirthDateAm = eBio.BirthDate.ToEthiopianDateString("MMMM dd, yyyy");
            c.BirthLocation = eBio.BirthLocation;
            c.MotherFullName = eBio.MotherFullName;
            c.HasBirthCert = ((YesNo)Enum.Parse(typeof(YesNo), eBio.HasBirthCert)).ToDisplayName();
            c.HasMarriageCert = ((YesNo)Enum.Parse(typeof(YesNo), eBio.HasMarriageCert)).ToDisplayName();
            c.MaritalStatus = ((MaritalStat)Enum.Parse(typeof(MaritalStat), eBio.MaritalStatus)).ToDisplayName();

            var address = await _unitOfWork.Repository<Address>().GetById(eBio.AddressId);
            if (address != null)
            {
                c.Address = $"{((AddressType)Enum.Parse(typeof(AddressType), address.AddressType)).ToDisplayName()}: {address.Region} | {address.Zone}({address.Subcity}) | {address.Woreda} | {address.Kebele})";
                c.Telephone = address.Telephone;
            }
        }
        else
        {
            c.BirthDate = "";
            c.BirthDateAm = "";
            c.BirthLocation = "";
            c.MotherFullName = "";
            c.HasBirthCert = "";
            c.HasMarriageCert = "";
            c.MaritalStatus = "";
            c.Address = "";
            c.Telephone = "";
        }

        if (eFin != null)
        {
            c.Tin = eFin.Tin;
            c.BankAccountNo = eFin.BankAccountNo;
            c.PensionNumber = eFin.PensionNumber;
        }
        else
        {
            c.Tin = "";
            c.BankAccountNo = "";
            c.PensionNumber = "";
        }

        if (eCon != null)
        {
            var con = await _unitOfWork.Repository<Person>().GetById(eCon.PersonId);
            var reV = rel.Res.FirstOrDefault(r => r.Id == eCon.RelationId.ToString());
            var re = "NOT AVAILABLE";
            if (reV != null)
            {
                re = reV.Name;
            }
            c.ConFullName = $"{con!.FirstName} {con.MiddleName} {con.LastName}";
            c.ConFullNameAm = $"{con.FirstNameAm} {con.MiddleNameAm} {con.LastNameAm}";
            c.ConNationality = con.Nationality;
            c.ConGender = ((Gender)Enum.Parse(typeof(Gender), con.Gender)).ToDisplayName();
            c.ConRelation = re;

            var address = await _unitOfWork.Repository<Address>().GetById(eCon.AddressId);
            if (address != null)
            {
                c.ConAddress = $"{((AddressType)Enum.Parse(typeof(AddressType), address.AddressType)).ToDisplayName()}: {address.Region} | {address.Zone}({address.Subcity}) | {address.Woreda} | {address.Kebele})";
                c.ConTelephone = address.Telephone;
            }
            else
            {
                c.ConAddress = "";
                c.ConTelephone = "";
            }
        }
        else
        {
            c.ConFullName = "";
            c.ConFullNameAm = "";
            c.ConNationality = "";
            c.ConGender = "";
            c.ConRelation = "";
            c.ConAddress = "";
            c.ConTelephone = "";
        }

        if (eGua != null)
        {
            var gua = await _unitOfWork.Repository<Person>().GetById(eGua.PersonId);
            var reV = rel.Res.FirstOrDefault(r => r.Id == eGua.RelationId.ToString());
            var re = "NOT AVAILABLE";
            if (reV != null)
            {
                re = reV.Name;
            }
            c.GuaFullName = $"{gua!.FirstName} {gua.MiddleName} {gua.LastName}";
            c.GuaFullNameAm = $"{gua.FirstNameAm} {gua.MiddleNameAm} {gua.LastNameAm}";
            c.GuaNationality = gua.Nationality;
            c.GuaGender = ((Gender)Enum.Parse(typeof(Gender), gua.Gender)).ToDisplayName();
            c.GuaRelation = re;

            var address = await _unitOfWork.Repository<Address>().GetById(eGua.AddressId);
            if (address != null)
            {
                c.GuaAddress = $"{((AddressType)Enum.Parse(typeof(AddressType), address.AddressType)).ToDisplayName()}: {address.Region} | {address.Zone}({address.Subcity}) | {address.Woreda} | {address.Kebele})";
                c.GuaTelephone = address.Telephone;
            }
            else
            {
                c.GuaAddress = "";
                c.GuaTelephone = "";
            }

            var gFile = await _unitOfWork.Repository<EmpGuarantorFile>().GetFoD(b => b.EmpGuarantorId == eGua.Id);
            if (gFile != null)
            {
                var gFileMeta = await _unitOfWork.Repository<FileMetaData>().GetById(gFile.FileMetaDataId);
                c.GuaFileName = gFileMeta!.FileName;
                c.GuaFileSize = SizeFormatter.FormatBytes(gFileMeta.FileSize);
                c.GuaFileType = gFileMeta.ContentType;
            }
            else
            {
                c.GuaFileName = "";
                c.GuaFileSize = "";
                c.GuaFileType = "";
            }
        }
        else
        {
            c.GuaFullName = "";
            c.GuaFullNameAm = "";
            c.GuaNationality = "";
            c.GuaGender = "";
            c.GuaRelation = "";
            c.GuaAddress = "";
            c.GuaTelephone = "";
            c.GuaFileName = "";
            c.GuaFileSize = "";
            c.GuaFileType = "";
        }

        return c;
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