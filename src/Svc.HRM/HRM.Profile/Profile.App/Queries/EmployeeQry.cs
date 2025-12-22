using Common;
using EthiopianCalendar;
using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Profile.Domain.Enums;

namespace Profile.App.Queries;

public class EmployeeAllQry : IRequest<List<EmployeeListDto>> { }
public class EmployeeByIdQry : IRequest<EmployeeListDto?> { public Guid Id { get; set; } }
public class Step5Qry : IRequest<Step5Dto?> { public Guid Id { get; set; } }
public class Step2Qry : IRequest<BasicInfoDto?> { public Guid Id { get; set; } }

public class EmployeeAllQryHandler : IRequestHandler<EmployeeAllQry, List<EmployeeListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ICorMod _corMod;

    public EmployeeAllQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<List<EmployeeListDto>> Handle(EmployeeAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Employee>().GetAll();
        var dataL = new List<EmployeeListDto>();
        var perL = await _unitOfWork.Repository<Person>().GetAll();
        var deL = await _corMod.DeptList(cancellationToken);
        var jgL = await _corHRMM.JobGradeList(cancellationToken);
        var poL = await _corHRMM.PositionList(cancellationToken);
        var ePhotoL = await _unitOfWork.Repository<EmpPhoto>().GetAll();

        foreach (var data in dbData)
        {
            var per = perL.FirstOrDefault(t => t.Id == data.PersonId);
            var dept = deL!.FirstOrDefault(t => t.Id == data.DepartmentId);
            var jg = jgL!.FirstOrDefault(t => t.Id == data.JobGradeId);
            var pos = poL!.FirstOrDefault(t => t.Id == data.PositionId);
            var photo = "";
            if (ePhotoL.Any())
            {
                var ePhoto = ePhotoL.FirstOrDefault(t => t.EmployeeId == data.Id);
                if (ePhoto != null)
                {
                    var ePhotoB = await _unitOfWork.Repository<EmpPhotoThumbnail>().GetFoD(t => t.FileMetaDataId == ePhoto.ThumbnailId);
                    photo = Convert.ToBase64String(ePhotoB!.Data);
                }
            }

            var c = new EmployeeListDto
            {
                Id = data.Id,
                EmpFullName = per != null ? $"{per.FirstName} {per.MiddleName} {per.LastName}" : "NOT AVAILABLE",
                EmpFullNameAm = per != null ? $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}" : "NOT AVAILABLE",
                Code = data.Code,
                Gender = ((Gender)Enum.Parse(typeof(Gender), per!.Gender)).ToDisplayName(),
                Branch = dept != null ? dept.NameAm : "NOT AVAILABLE",
                Department = dept != null ? dept.Name : "NOT AVAILABLE",
                Position = pos != null ? pos.Name : "NOT AVAILABLE",
                JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
                EmpType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
                EmpNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName(),
                Photo = photo,
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

public class EmployeeByIdQryHandler : IRequestHandler<EmployeeByIdQry, EmployeeListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ICorMod _corMod;

    public EmployeeByIdQryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<EmployeeListDto?> Handle(EmployeeByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var dept = await _corMod.Dept(data.DepartmentId, cancellationToken);
        var jg = await _corHRMM.JobGrade(data.JobGradeId, cancellationToken);
        var pos = await _corHRMM.Position(data.PositionId, cancellationToken);
        var ePhoto = await _unitOfWork.Repository<EmpPhoto>().GetFoD(t => t.EmployeeId == request.Id);
        var ePhotoB = await _unitOfWork.Repository<EmpPhotoThumbnail>().GetFoD(t => t.FileMetaDataId == ePhoto!.FileMetaDataId);

        var c = new EmployeeListDto
        {
            Id = data.Id,
            EmpFullName = $"{per!.FirstName} {per.MiddleName} {per.LastName}",
            EmpFullNameAm = $"{per.FirstNameAm} {per.MiddleNameAm} {per.LastNameAm}",
            Code = data.Code,
            Gender = ((Gender)Enum.Parse(typeof(Gender), per.Gender)).ToDisplayName(),
            Branch = dept != null ? dept.NameAm : "NOT AVAILABLE",
            Department = dept != null ? dept.Name : "NOT AVAILABLE",
            Position = pos != null ? pos.Name : "NOT AVAILABLE",
            JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
            EmpType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
            EmpNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName(),
            Photo = Convert.ToBase64String(ePhotoB!.Data),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class Step5QryHandler : IRequestHandler<Step5Qry, Step5Dto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICorHRMM _corHRMM;
    private readonly ICorMod _corMod;
    private readonly ILupClient _lupClient;

    public Step5QryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod, ILupClient lupClient)
    {
        _unitOfWork = unitOfWork;
        _corHRMM = corHRMM;
        _corMod = corMod;
        _lupClient = lupClient;
    }

    public async Task<Step5Dto?> Handle(Step5Qry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Employee>().GetById(request.Id);
        if (data == null) { return null; }
        var per = await _unitOfWork.Repository<Person>().GetById(data.PersonId);
        var rel = await _lupClient.GetRelList(cancellationToken);
        var dept = await _corMod.Dept(data.DepartmentId, cancellationToken);
        var jg = await _corHRMM.JobGrade(data.JobGradeId, cancellationToken);
        var pos = await _corHRMM.Position(data.PositionId, cancellationToken);
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
            JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
            Position = pos != null ? pos.Name : "NOT AVAILABLE",
            Department = dept != null ? dept.Name : "NOT AVAILABLE",
            Branch = dept != null ? dept.NameAm : "NOT AVAILABLE",
            EmploymentType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
            EmploymentNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName()
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
            if (reV.Id != null)
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
            if (reV.Id != null)
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
    private readonly ICorHRMM _corHRMM;
    private readonly ICorMod _corMod;

    public Step2QryHandler(IUnitOfWork unitOfWork, ICorHRMM corHRMM, ICorMod corMod)
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
        var dept = await _corMod.Dept(data.DepartmentId, cancellationToken);
        var jg = await _corHRMM.JobGrade(data.JobGradeId, cancellationToken);
        var pos = await _corHRMM.Position(data.PositionId, cancellationToken);
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
            JobGrade = jg != null ? jg.Name : "NOT AVAILABLE",
            Position = pos != null ? pos.Name : "NOT AVAILABLE",
            Department = dept != null ? dept.Name : "NOT AVAILABLE",
            Branch = dept != null ? dept.NameAm : "NOT AVAILABLE",
            EmploymentType = ((EmpType)Enum.Parse(typeof(EmpType), data.EmploymentType)).ToDisplayName(),
            EmploymentNature = ((EmpNature)Enum.Parse(typeof(EmpNature), data.EmploymentNature)).ToDisplayName()
        };

        return c;
    }
}