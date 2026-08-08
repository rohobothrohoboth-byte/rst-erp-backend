using MediatR;
using Svc.Auth.Models.Dtos;
using Common;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace Svc.Auth.Queries.Setup;

public class GetSetupDataQry : IRequest<SetupDataDto> { }

public class SetupDataDto
{
    public List<ModuleDto> Modules { get; set; } = new();
    public List<RoleDto> Roles { get; set; } = new();
    public List<string> JobGrades { get; set; } = new();
}

public class ModuleDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class RoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GetSetupDataHandler : IRequestHandler<GetSetupDataQry, SetupDataDto>
{
    private readonly IUnitOfWork _uow;

    public GetSetupDataHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<SetupDataDto> Handle(GetSetupDataQry request, CancellationToken ct)
    {
        var dto = new SetupDataDto();

        // Get modules
        var modules = await _uow.Set<PerModule>()
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.Order)
            .ToListAsync(ct);

        dto.Modules = modules.Select(m => new ModuleDto
        {
            Key = m.Key,
            Name = m.Desc
        }).ToList();

        // Get roles
        var roles = await _uow.Set<AppRole>()
            .ToListAsync(ct);

        dto.Roles = roles.Select(r => new RoleDto
        {
            Name = r.Name ?? string.Empty,
            Description = r.Desc
        }).ToList();

        // Job grades
        dto.JobGrades = new List<string>
        {
            "Entry Level", "Junior", "Mid Level", "Senior", "Manager", "Director", "Executive"
        };

        return dto;
    }
}