using Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Svc.Auth.Helpers;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Commands;

public class RegStep1Cmd : IRequest<RegRes?> { public RegStep1 Reg { get; set; } = default!; }
public class RegStep2Cmd : IRequest<RegRes> { public RegStep2 Reg { get; set; } = default!; }
public class RegStep3Cmd : IRequest<RegRes> { public RegStep3 Reg { get; set; } = default!; }


public class RegStep1CmdHandler : IRequestHandler<RegStep1Cmd, RegRes?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _gRPC_HrmPro;
    private readonly UserManager<AppUser> _uManager;
    private readonly RoleManager<AppRole> _rManager;

    public RegStep1CmdHandler(IUnitOfWork unitOfWork, IHrmProfileClient gRPC_HrmPro, UserManager<AppUser> uManager, RoleManager<AppRole> rManager)
    {
        _unitOfWork = unitOfWork;
        _gRPC_HrmPro = gRPC_HrmPro;
        _uManager = uManager;
        _rManager = rManager;
    }

    public async Task<RegRes?> Handle(RegStep1Cmd request, CancellationToken cancellationToken)
    {
        var selPer = request.Reg.PerModules;
        if (selPer.Count <= 0) { throw new DomainException($"NO MODULES selected"); }
        await _unitOfWork.Begin();

        try
        {
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
                //var userid = user.Id;
            }

            userid = usr!.Id;

            foreach (var per in selPer)
            {
                var added = await _unitOfWork.Repository<UserPerModule>().GetFoD(p => p.UserId == userid && p.PerModuleId == per);
                if (added == null)
                {
                    var userMod = new UserPerModule
                    {
                        UserId = userid,
                        PerModuleId = per
                    };
                    await _unitOfWork.Repository<UserPerModule>().Add(userMod);
                }
            }
            await _unitOfWork.Commit();

            var res = new RegRes { UserId = userid };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class RegStep2CmdHandler : IRequestHandler<RegStep2Cmd, RegRes?>
{
    private readonly IUnitOfWork _unitOfWork;
    public RegStep2CmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<RegRes?> Handle(RegStep2Cmd request, CancellationToken cancellationToken)
    {
        var selPer = request.Reg.PerMenus;
        if (selPer.Count <= 0) { throw new DomainException($"NO MENU Permission selected"); }
        await _unitOfWork.Begin();

        try
        {
            var userId = request.Reg.UserId;
            foreach (var per in selPer)
            {
                var added = await _unitOfWork.Repository<UserPerMenu>().GetFoD(p => p.UserId == userId && p.PerMenuId == per);
                if (added == null)
                {
                    var userMod = new UserPerMenu
                    {
                        UserId = userId,
                        PerMenuId = per
                    };
                    await _unitOfWork.Repository<UserPerMenu>().Add(userMod);
                }
            }
            await _unitOfWork.Commit();

            var res = new RegRes { UserId = userId };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class RegStep3CmdHandler : IRequestHandler<RegStep3Cmd, RegRes?>
{
    private readonly IUnitOfWork _unitOfWork;
    public RegStep3CmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<RegRes?> Handle(RegStep3Cmd request, CancellationToken cancellationToken)
    {
        var selPer = request.Reg.PerAccess;
        if (selPer.Count <= 0) { throw new DomainException($"NO ACCESS Permission selected"); }
        await _unitOfWork.Begin();

        try
        {
            var userId = request.Reg.UserId;
            foreach (var per in selPer)
            {
                var added = await _unitOfWork.Repository<UserPerApi>().GetFoD(p => p.UserId == userId && p.PerApiId == per);
                if (added == null)
                {
                    var userMod = new UserPerApi
                    {
                        UserId = userId,
                        PerApiId = per
                    };
                    await _unitOfWork.Repository<UserPerApi>().Add(userMod);
                }                
            }
            await _unitOfWork.Commit();

            var res = new RegRes { UserId = userId };
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}