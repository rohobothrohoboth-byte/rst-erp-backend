using Common;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpAddStep1Cmd : IRequest<EmpAddRes> { public Step1Dto AddDto { get; set; } = default!; }
public class EmpAddStep2Cmd : IRequest<EmpAddRes> { public Step2Dto AddDto { get; set; } = default!; }



public class EmpAddStep1CmdHandler(IUnitOfWork _uow, IEmpModService _empModSer) : IRequestHandler<EmpAddStep1Cmd, EmpAddRes>
{
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

            //var empFin = new EmpFinance
            //{
            //    Tin = "",
            //    BankAccountNo = "",
            //    PensionNumber = "",
            //    EmployeeId = data.Id
            //};
            //await _uow.Add(empFin, ct);

            var sDto = new ModSalaryDto
            {
                EmployeeId = data.Id,
                JgStepId = request.AddDto.JgStepId,
                EmploymentDate = request.AddDto.EmploymentDate
            };
            await _empModSer.Salary(sDto, ct);

            if (request.AddDto.File != null)
            {
                var pDto = new ModFileDto
                {
                    Id = data.Id,
                    File = request.AddDto.File
                };
                await _empModSer.Photo(pDto, ct);
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

public class EmpAddStep2CmdHandler(IUnitOfWork _uow, IEmpModService _empModSer) : IRequestHandler<EmpAddStep2Cmd, EmpAddRes>
{
    public async Task<EmpAddRes> Handle(EmpAddStep2Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = new EmpAddRes { Id = request.AddDto.EmployeeId };
            var added = await _uow.Set<EmpGuarantor>().FirstOrDefaultAsync(x => x.EmployeeId == request.AddDto.EmployeeId, ct);
            if (added != null) { return res; }

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
                var pDto = new ModFileDto
                {
                    Id = data.Id,
                    File = request.AddDto.File
                };
                await _empModSer.GraFile(pDto, ct);
            }
            await _uow.Commit(ct);
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}