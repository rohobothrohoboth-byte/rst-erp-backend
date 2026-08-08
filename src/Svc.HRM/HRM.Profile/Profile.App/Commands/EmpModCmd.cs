using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpBasicModCmd : IRequest<EmpModRes> { public EmpModBasicDto ModDto { get; set; } = default!; }
public class EmpBioModCmd : IRequest<EmpModRes> { public EmpModBioDto ModDto { get; set; } = default!; }
public class EmpGuarModCmd : IRequest<EmpModRes> { public EmpModGuarDto ModDto { get; set; } = default!; }
public class EmpStampModCmd : IRequest<EmpModRes> { public ModFileDto Dto { get; set; } = default!; }
public class EmpSignModCmd : IRequest<EmpModRes> { public ModFileDto Dto { get; set; } = default!; }



public class EmpBasicModHandler(IUnitOfWork _uow, IEmpModService _empModSer) : IRequestHandler<EmpBasicModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpBasicModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var emp = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (emp == null) { throw new DomainException("Employee data NOT AVAILABLE for updating.", 404); }

            var per = await _uow.Set<Person>().FirstOrDefaultAsync(x => x.Id == emp.PersonId, ct);
            per!.FirstName = request.ModDto.FirstName;
            per.FirstNameAm = request.ModDto.FirstNameAm;
            per.MiddleName = request.ModDto.MiddleName;
            per.MiddleNameAm = request.ModDto.MiddleNameAm;
            per.LastName = request.ModDto.LastName;
            per.LastNameAm = request.ModDto.LastNameAm;
            per.Gender = request.ModDto.Gender;
            per.Nationality = request.ModDto.Nationality;
            await _uow.Update(per);

            emp.EmploymentDate = request.ModDto.EmploymentDate;
            emp.JobGradeId = request.ModDto.JobGradeId;
            emp.PositionId = request.ModDto.PositionId;
            emp.DepartmentId = request.ModDto.DepartmentId;
            emp.EmploymentType = request.ModDto.EmploymentType;
            emp.EmploymentNature = request.ModDto.EmploymentNature;
            emp.WorkArrangement = request.ModDto.WorkArrangement;
            emp.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(emp);

            var sDto = new ModSalaryDto
            {
                EmployeeId = request.ModDto.Id,
                JgStepId = request.ModDto.JgStepId,
                EmploymentDate = request.ModDto.EmploymentDate
            };
            await _empModSer.Salary(sDto, ct);

            if (request.ModDto.File != null)
            {
                var pDto = new ModFileDto
                {
                    Id = request.ModDto.Id,
                    File = request.ModDto.File
                };
                await _empModSer.Photo(pDto, ct);
            }

            await _uow.Commit(ct);
            var res = new EmpModRes { Id = request.ModDto.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpBioModHandler(IUnitOfWork _uow) : IRequestHandler<EmpBioModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpBioModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var eBio = await _uow.Set<EmpBio>().FirstOrDefaultAsync(x => x.EmployeeId == request.ModDto.EmployeeId, ct);
            if (eBio == null)
            {
                var address = new Address
                {
                    AddressType = request.ModDto.AddressType,
                    Country = request.ModDto.Country,
                    Region = request.ModDto.Region,
                    Subcity = request.ModDto.Subcity,
                    Zone = request.ModDto.Zone,
                    Woreda = request.ModDto.Woreda,
                    Kebele = request.ModDto.Kebele,
                    HouseNo = request.ModDto.HouseNo,
                    Telephone = request.ModDto.Telephone,
                    PoBox = request.ModDto.PoBox,
                    Fax = request.ModDto.Fax,
                    Email = request.ModDto.Email,
                    Website = request.ModDto.Website
                };
                await _uow.Add(address, ct);                              

                var empBio = new EmpBio
                {
                    BirthDate = request.ModDto.BirthDate,
                    BirthLocation = request.ModDto.BirthLocation,
                    MotherFullName = request.ModDto.MotherFullName,
                    HasBirthCert = BoolToStr.EnumToString(YesNo.No),
                    HasMarriageCert = BoolToStr.EnumToString(YesNo.No),
                    MaritalStatus = request.ModDto.MaritalStatus,
                    AddressId = address.Id,
                    EmployeeId = request.ModDto.EmployeeId,
                };
                await _uow.Add(empBio, ct);
            }
            else
            {
                var add = await _uow.Set<Address>().FirstOrDefaultAsync(x => x.Id == eBio.AddressId, ct);
                add!.AddressType = request.ModDto.AddressType;
                add.Country = request.ModDto.Country;
                add.Region = request.ModDto.Region;
                add.Subcity = request.ModDto.Subcity;
                add.Zone = request.ModDto.Zone;
                add.Woreda = request.ModDto.Woreda;
                add.Kebele = request.ModDto.Kebele;
                add.HouseNo = request.ModDto.HouseNo;
                add.Telephone = request.ModDto.Telephone;
                add.PoBox = request.ModDto.PoBox;
                add.Fax = request.ModDto.Fax;
                add.Email = request.ModDto.Email;
                add.Website = request.ModDto.Website;
                await _uow.Update(add);

                eBio.BirthDate = request.ModDto.BirthDate;
                eBio.BirthLocation = request.ModDto.BirthLocation;
                eBio.MotherFullName = request.ModDto.MotherFullName;
                eBio.MaritalStatus = request.ModDto.MaritalStatus;
                eBio.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
                await _uow.Update(eBio);
            }

            var eFin = await _uow.Set<EmpFinance>().FirstOrDefaultAsync(x => x.EmployeeId == request.ModDto.EmployeeId, ct);
            if (eFin == null)
            {
                var empFin = new EmpFinance
                {
                    Tin = request.ModDto.Tin,
                    BankAccountNo = request.ModDto.BankAccountNo,
                    PensionNumber = request.ModDto.PensionNumber,
                    EmployeeId = request.ModDto.Id
                };
                await _uow.Add(empFin, ct);
            }
            else
            {
                eFin.Tin = request.ModDto.Tin;
                eFin.BankAccountNo = request.ModDto.BankAccountNo;
                eFin.PensionNumber = request.ModDto.PensionNumber;
                await _uow.Update(eFin);
            }

            await _uow.Commit(ct);
            var res = new EmpModRes { Id = request.ModDto.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpGuarModHandler(IUnitOfWork _uow, IEmpModService _empModSer) : IRequestHandler<EmpGuarModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpGuarModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var eGuar = await _uow.Set<EmpGuarantor>().FirstOrDefaultAsync(x => x.EmployeeId == request.ModDto.EmployeeId, ct);
            if (eGuar == null)
            {
                var address = new Address
                {
                    AddressType = request.ModDto.AddressType,
                    Country = request.ModDto.Country,
                    Region = request.ModDto.Region,
                    Subcity = request.ModDto.Subcity,
                    Zone = request.ModDto.Zone,
                    Woreda = request.ModDto.Woreda,
                    Kebele = request.ModDto.Kebele,
                    HouseNo = request.ModDto.HouseNo,
                    Telephone = request.ModDto.Telephone,
                    PoBox = request.ModDto.PoBox,
                    Fax = request.ModDto.Fax,
                    Email = request.ModDto.Email,
                    Website = request.ModDto.Website
                };
                await _uow.Add(address, ct);

                var data = new EmpGuarantor
                {
                    FirstName = request.ModDto.FirstName,
                    MiddleName = request.ModDto.MiddleName,
                    LastName = request.ModDto.LastName,
                    Gender = request.ModDto.Gender,
                    Nationality = request.ModDto.Nationality,
                    Relation = request.ModDto.Relation,
                    AddressId = address.Id,
                    EmployeeId = request.ModDto.EmployeeId
                };
                await _uow.Add(data, ct);

                if (request.ModDto.File != null)
                {
                    var fDto = new ModFileDto
                    {
                        Id = data.Id,
                        File = request.ModDto.File
                    };
                    await _empModSer.GraFile(fDto, ct);
                }
            }
            else
            {
                var add = await _uow.Set<Address>().FirstOrDefaultAsync(x => x.Id == eGuar.AddressId, ct);
                add!.AddressType = request.ModDto.AddressType;
                add.Country = request.ModDto.Country;
                add.Region = request.ModDto.Region;
                add.Subcity = request.ModDto.Subcity;
                add.Zone = request.ModDto.Zone;
                add.Woreda = request.ModDto.Woreda;
                add.Kebele = request.ModDto.Kebele;
                add.HouseNo = request.ModDto.HouseNo;
                add.Telephone = request.ModDto.Telephone;
                add.PoBox = request.ModDto.PoBox;
                add.Fax = request.ModDto.Fax;
                add.Email = request.ModDto.Email;
                add.Website = request.ModDto.Website;
                await _uow.Update(add);

                eGuar.FirstName = request.ModDto.FirstName;
                eGuar.MiddleName = request.ModDto.MiddleName;
                eGuar.LastName = request.ModDto.LastName;
                eGuar.Gender = request.ModDto.Gender;
                eGuar.Nationality = request.ModDto.Nationality;
                eGuar.Relation = request.ModDto.Relation;
                eGuar.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
                await _uow.Update(eGuar);

                if (request.ModDto.File != null)
                {
                    var fDto = new ModFileDto
                    {
                        Id = eGuar.Id,
                        File = request.ModDto.File
                    };
                    await _empModSer.GraFile(fDto, ct);
                }
            }

            await _uow.Commit(ct);
            var res = new EmpModRes { Id = request.ModDto.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpStampModHandler(IUnitOfWork _uow, IEmpModService _empModSer) : IRequestHandler<EmpStampModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpStampModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            if (request.Dto.File != null)
            {
                await _empModSer.StampFile(request.Dto, ct);
            }

            await _uow.Commit(ct);
            var res = new EmpModRes { Id = request.Dto.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpSignModHandler(IUnitOfWork _uow, IEmpModService _empModSer) : IRequestHandler<EmpSignModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpSignModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            if (request.Dto.File != null)
            {
                await _empModSer.SignFile(request.Dto, ct);
            }

            await _uow.Commit(ct);
            var res = new EmpModRes { Id = request.Dto.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

