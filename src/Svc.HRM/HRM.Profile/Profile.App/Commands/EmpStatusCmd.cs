using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpTermCmd : IRequest<EmpModRes> { public Guid Id { get; set; } }
public class EmpStByCmd : IRequest<EmpModRes> { public Guid Id { get; set; } }
public class EmpSuspCmd : IRequest<EmpModRes> { public Guid Id { get; set; } }
public class EmpRetiCmd : IRequest<EmpModRes> { public Guid Id { get; set; } }
public class EmpAppCmd : IRequest<EmpModRes> { public EmpRevDto Dto { get; set; } = default!; }



public class EmpTermHandler(IUnitOfWork _uow) : IRequestHandler<EmpTermCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpTermCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var emp = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (emp == null) { throw new DomainException("Employee data NOT AVAILABLE for changing status.", 404); }

            emp.EmpState = BoolToStr.EnumToString(EmpState.Term);
            await _uow.Update(emp);
            await _uow.Commit(ct);

            var res = new EmpModRes { Id = request.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpStByHandler(IUnitOfWork _uow) : IRequestHandler<EmpStByCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpStByCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var emp = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (emp == null) { throw new DomainException("Employee data NOT AVAILABLE for changing status.", 404); }

            emp.EmpState = BoolToStr.EnumToString(EmpState.StandBy);
            await _uow.Update(emp);
            await _uow.Commit(ct);

            var res = new EmpModRes { Id = request.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpSuspHandler(IUnitOfWork _uow) : IRequestHandler<EmpSuspCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpSuspCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var emp = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (emp == null) { throw new DomainException("Employee data NOT AVAILABLE for changing status.", 404); }

            emp.EmpState = BoolToStr.EnumToString(EmpState.Sus);
            await _uow.Update(emp);
            await _uow.Commit(ct);

            var res = new EmpModRes { Id = request.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpRetiHandler(IUnitOfWork _uow) : IRequestHandler<EmpRetiCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpRetiCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var emp = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (emp == null) { throw new DomainException("Employee data NOT AVAILABLE for changing status.", 404); }

            emp.EmpState = BoolToStr.EnumToString(EmpState.Retire);
            await _uow.Update(emp);
            await _uow.Commit(ct);

            var res = new EmpModRes { Id = request.Id };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class EmpAppHandler(IUnitOfWork _uow) : IRequestHandler<EmpAppCmd, EmpModRes>
{
    public async Task<EmpModRes> Handle(EmpAppCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var emp = await _uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id, ct);
            if (emp == null) { throw new DomainException("Employee data NOT AVAILABLE for reviewing.", 404); }

            if (request.Dto.Decision == true)
            {
                emp.EmpState = BoolToStr.EnumToString(EmpState.Active);
                await _uow.Update(emp);
            }
            else
            {
                emp.EmpState = BoolToStr.EnumToString(EmpState.Rej);
                await _uow.Update(emp);
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