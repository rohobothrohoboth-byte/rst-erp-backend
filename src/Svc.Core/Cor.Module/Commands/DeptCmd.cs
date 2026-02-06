using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Queries;
using Helpers;
using MediatR;

namespace Cor.Module.Commands;

public class AddDeptCmd : IRequest<DeptListDto> { public AddDeptDto AddDto { get; set; } = default!; }
public class ModDeptCmd : IRequest<DeptListDto> { public EdtDeptDto ModDto { get; set; } = default!; }
public class DelDeptCmd : IRequest { public Guid Id { get; set; } }

public class AddDeptCmdHandler : IRequestHandler<AddDeptCmd, DeptListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddDeptCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<DeptListDto> Handle(AddDeptCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var dep = new Department
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                DeptStat = "0",
                BranchId = request.AddDto.BranchId
            };
            await _unitOfWork.Repository<Department>().Add(dep);
            await _unitOfWork.Commit();

            var res = new DeptListDto();
            var response = await _med.Send(new DeptByIdQry { Id = dep.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class ModDeptCmdHandler : IRequestHandler<ModDeptCmd, DeptListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModDeptCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<DeptListDto> Handle(ModDeptCmd request, CancellationToken cancellationToken)
    {
        var oldDept = await _unitOfWork.Repository<Department>().GetById(request.ModDto.Id);
        if (oldDept == null) { throw new DomainException($"DEPARTMENT with id [{request.ModDto.Id}] NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldDept.Name = request.ModDto.Name;
            oldDept.NameAm = request.ModDto.NameAm;
            oldDept.DeptStat = request.ModDto.DeptStat;
            oldDept.BranchId = request.ModDto.BranchId;
            var dep = await _unitOfWork.Repository<Department>().Update(oldDept);
            await _unitOfWork.Commit();

            var res = new DeptListDto();
            var response = await _med.Send(new DeptByIdQry { Id = dep.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class DelDeptCmdHandler : IRequestHandler<DelDeptCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelDeptCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelDeptCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Department>().GetById(request.Id);
            if (data == null) { throw new DomainException($"DEPARTMENT with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<Department>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}