using Helpers;
using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpAddStep1Cmd : IRequest<EmpAddRes> { public Step1Dto AddDto { get; set; } = default!; }
public class EmpAddStep2Cmd : IRequest<EmpAddRes> { public Step2Dto AddDto { get; set; } = default!; }
public class EmpAddStep3Cmd : IRequest<EmpAddRes> { public Step3Dto AddDto { get; set; } = default!; }
public class EmpAddStep4Cmd : IRequest<EmpAddRes> { public Step4Dto AddDto { get; set; } = default!; }

public class EmpAddStep1CmdHandler : IRequestHandler<EmpAddStep1Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpAddStep1CmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; }

    public async Task<EmpAddRes> Handle(EmpAddStep1Cmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var per = new Person
            {
                FirstName = request.AddDto.FirstName,
                FirstNameAm = request.AddDto.FirstNameAm,
                MiddleName = request.AddDto.MiddleName,
                MiddleNameAm = request.AddDto.MiddleNameAm,
                LastName = request.AddDto.LastName,
                LastNameAm = request.AddDto.LastNameAm,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality
            };
            await _unitOfWork.Repository<Person>().Add(per);

            var code = await new CodeGen(_unitOfWork).GetEmpCode();
            var data = new Employee
            {
                Code = code,
                EmploymentDate = request.AddDto.EmploymentDate,
                JobGradeId = request.AddDto.JobGradeId,
                PositionId = request.AddDto.PositionId,
                DepartmentId = request.AddDto.DepartmentId,
                EmploymentType = request.AddDto.EmploymentType,
                EmploymentNature = request.AddDto.EmploymentNature,
                WorkArrangement = request.AddDto.WorkArrangement,
                PersonId = per.Id
            };
            await _unitOfWork.Repository<Employee>().Add(data);

            if (request.AddDto.File != null)
            {
                var mData = new FileMetaData
                {
                    FileName = request.AddDto.File.FileName,
                    ContentType = request.AddDto.File.ContentType,
                    FileSize = request.AddDto.File.Length
                };
                await _unitOfWork.Repository<FileMetaData>().Add(mData);

                using var ms = new MemoryStream();
                await request.AddDto.File.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;
                var pBlob = new EmpPhotoBlob
                {
                    FileMetaDataId = mData.Id,
                    Data = ms.ToArray()
                };
                await _unitOfWork.Repository<EmpPhotoBlob>().Add(pBlob);

                var thumbData = ThumbnailGenerator.GenerateThumbnail(ms);
                var tData = new FileMetaData
                {
                    FileName = $"{request.AddDto.File.FileName}_thumbnail",
                    ContentType = "image/png",
                    FileSize = thumbData.Length
                };
                await _unitOfWork.Repository<FileMetaData>().Add(tData);

                var tBlob = new EmpPhotoThumbnail
                {
                    FileMetaDataId = tData.Id,
                    Data = thumbData.ToArray()
                };
                await _unitOfWork.Repository<EmpPhotoThumbnail>().Add(tBlob);

                var emp = new EmpPhoto
                {
                    ThumbnailId = tData.Id,
                    FileMetaDataId = mData.Id,
                    EmployeeId = data.Id
                };
                await _unitOfWork.Repository<EmpPhoto>().Add(emp);
            }

            await _unitOfWork.Commit();
            var res = new EmpAddRes { Id = data.Id };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class EmpAddStep2CmdHandler : IRequestHandler<EmpAddStep2Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpAddStep2CmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; }

    public async Task<EmpAddRes> Handle(EmpAddStep2Cmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var address = new Address
            {
                AddressType = request.AddDto.AddressType,
                Country = request.AddDto.Country,
                Region = request.AddDto.Region,
                Subcity = request.AddDto.Subcity,
                Zone = request.AddDto.Zone,
                Woreda = request.AddDto.Woreda,
                Kebele = request.AddDto.Kebele,
                HouseNo = request.AddDto.HouseNo,
                Telephone = request.AddDto.Telephone,
                PoBox = request.AddDto.PoBox,
                Fax = request.AddDto.Fax,
                Email = request.AddDto.Email,
                Website = request.AddDto.Website
            };
            await _unitOfWork.Repository<Address>().Add(address);

            var fin = new EmpFinance
            {
                Tin = request.AddDto.Tin,
                BankAccountNo = request.AddDto.BankAccountNo,
                PensionNumber = request.AddDto.PensionNumber,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpFinance>().Add(fin);

            var data = new EmpBio
            {
                BirthDate = request.AddDto.BirthDate,
                BirthLocation = request.AddDto.BirthLocation,
                MotherFullName = request.AddDto.MotherFullName,
                HasBirthCert = request.AddDto.HasBirthCert,
                HasMarriageCert = request.AddDto.HasMarriageCert,
                MaritalStatus = request.AddDto.MaritalStatus,
                AddressId = address.Id,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpBio>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpAddRes { Id = request.AddDto.EmployeeId };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}


public class EmpAddStep3CmdHandler : IRequestHandler<EmpAddStep3Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpAddStep3CmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; }

    public async Task<EmpAddRes> Handle(EmpAddStep3Cmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var address = new Address
            {
                AddressType = request.AddDto.AddressType,
                Country = request.AddDto.Country,
                Region = request.AddDto.Region,
                Subcity = request.AddDto.Subcity,
                Zone = request.AddDto.Zone,
                Woreda = request.AddDto.Woreda,
                Kebele = request.AddDto.Kebele,
                HouseNo = request.AddDto.HouseNo,
                Telephone = request.AddDto.Telephone,
                PoBox = request.AddDto.PoBox,
                Fax = request.AddDto.Fax,
                Email = request.AddDto.Email,
                Website = request.AddDto.Website
            };
            await _unitOfWork.Repository<Address>().Add(address);

            var per = new Person
            {
                FirstName = request.AddDto.FirstName,
                FirstNameAm = request.AddDto.FirstNameAm,
                MiddleName = request.AddDto.MiddleName,
                MiddleNameAm = request.AddDto.MiddleNameAm,
                LastName = request.AddDto.LastName,
                LastNameAm = request.AddDto.LastNameAm,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality
            };
            await _unitOfWork.Repository<Person>().Add(per);

            var data = new EmergencyContact
            {
                AddressId = address.Id,
                RelationId = request.AddDto.RelationId,
                EmployeeId = request.AddDto.EmployeeId,
                PersonId = per.Id
            };
            await _unitOfWork.Repository<EmergencyContact>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpAddRes { Id = request.AddDto.EmployeeId };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class EmpAddStep4CmdHandler : IRequestHandler<EmpAddStep4Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpAddStep4CmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; }

    public async Task<EmpAddRes> Handle(EmpAddStep4Cmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var per = new Person
            {
                FirstName = request.AddDto.FirstName,
                FirstNameAm = request.AddDto.FirstNameAm,
                MiddleName = request.AddDto.MiddleName,
                MiddleNameAm = request.AddDto.MiddleNameAm,
                LastName = request.AddDto.LastName,
                LastNameAm = request.AddDto.LastNameAm,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality
            };
            await _unitOfWork.Repository<Person>().Add(per);

            var address = new Address
            {
                AddressType = request.AddDto.AddressType,
                Country = request.AddDto.Country,
                Region = request.AddDto.Region,
                Subcity = request.AddDto.Subcity,
                Zone = request.AddDto.Zone,
                Woreda = request.AddDto.Woreda,
                Kebele = request.AddDto.Kebele,
                HouseNo = request.AddDto.HouseNo,
                Telephone = request.AddDto.Telephone,
                PoBox = request.AddDto.PoBox ?? "",
                Fax = request.AddDto.Fax ?? "",
                Email = request.AddDto.Email ?? "",
                Website = request.AddDto.Website ?? ""
            };

            await _unitOfWork.Repository<Address>().Add(address);

            var data = new EmpGuarantor
            {
                AddressId = address.Id,
                RelationId = request.AddDto.RelationId,
                EmployeeId = request.AddDto.EmployeeId,
                PersonId = per.Id
            };
            await _unitOfWork.Repository<EmpGuarantor>().Add(data);

            if (request.AddDto.File != null)
            {
                var mData = new FileMetaData
                {
                    FileName = request.AddDto.File.FileName,
                    ContentType = request.AddDto.File.ContentType,
                    FileSize = request.AddDto.File.Length
                };
                await _unitOfWork.Repository<FileMetaData>().Add(mData);

                using var ms = new MemoryStream();
                await request.AddDto.File.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;
                var pBlob = new EmpGuarantorFileBlob
                {
                    FileMetaDataId = mData.Id,
                    Data = ms.ToArray()
                };
                await _unitOfWork.Repository<EmpGuarantorFileBlob>().Add(pBlob);

                var emp = new EmpGuarantorFile
                {
                    FileMetaDataId = mData.Id,
                    EmpGuarantorId = data.Id
                };
                await _unitOfWork.Repository<EmpGuarantorFile>().Add(emp);
            }
            await _unitOfWork.Commit();

            var res = new EmpAddRes { Id = request.AddDto.EmployeeId };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}