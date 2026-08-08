using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpExpAddCmd : IRequest<EmpExpListDto> { public EmpExpAddDto AddDto { get; set; } = default!; }
public class EmpExpModCmd : IRequest<EmpExpListDto> { public EmpExpModDto ModDto { get; set; } = default!; }
public class EmpExpRvwCmd : IRequest { public EmpRevDto Dto { get; set; } = default!; }
public class EmpExpRvwAllCmd : IRequest { public EmpRevDto Dto { get; set; } = default!; }
public class EmpExpDelCmd : IRequest { public Guid Id { get; set; } }



public class EmpExpAdd(IUnitOfWork _uow, IMediator _med) : IRequestHandler<EmpExpAddCmd, EmpExpListDto>
{
    public async Task<EmpExpListDto> Handle(EmpExpAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EmpExperience
            {
                Company = request.AddDto.Company,
                PosTitle = request.AddDto.PosTitle,
                Location = request.AddDto.Location,
                StartDate = request.AddDto.StartDate,
                EndDate = request.AddDto.EndDate,
                Respo = request.AddDto.Respo,
                Status = BoolToStr.EnumToString(ApprovalStatus.Pending),
                EmployeeId = request.AddDto.EmpId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new EmpExpListDto();
            var response = await _med.Send(new EmpExpByIdQry { Id = data.Id }, ct);
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

public class EmpExpMod(IUnitOfWork _uow, IMediator _med) : IRequestHandler<EmpExpModCmd, EmpExpListDto>
{
    public async Task<EmpExpListDto> Handle(EmpExpModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EmpExperience>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException("EMPLOYEE'S EXPERIENCE with given parameter NOT FOUND."); }

            // ✅ Update all fields including the status from the request
            oldData.Company = request.ModDto.Company;
            oldData.PosTitle = request.ModDto.PosTitle;
            oldData.Location = request.ModDto.Location;
            oldData.Respo = request.ModDto.Respo;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.EndDate = request.ModDto.EndDate;

            // ✅ FIXED: Use the status from the request, not hardcoded "Pending"
            oldData.Status = request.ModDto.Status; // This will be "1" for Approved, "2" for Rejected

            // ✅ FIXED: Handle empty RowVersion
            if (!string.IsNullOrEmpty(request.ModDto.RowVersion))
            {
                try
                {
                    oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
                }
                catch (FormatException)
                {
                    // If parsing fails, use current row version (don't update it)
                }
            }

            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EmpExpListDto();
            var response = await _med.Send(new EmpExpByIdQry { Id = request.ModDto.Id }, ct);
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

public class EmpExpRvw(IUnitOfWork _uow) : IRequestHandler<EmpExpRvwCmd>
{
    public async Task Handle(EmpExpRvwCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var exp = await _uow.Set<EmpExperience>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id, ct);
            if (exp == null) { throw new DomainException("EMPLOYEE'S EXPERIANCE NOT AVAILABLE for reviewing.", 404); }

            if (request.Dto.Decision == true)
            {
                exp.Status = BoolToStr.EnumToString(ApprovalStatus.Approved);
                await _uow.Update(exp);
            }
            else
            {
                exp.Status = BoolToStr.EnumToString(ApprovalStatus.Rejected);
                await _uow.Update(exp);
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

public class EmpExpRvwAll(IUnitOfWork _uow) : IRequestHandler<EmpExpRvwAllCmd>
{
    public async Task Handle(EmpExpRvwAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ApprovalStatus.Pending);
            var exp = await _uow.Set<EmpExperience>().Where(x => x.EmployeeId == request.Dto.Id && x.Status == stat).ToListAsync(ct);
            if (exp.Count <= 0) { throw new DomainException("EMPLOYEE'S EXPERIANCES NOT AVAILABLE for reviewing.", 404); }

            if (request.Dto.Decision == true)
            {
                foreach (var data in exp)
                {
                    data.Status = BoolToStr.EnumToString(ApprovalStatus.Approved);
                    await _uow.Update(data);
                }
            }
            else
            {
                foreach (var data in exp)
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

public class EmpExpDel(IUnitOfWork _uow) : IRequestHandler<EmpExpDelCmd>
{
    public async Task Handle(EmpExpDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EmpExperience>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException("EMPLOYEE'S EXPERIENCE with given parameter NOT FOUND."); }
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