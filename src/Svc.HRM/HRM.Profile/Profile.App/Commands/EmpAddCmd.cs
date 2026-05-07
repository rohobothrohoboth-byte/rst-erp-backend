using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpAddStep1Cmd : IRequest<EmpAddRes> { public Step1Dto AddDto { get; set; } = default!; }
public class EmpAddStep2Cmd : IRequest<EmpAddRes> { public Step2Dto AddDto { get; set; } = default!; }



public class EmpAddStep1CmdHandler : IRequestHandler<EmpAddStep1Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _uow;
    private readonly ICorHrmmClient _hrmmClient;
    public EmpAddStep1CmdHandler(IUnitOfWork uow, ICorHrmmClient hrmmClient) { _uow = uow; _hrmmClient = hrmmClient;}

    public async Task<EmpAddRes> Handle(EmpAddStep1Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
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
            await _uow.Add(per, ct);

            var eState = BoolToStr.EnumToString(EmpState.Pen);
            var data = new Employee
            {
                EmpState = eState,
                EmploymentDate = request.AddDto.EmploymentDate,
                JobGradeId = request.AddDto.JobGradeId,
                PositionId = request.AddDto.PositionId,
                DepartmentId = request.AddDto.DepartmentId,
                EmploymentType = request.AddDto.EmploymentType,
                EmploymentNature = request.AddDto.EmploymentNature,
                WorkArrangement = request.AddDto.WorkArrangement,
                PersonId = per.Id
            };
            await _uow.Add(data, ct);

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
            await _uow.Add(address, ct);

            var empBio = new EmpBio
            {
                BirthDate = request.AddDto.BirthDate,
                BirthLocation = "",
                MotherFullName = "",
                HasBirthCert = BoolToStr.EnumToString(YesNo.No),
                HasMarriageCert = BoolToStr.EnumToString(YesNo.No),
                MaritalStatus = request.AddDto.MaritalStatus,
                AddressId = address.Id,
                EmployeeId = data.Id
            };
            await _uow.Add(empBio, ct);

            var empFin = new EmpFinance
            {
                Tin = "",
                BankAccountNo = "",
                PensionNumber = "",
                EmployeeId = data.Id
            };
            await _uow.Add(empFin, ct);

            var salary = new EmpSalary
            {
                BaseSalary = 0,
                Currency = "",
                SalaryPayFreq = "",
                EffectiveFrom = request.AddDto.EmploymentDate,
                JgStepId = request.AddDto.JgStepId,
                EmployeeId = data.Id
            };
            var slyTask = await _hrmmClient.GetSalaryJgs((request.AddDto.JgStepId).ToString(), ct);
            if (slyTask.Salary != null)
            {
                salary.BaseSalary = double.Parse(slyTask.Salary);
                salary.Currency = slyTask.Currency;
                salary.SalaryPayFreq = slyTask.SalaryPayFreq;
            }
            await _uow.Add(salary, ct);

            if (request.AddDto.File != null)
            {
                var mData = new FileMetaData
                {
                    FileName = request.AddDto.File.FileName,
                    ContentType = request.AddDto.File.ContentType,
                    FileSize = request.AddDto.File.Length
                };
                await _uow.Add(mData, ct);

                using var ms = new MemoryStream();
                await request.AddDto.File.CopyToAsync(ms, ct);
                ms.Position = 0;
                var pBlob = new EmpPhotoBlob
                {
                    FileMetaDataId = mData.Id,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob, ct);

                var thumbData = ThumbnailGenerator.GenerateThumbnail(ms);
                var tData = new FileMetaData
                {
                    FileName = $"{request.AddDto.File.FileName}_thumbnail",
                    ContentType = "image/png",
                    FileSize = thumbData.Length
                };
                await _uow.Add(tData, ct);

                var tBlob = new EmpPhotoThumbnail
                {
                    FileMetaDataId = tData.Id,
                    Data = thumbData.ToArray()
                };
                await _uow.Add(tBlob, ct);

                var emp = new EmpPhoto
                {
                    ThumbnailId = tData.Id,
                    FileMetaDataId = mData.Id,
                    EmployeeId = data.Id
                };
                await _uow.Add(emp, ct);
            }

            await _uow.Commit(ct);
            var res = new EmpAddRes { Id = data.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpAddStep2CmdHandler : IRequestHandler<EmpAddStep2Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _uow;
    public EmpAddStep2CmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; }

    public async Task<EmpAddRes> Handle(EmpAddStep2Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
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
                PoBox = request.AddDto.PoBox ?? "",
                Fax = request.AddDto.Fax ?? "",
                Email = request.AddDto.Email ?? "",
                Website = request.AddDto.Website ?? ""
            };
            await _uow.Add(address, ct);

            var data = new EmpGuarantor
            {
                FirstName = request.AddDto.FirstName,
                MiddleName = request.AddDto.MiddleName,
                LastName = request.AddDto.LastName,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality,
                Relation = request.AddDto.Relation,
                AddressId = address.Id,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _uow.Add(data, ct);

            if (request.AddDto.File != null)
            {
                var mData = new FileMetaData
                {
                    FileName = request.AddDto.File.FileName,
                    ContentType = request.AddDto.File.ContentType,
                    FileSize = request.AddDto.File.Length
                };
                await _uow.Add(mData, ct);

                using var ms = new MemoryStream();
                await request.AddDto.File.CopyToAsync(ms, ct);
                ms.Position = 0;
                var pBlob = new EmpGuarantorFileBlob
                {
                    FileMetaDataId = mData.Id,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob, ct);

                var emp = new EmpGuarantorFile
                {
                    FileMetaDataId = mData.Id,
                    EmpGuarantorId = data.Id
                };
                await _uow.Add(emp, ct);
            }
            await _uow.Commit(ct);

            var res = new EmpAddRes { Id = request.AddDto.EmployeeId };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}