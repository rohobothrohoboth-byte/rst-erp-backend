using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class RoleAllQry : IRequest<List<RoleListDto>> { }
public class RoleByIdQry : IRequest<RoleListDto?> { public string Id { get; set; } = default!; }

public class RoleAllQryHandler : IRequestHandler<RoleAllQry, List<RoleListDto>>
{
    private readonly RoleManager<AppRole> _roleManager;
    public RoleAllQryHandler(RoleManager<AppRole> roleManager) { _roleManager = roleManager; }
    public Task<List<RoleListDto>> Handle(RoleAllQry request, CancellationToken cancellationToken)
    {
        var dbData = _roleManager.Roles.Where(r => r.Name != "admin").AsNoTracking().ToList();
        var dataL = dbData.Select(data => new RoleListDto { Id = data.Id, Role = data.Desc }).ToList();
        return Task.FromResult(dataL);
    }
}

public class RoleByIdQryHandler : IRequestHandler<RoleByIdQry, RoleListDto?>
{
    private readonly RoleManager<AppRole> _roleManager;
    public RoleByIdQryHandler(RoleManager<AppRole> roleManager) { _roleManager = roleManager; }
    public async Task<RoleListDto?> Handle(RoleByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _roleManager.FindByIdAsync(request.Id);
        if (data == null) { return null; }
        var c = new RoleListDto { Id = data.Id, Role = data.Desc };
        return c;
    }
}