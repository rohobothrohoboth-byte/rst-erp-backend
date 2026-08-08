using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpEduAddCmd : IRequest<EmpEduListDto> { public EmpEduAddDto AddDto { get; set; } = default!; }
public class EmpEduModCmd : IRequest<EmpEduListDto> { public EmpEduModDto ModDto { get; set; } = default!; }
public class EmpEduRvwCmd : IRequest { public EmpRevDto Dto { get; set; } = default!; }
public class EmpEduRvwAllCmd : IRequest { public EmpRevDto Dto { get; set; } = default!; }
public class EmpEduDelCmd : IRequest { public Guid Id { get; set; } }



public class EmpEduAdd(IUnitOfWork _uow, IMediator _med) : IRequestHandler<EmpEduAddCmd, EmpEduListDto>
{
    public async Task<EmpEduListDto> Handle(EmpEduAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EmpEducation
            {
                EduLevel = request.AddDto.EduLevel,
                Institution = request.AddDto.Institution,
                FieldOfStudy = request.AddDto.FieldOfStudy,
                StartDate = request.AddDto.StartDate,
                EndDate = request.AddDto.EndDate,
                GPA = request.AddDto.GPA,
                Status = BoolToStr.EnumToString(ApprovalStatus.Pending),
                EmployeeId = request.AddDto.EmpId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new EmpEduListDto();
            var response = await _med.Send(new EmpEduByIdQry { Id = data.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpEduMod(IUnitOfWork _uow, IMediator _med) : IRequestHandler<EmpEduModCmd, EmpEduListDto>
{
    private static DateTime? GetNullableDate(DateTime date)
    {
        return date == DateTime.MinValue || date == default(DateTime)
            ? (DateTime?)null
            : date;
    }

    public async Task<EmpEduListDto> Handle(EmpEduModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EmpEducation>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException("EMPLOYEE'S EDUCATION with given parameter NOT FOUND."); }

            // ✅ Update all fields
            oldData.EduLevel = request.ModDto.EduLevel;
            oldData.Institution = request.ModDto.Institution;
            oldData.FieldOfStudy = request.ModDto.FieldOfStudy;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.EndDate = request.ModDto.EndDate;
            oldData.GPA = request.ModDto.GPA;
            oldData.Status = request.ModDto.Status;

            // ✅ Handle empty RowVersion
            if (!string.IsNullOrEmpty(request.ModDto.RowVersion))
            {
                try
                {
                    oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
                }
                catch (FormatException)
                {
                    // If parsing fails, use current row version
                }
            }

            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EmpEduListDto();
            var response = await _med.Send(new EmpEduByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpEduRvw(IUnitOfWork _uow) : IRequestHandler<EmpEduRvwCmd>
{
    public async Task Handle(EmpEduRvwCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var edu = await _uow.Set<EmpEducation>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id, ct);
            if (edu == null) { throw new DomainException("EMPLOYEE'S EDUCATION NOT AVAILABLE for reviewing.", 404); }

            if (request.Dto.Decision == true)
            {
                edu.Status = BoolToStr.EnumToString(ApprovalStatus.Approved);
                await _uow.Update(edu);
            }
            else
            {
                edu.Status = BoolToStr.EnumToString(ApprovalStatus.Rejected);
                await _uow.Update(edu);
            }

            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpEduRvwAll(IUnitOfWork _uow) : IRequestHandler<EmpEduRvwAllCmd>
{
    public async Task Handle(EmpEduRvwAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ApprovalStatus.Pending);
            var edu = await _uow.Set<EmpEducation>().Where(x => x.EmployeeId == request.Dto.Id && x.Status == stat).ToListAsync(ct);
            if (edu.Count <= 0) { throw new DomainException("EMPLOYEE'S EDUCATIONS NOT AVAILABLE for reviewing.", 404); }

            if (request.Dto.Decision == true)
            {
                foreach (var data in edu)
                {
                    data.Status = BoolToStr.EnumToString(ApprovalStatus.Approved);
                    await _uow.Update(data);
                }
            }
            else
            {
                foreach (var data in edu)
                {
                    data.Status = BoolToStr.EnumToString(ApprovalStatus.Rejected);
                    await _uow.Update(data);
                }
            }

            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpEduDel(IUnitOfWork _uow) : IRequestHandler<EmpEduDelCmd>
{
    public async Task Handle(EmpEduDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EmpEducation>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException("EMPLOYEE'S EDUCATION with given parameter NOT FOUND."); }
            await _uow.Remove(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}