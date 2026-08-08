// Svc.Auth/Commands/PerMenuCmd.cs

using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;

namespace Svc.Auth.Commands;

public class PerMenuAddCmd : IRequest<PerMenuListDto>
{
    public PerMenuAddDto AddDto { get; set; } = default!;
}

public class PerMenuModCmd : IRequest<PerMenuListDto>
{
    public PerMenuModDto ModDto { get; set; } = default!;
}

// Remove the duplicate - keep only ONE definition
public class PerMenuDelCmd : IRequest<Unit>  // Change to IRequest<Unit>
{
    public Guid Id { get; set; }
}

// Add the handler with proper return type
public class PerMenuAddCmdHandler : IRequestHandler<PerMenuAddCmd, PerMenuListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PerMenuAddCmdHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<PerMenuListDto> Handle(PerMenuAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            Guid? parentId = null;
            if (request.AddDto.IsChild)
            {
                var pa = await _med.Send(new PerMenuByKeyQry { Key = request.AddDto.ParentKey }, ct);
                if (pa != null)
                {
                    parentId = pa.Id;
                }
                else
                {
                    throw new DomainException($"PARENT MENU with Parent key {request.AddDto.ParentKey} NOT FOUND.");
                }
            }

            var data = new PerMenu
            {
                PerModuleId = request.AddDto.PerModuleId,
                Key = request.AddDto.Key,
                Label = request.AddDto.Label,
                Path = request.AddDto.Path,
                Icon = request.AddDto.Icon,
                IsChild = request.AddDto.IsChild,
                ParentId = parentId,
                Order = request.AddDto.Order
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var response = await _med.Send(new PerMenuByIdQry { Id = data.Id }, ct);
            if (response == null)
            {
                return new PerMenuListDto();
            }
            return response;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PerMenuModCmdHandler : IRequestHandler<PerMenuModCmd, PerMenuListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PerMenuModCmdHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<PerMenuListDto> Handle(PerMenuModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PerMenu>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null)
            {
                throw new DomainException($"MENU PERMISSION with Id {request.ModDto.Id} NOT FOUND.");
            }

            Guid? parentId = null;
            if (request.ModDto.IsChild)
            {
                var pa = await _med.Send(new PerMenuByKeyQry { Key = request.ModDto.ParentKey }, ct);
                if (pa != null)
                {
                    parentId = pa.Id;
                }
            }
            oldData.PerModuleId = request.ModDto.PerModuleId;
            oldData.Key = request.ModDto.Key;
            oldData.Label = request.ModDto.Label;
            oldData.Path = request.ModDto.Path;
            oldData.Icon = request.ModDto.Icon;
            oldData.IsChild = request.ModDto.IsChild;
            oldData.ParentId = parentId;
            oldData.Order = request.ModDto.Order;
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var response = await _med.Send(new PerMenuByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null)
            {
                return new PerMenuListDto();
            }
            return response;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}


public class PerMenuDelCmdHandler : IRequestHandler<PerMenuDelCmd, Unit>
{
    private readonly IUnitOfWork _uow;

    public PerMenuDelCmdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // Change return type from Task to Task<Unit>
    public async Task<Unit> Handle(PerMenuDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PerMenu>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
            if (data == null)
            {
                throw new DomainException($"MENU PERMISSION with id [{request.Id}] NOT FOUND.");
            }

            // Soft delete instead of hard delete
            data.IsDeleted = true;
            data.DateMod = DateTime.UtcNow;
            await _uow.Update(data);
            await _uow.Commit(ct);

            return Unit.Value;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}


public class UpdateMenuPermissionCmd : IRequest<bool>
{
    public UpdateMenuPermissionDto Dto { get; set; } = default!;
}

public class UpdateMenuPermissionHandler : IRequestHandler<UpdateMenuPermissionCmd, bool>
{
    private readonly IDapperHelper _dapper;

    public UpdateMenuPermissionHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<bool> Handle(UpdateMenuPermissionCmd request, CancellationToken cancellationToken)
    {
        try
        {
            // Convert parentKey to ParentId
            Guid? parentId = null;
            if (!string.IsNullOrEmpty(request.Dto.ParentKey))
            {
                const string getParentSql = @"
                    SELECT ""Id"" FROM ""PerMenu""
                    WHERE ""Key"" = @ParentKey AND ""IsDeleted"" = false";

                parentId = await _dapper.QueryFirstOrDefaultAsync<Guid?>(
                    getParentSql,
                    new { ParentKey = request.Dto.ParentKey },
                    cancellationToken);
            }

            const string sql = @"
                UPDATE ""PerMenu""
                SET
                    ""PerModuleId"" = @PerModuleId,
                    ""Key"" = @Key,
                    ""Label"" = @Label,
                    ""Path"" = @Path,
                    ""Icon"" = @Icon,
                    ""IsChild"" = @IsChild,
                    ""Order"" = @Order,
                    ""ParentId"" = @ParentId,
                    ""DateMod"" = NOW()
                WHERE ""Id"" = @Id";

            var parameters = new
            {
                request.Dto.Id,
                request.Dto.PerModuleId,
                request.Dto.Key,
                request.Dto.Label,
                request.Dto.Path,
                request.Dto.Icon,
                request.Dto.IsChild,
                request.Dto.Order,
                ParentId = parentId
            };

            var rowsAffected = await _dapper.ExecuteAsync(sql, parameters, cancellationToken);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new DomainException($"Failed to update menu permission: {ex.Message}");
        }
    }
}

