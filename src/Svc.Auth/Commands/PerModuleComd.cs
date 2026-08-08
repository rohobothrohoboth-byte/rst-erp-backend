using MediatR;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Commands;

public class PerModuleAddCmd : IRequest<PerModuleListDto>
{
    public PerModuleAddDto AddDto { get; set; } = null!;
}

public class PerModuleModCmd : IRequest<PerModuleListDto>
{
    public PerModuleModDto ModDto { get; set; } = null!;
}

public class PerModuleDelCmd : IRequest<Unit>
{
    public Guid Id { get; set; }
}