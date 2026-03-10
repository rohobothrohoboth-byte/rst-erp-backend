using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Module.Commands;

public class AddDeptCmd : IRequest<DeptListDto> { public AddDeptDto AddDto { get; set; } = default!; }
public class ModDeptCmd : IRequest<DeptListDto> { public EdtDeptDto ModDto { get; set; } = default!; }
public class DelDeptCmd : IRequest { public Guid Id { get; set; } }

public class AddDeptCmdHandler : IRequestHandler<AddDeptCmd, DeptListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public AddDeptCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<DeptListDto> Handle(AddDeptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var dep = new Department
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                DeptStat = "0",
                BranchId = request.AddDto.BranchId
            };
            await _uow.Add(dep, ct);
            await _uow.Commit(ct);

            var res = new DeptListDto();
            var response = await _med.Send(new DeptByIdQry { Id = dep.Id }, ct);
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

public class ModDeptCmdHandler : IRequestHandler<ModDeptCmd, DeptListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public ModDeptCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<DeptListDto> Handle(ModDeptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Department>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id);
            if (oldData == null) { throw new DomainException($"DEPARTMENT with id [{request.ModDto.Id}] NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.DeptStat = request.ModDto.DeptStat;
            oldData.BranchId = request.ModDto.BranchId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new DeptListDto();
            var response = await _med.Send(new DeptByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback();
            throw;
        }
    }
}

public class DelDeptCmdHandler : IRequestHandler<DelDeptCmd>
{
    private readonly IUnitOfWork _uow;
    public DelDeptCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(DelDeptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Department>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"DEPARTMENT with id [{request.Id}] NOT FOUND."); }
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