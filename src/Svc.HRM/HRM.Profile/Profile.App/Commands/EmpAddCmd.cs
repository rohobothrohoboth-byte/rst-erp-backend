using Common;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Services;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;
using Shared.Helpers.Events;
using Microsoft.Extensions.Logging;

namespace Profile.App.Commands;

public class EmpAddStep1Cmd : IRequest<EmpAddRes> { public Step1Dto AddDto { get; set; } = default!; }
public class EmpAddStep2Cmd : IRequest<EmpAddRes> { public Step2Dto AddDto { get; set; } = default!; }

public class EmpAddStep1CmdHandler : IRequestHandler<EmpAddStep1Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmpModService _empModSer;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<EmpAddStep1CmdHandler> _logger;

    public EmpAddStep1CmdHandler(
        IUnitOfWork uow,
        IEmpModService empModSer,
        IEventPublisher eventPublisher,
        ILogger<EmpAddStep1CmdHandler> logger)
    {
        _uow = uow;
        _empModSer = empModSer;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<EmpAddRes> Handle(EmpAddStep1Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // ? FIX: Convert dates to UTC
            var employmentDate = EnsureUtc(request.AddDto.EmploymentDate);
            var birthDate = EnsureUtc(request.AddDto.BirthDate);
            var now = DateTime.UtcNow;

            var per = new Person
            {
                Id = Guid.CreateVersion7(),
                FirstName = request.AddDto.FirstName,
                FirstNameAm = request.AddDto.FirstNameAm,
                MiddleName = request.AddDto.MiddleName,
                MiddleNameAm = request.AddDto.MiddleNameAm,
                LastName = request.AddDto.LastName,
                LastNameAm = request.AddDto.LastNameAm,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality,
                DateAdd = now,
                DateMod = now,
                IsDeleted = false
            };
            await _uow.Add(per, ct);

            var eState = BoolToStr.EnumToString(EmpState.Pen);
            var data = new Employee
            {
                Id = Guid.CreateVersion7(),
                EmpState = eState,
                EmploymentDate = employmentDate,
                JobGradeId = request.AddDto.JobGradeId,
                PositionId = request.AddDto.PositionId,
                DepartmentId = request.AddDto.DepartmentId,
                ReportsToId = request.AddDto.ReportsToId,
                EmploymentType = request.AddDto.EmploymentType,
                EmploymentNature = request.AddDto.EmploymentNature,
                WorkArrangement = request.AddDto.WorkArrangement,
                PersonId = per.Id,
                DateAdd = now,
                DateMod = now,
                IsDeleted = false
            };
            await _uow.Add(data, ct);

            var address = new Address
            {
                Id = Guid.CreateVersion7(),
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
                Website = request.AddDto.Website,
                DateAdd = now,
                DateMod = now,
                IsDeleted = false
            };
            await _uow.Add(address, ct);

            var empBio = new EmpBio
            {
                Id = Guid.CreateVersion7(),
                BirthDate = birthDate,
                BirthLocation = "",
                MotherFullName = "",
                HasBirthCert = BoolToStr.EnumToString(YesNo.No),
                HasMarriageCert = BoolToStr.EnumToString(YesNo.No),
                MaritalStatus = request.AddDto.MaritalStatus,
                AddressId = address.Id,
                EmployeeId = data.Id,
                DateAdd = now,
                DateMod = now,
                IsDeleted = false
            };
            await _uow.Add(empBio, ct);

            var sDto = new ModSalaryDto
            {
                EmployeeId = data.Id,
                JgStepId = request.AddDto.JgStepId,
                EmploymentDate = employmentDate
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

            // ? Publish event for real-time sync
            var employeeData = new EmployeeEventData
            {
                Id = data.Id,
                Code = data.Code,
                EmpState = data.EmpState,
                EmploymentType = data.EmploymentType,
                EmploymentNature = data.EmploymentNature,
                WorkArrangement = data.WorkArrangement,
                EmploymentDate = data.EmploymentDate,
                PersonId = data.PersonId,
                JobGradeId = data.JobGradeId,
                PositionId = data.PositionId,
                DepartmentId = data.DepartmentId,
                AppUserId = data.AppUserId,
                FirstName = per.FirstName,
                FirstNameAm = per.FirstNameAm,
                MiddleName = per.MiddleName,
                MiddleNameAm = per.MiddleNameAm,
                LastName = per.LastName,
                LastNameAm = per.LastNameAm,
                Gender = per.Gender,
                Nationality = per.Nationality,
                Email = request.AddDto.Email,
                Phone = request.AddDto.Telephone,
                IsDeleted = false,
                IsActive = false,
                BranchId = null  // Will be populated from Department
            };

            await _eventPublisher.PublishAsync("Employee", "CREATED", employeeData, ct);
            _logger.LogInformation("Employee created and event published: {EmployeeId}", data.Id);

            var res = new EmpAddRes { Id = data.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    // ? Helper method to ensure DateTime is UTC
    private DateTime EnsureUtc(DateTime date)
    {
        if (date.Kind == DateTimeKind.Utc)
            return date;

        if (date.Kind == DateTimeKind.Local)
            return date.ToUniversalTime();

        return DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
}

public class EmpAddStep2CmdHandler : IRequestHandler<EmpAddStep2Cmd, EmpAddRes>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmpModService _empModSer;

    public EmpAddStep2CmdHandler(IUnitOfWork uow, IEmpModService empModSer)
    {
        _uow = uow;
        _empModSer = empModSer;
    }

    public async Task<EmpAddRes> Handle(EmpAddStep2Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = new EmpAddRes { Id = request.AddDto.EmployeeId };
            var added = await _uow.Set<EmpGuarantor>().FirstOrDefaultAsync(x => x.EmployeeId == request.AddDto.EmployeeId, ct);
            if (added != null) { return res; }

            var now = DateTime.UtcNow;

            var address = new Address
            {
                Id = Guid.CreateVersion7(),
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
                Website = request.AddDto.Website,
                DateAdd = now,
                DateMod = now,
                IsDeleted = false
            };
            await _uow.Add(address, ct);

            var data = new EmpGuarantor
            {
                Id = Guid.CreateVersion7(),
                FirstName = request.AddDto.FirstName,
                MiddleName = request.AddDto.MiddleName,
                LastName = request.AddDto.LastName,
                Gender = request.AddDto.Gender,
                Nationality = request.AddDto.Nationality,
                Relation = request.AddDto.Relation,
                AddressId = address.Id,
                EmployeeId = request.AddDto.EmployeeId,
                DateAdd = now,
                DateMod = now,
                IsDeleted = false
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