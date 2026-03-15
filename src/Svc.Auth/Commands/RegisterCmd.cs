using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

public class RegStep1Cmd : IRequest<RegRes?> { public RegStep1 Reg { get; set; } = default!; }
public class RegStep2Cmd : IRequest<RegRes> { public RegStep2 Reg { get; set; } = default!; }
public class RegStep3Cmd : IRequest<RegRes> { public RegStep3 Reg { get; set; } = default!; }



public class RegStep1CmdHandler : IRequestHandler<RegStep1Cmd, RegRes?>
{
    private readonly IUnitOfWork _uow;
    private readonly IHrmProfileClient _gRPC_HrmPro;
    private readonly UserManager<AppUser> _uManager;
    private readonly RoleManager<AppRole> _rManager;

    public RegStep1CmdHandler(IUnitOfWork uow, IHrmProfileClient gRPC_HrmPro, UserManager<AppUser> uManager, RoleManager<AppRole> rManager)
    {
        _uow = uow;
        _gRPC_HrmPro = gRPC_HrmPro;
        _uManager = uManager;
        _rManager = rManager;
    }

    public async Task<RegRes?> Handle(RegStep1Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var selPer = request.Reg.PerModules;
            if (selPer.Count <= 0) { throw new DomainException($"NO MODULES selected"); }

            var empId = request.Reg.EmployeeId;
            var code = (await _gRPC_HrmPro.GetEmpCode(empId.ToString())).Code;
            if (code == null) { throw new DomainException($"UNABLE to FIND employee with Id {request.Reg.EmployeeId}!!"); }

            //var usr = await _uManager.FindByNameAsync(code);
            //if (usr != null) { throw new DomainException($"USER ACCOUNT ALREADY CREATED for selected employee!!"); }

            var role = await _rManager.FindByIdAsync(request.Reg.RoleId);
            if (role == null) { throw new DomainException($"UNABLE to FIND Description for selected role!!"); }

            var userid = "";
            var usr = await _uManager.FindByNameAsync(code);
            if (usr == null)
            {
                var usrPw = request.Reg.Password;
                var user = new AppUser
                {
                    EmployeeId = empId,
                    IsActive = true,
                    UserName = code,
                    Email = code,
                    EmailConfirmed = true,
                };

                var createResult = await _uManager.CreateAsync(user, usrPw);
                if (!createResult.Succeeded) { throw new DomainException($"UNABLE to Create USER ACCOUNT!!"); }

                await _uManager.AddToRoleAsync(user, role.Name!);
                userid = user.Id;
            }
            else
            {
                userid = usr!.Id;
            }            

            foreach (var per in selPer)
            {
                var added = await _uow.Set<UserPerModule>().FirstOrDefaultAsync(p => p.UserId == userid && p.PerModuleId == per, ct);
                if (added == null)
                {
                    var userMod = new UserPerModule
                    {
                        UserId = userid,
                        PerModuleId = per
                    };
                    await _uow.Add(userMod, ct);
                }
            }
            await _uow.Commit(ct);

            var res = new RegRes { UserId = userid };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class RegStep2CmdHandler : IRequestHandler<RegStep2Cmd, RegRes?>
{
    private readonly IUnitOfWork _uow;
    public RegStep2CmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task<RegRes?> Handle(RegStep2Cmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var selPer = request.Reg.PerMenus;
            if (selPer.Count <= 0) { throw new DomainException($"NO MENU Permission selected"); }

            var userId = request.Reg.UserId;
            foreach (var per in selPer)
            {
                var added = await _uow.Set<UserPerMenu>().FirstOrDefaultAsync(p => p.UserId == userId && p.PerMenuId == per, ct);
                if (added == null)
                {
                    var userMod = new UserPerMenu
                    {
                        UserId = userId,
                        PerMenuId = per
                    };
                    await _uow.Add(userMod, ct);
                }
            }
            await _uow.Commit(ct);

            var res = new RegRes { UserId = userId };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class RegStep3CmdHandler : IRequestHandler<RegStep3Cmd, RegRes?>
{
    private readonly IUnitOfWork _uow;
    public RegStep3CmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task<RegRes?> Handle(RegStep3Cmd request, CancellationToken ct)
    {
        await _uow.Begin();
        try
        {
            var selPer = request.Reg.PerAccess;
            if (selPer.Count <= 0) { throw new DomainException($"NO ACCESS Permission selected"); }

            var userId = request.Reg.UserId;
            foreach (var per in selPer)
            {
                var added = await _uow.Set<UserPerApi>().FirstOrDefaultAsync(p => p.UserId == userId && p.PerApiId == per, ct);
                if (added == null)
                {
                    var userMod = new UserPerApi
                    {
                        UserId = userId,
                        PerApiId = per
                    };
                    await _uow.Add(userMod, ct);
                }
            }
            await _uow.Commit(ct);

            var res = new RegRes { UserId = userId };
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}