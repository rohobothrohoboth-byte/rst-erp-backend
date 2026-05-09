using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpBioModCmd : IRequest<EmpModRes> { public EmpBioModDto ModDto { get; set; } = default!; }
public class EmpFinanceModCmd : IRequest<EmpModRes> { public EmpFinanceModDto ModDto { get; set; } = default!; }
public class EmContactModCmd : IRequest<EmpModRes> { public EmContactModDto ModDto { get; set; } = default!; }
public class EmpFamilyAddCmd : IRequest<EmpModRes> { public EmpFamilyAddDto AddDto { get; set; } = default!; }
public class EmpFamilyModCmd : IRequest<EmpModRes> { public EmpFamilyModDto ModDto { get; set; } = default!; }
public class EmpFamilyDelCmd : IRequest { public Guid Id { get; set; } }



public class EmpBioModHandler(IUnitOfWork _uow, IEmpCertService _iEmpCertSer) : IRequestHandler<EmpBioModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpBioModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var added = await _uow.Set<EmpBio>().FirstOrDefaultAsync(x => x.EmployeeId == request.ModDto.Id, ct);
            if (added == null) { throw new DomainException("Dear Employee PLEASE CONTACT HR before updating your profile."); }

            added.BirthLocation = request.ModDto.BirthLocation;
            added.MotherFullName = request.ModDto.MotherFullName;
            added.HasBirthCert = request.ModDto.HasBirthCert;
            added.HasMarriageCert = request.ModDto.HasMarriageCert;
            await _uow.Update(added);

            var dto = new CertSerDto
            {
                Id = request.ModDto.Id,
                HasCert = request.ModDto.HasBirthCert,
                File = request.ModDto.File1
            };

            var dto2 = new CertSerDto
            {
                Id = request.ModDto.Id,
                HasCert = request.ModDto.HasMarriageCert,
                File = request.ModDto.File2
            };
            await _iEmpCertSer.CertBirth(dto, ct);
            await _iEmpCertSer.CertMarriage(dto2, ct);
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

public class EmpFinanceModHandler(IUnitOfWork _uow) : IRequestHandler<EmpFinanceModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpFinanceModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var added = await _uow.Set<EmpFinance>().FirstOrDefaultAsync(x => x.EmployeeId == request.ModDto.Id, ct);
            if (added != null)
            {
                added.Tin = request.ModDto.Tin;
                added.BankAccountNo = request.ModDto.BankAccountNo;
                added.PensionNumber = request.ModDto.PensionNumber;
                await _uow.Update(added);
            }
            else
            {
                var data = new EmpFinance
                {
                    Tin = request.ModDto.Tin,
                    BankAccountNo = request.ModDto.BankAccountNo,
                    PensionNumber = request.ModDto.PensionNumber,
                    EmployeeId = request.ModDto.Id
                };
                await _uow.Add(data, ct);
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

public class EmContactModHandler(IUnitOfWork _uow) : IRequestHandler<EmContactModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmContactModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var added = await _uow.Set<EmergencyContact>().FirstOrDefaultAsync(x => x.EmployeeId == request.ModDto.EmployeeId, ct);
            if (added != null)
            {
                var address = await _uow.Set<Address>().FirstOrDefaultAsync(x => x.Id == added.AddressId, ct);
                address!.AddressType = request.ModDto.AddressType;
                address.Country = request.ModDto.Country;
                address.Region = request.ModDto.Region;
                address.Subcity = request.ModDto.Subcity;
                address.Zone = request.ModDto.Zone;
                address.Woreda = request.ModDto.Woreda;
                address.Kebele = request.ModDto.Kebele;
                address.HouseNo = request.ModDto.HouseNo;
                address.Telephone = request.ModDto.Telephone;
                address.PoBox = request.ModDto.PoBox;
                address.Fax = request.ModDto.Fax;
                address.Email = request.ModDto.Email;
                address.Website = request.ModDto.Website;
                await _uow.Update(address);

                added.FirstName = request.ModDto.FirstName;
                added.MiddleName = request.ModDto.MiddleName;
                added.LastName = request.ModDto.LastName;
                added.Gender = request.ModDto.Gender;
                added.Nationality = request.ModDto.Nationality;
                added.Relation = request.ModDto.Relation;
                await _uow.Update(added);
            }
            else
            {
                var addr = new Address
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
                    PoBox = request.ModDto.PoBox ?? "",
                    Fax = request.ModDto.Fax ?? "",
                    Email = request.ModDto.Email ?? "",
                    Website = request.ModDto.Website ?? ""
                };
                await _uow.Add(addr, ct);

                var data = new EmergencyContact
                {
                    FirstName = request.ModDto.FirstName,
                    MiddleName = request.ModDto.MiddleName,
                    LastName = request.ModDto.LastName,
                    Gender = request.ModDto.Gender,
                    Nationality = request.ModDto.Nationality,
                    Relation = request.ModDto.Relation,
                    AddressId = addr.Id,
                    EmployeeId = request.ModDto.EmployeeId
                };
                await _uow.Add(data, ct);
            }

            await _uow.Commit(ct);
            var res = new EmpModRes { Id = request.ModDto.EmployeeId };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpFamilyAddHandler(IUnitOfWork _uow) : IRequestHandler<EmpFamilyAddCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpFamilyAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EmpFamily
            {
                FirstName = request.AddDto.FirstName,
                MiddleName = request.AddDto.MiddleName,
                LastName = request.AddDto.LastName,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality,
                Relation = request.AddDto.Relation,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new EmpModRes { Id = request.AddDto.EmployeeId };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpFamilyModHandler(IUnitOfWork _uow) : IRequestHandler<EmpFamilyModCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpFamilyModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var added = await _uow.Set<EmpFamily>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (added == null) { throw new DomainException("EMPLOYEE'S Families Info NOT FOUND based on give parameter."); }

            added.FirstName = request.ModDto.FirstName;
            added.MiddleName = request.ModDto.MiddleName;
            added.LastName = request.ModDto.LastName;
            added.Gender = request.ModDto.Gender;
            added.Nationality = request.ModDto.Nationality;
            added.Relation = request.ModDto.Relation;
            await _uow.Update(added);
            await _uow.Commit(ct);

            var res = new EmpModRes { Id = added.EmployeeId };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpFamilyDelHandler(IUnitOfWork _uow) : IRequestHandler<EmpFamilyDelCmd>
{
    public async Task Handle(EmpFamilyDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EmpFamily>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException("Employee Family with given parameter NOT FOUND."); }
            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}