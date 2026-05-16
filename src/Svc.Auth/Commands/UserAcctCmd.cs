using Helpers;
using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Services;

namespace Svc.Auth.Commands;

public class PwdChangeCmd : IRequest<OpResult?> { public PwdChgDto Dto { get; set; } = default!; }
public class UserDelCmd : IRequest<OpResult?> { public string Id { get; set; } = default!; }



public class PwdChangeHandler(IUnitOfWork _uow, IUserAcctService _iAccountService) : IRequestHandler<PwdChangeCmd, OpResult?>
{
    public async Task<OpResult?> Handle(PwdChangeCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = await _iAccountService.ChnagePassword(request.Dto, ct);
            //var res = await _iAccountService.ResetPassword(request.Dto.Id, request.Dto.NewPwd, ct);
            if (!res.IsSuccess) { throw new DomainException("UNABLE to CHANGE User's Password."); }
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

public class UserDelHandler(IUnitOfWork _uow, IUserAcctService _iAccountService) : IRequestHandler<UserDelCmd, OpResult?>
{
    public async Task<OpResult?> Handle(UserDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = await _iAccountService.DeleteAccount(request.Id, ct);
            if (!res.IsSuccess) { throw new DomainException("UNABLE to DELETE selected USER'S account."); }
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