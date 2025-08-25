using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using Cor.Domain.Enums;
using MediatR;

namespace Cor.App.Commands.Dept;

public class ModDeptCmd : IRequest<DeptListDto>
{
    public EdtDeptDto EdtDeptDto { get; set; } = default!;
}

public class ModDeptCmdHandler : IRequestHandler<ModDeptCmd, DeptListDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ModDeptCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<DeptListDto> Handle(ModDeptCmd request, CancellationToken cancellationToken)
    {
        var oldDept = await _unitOfWork.Repository<Department>().GetById(request.EdtDeptDto.Id);

        if (oldDept == null)
        {
            throw new KeyNotFoundException($"DEPARTMENT with Id {request.EdtDeptDto.Id} NOT FOUND.");
        }

        oldDept.Name = request.EdtDeptDto.Name;
        oldDept.NameAm = request.EdtDeptDto.NameAm;
        oldDept.DeptStat = request.EdtDeptDto.DeptStat;
        oldDept.BranchId = request.EdtDeptDto.BranchId;

        await _unitOfWork.Begin();

        try
        {
            var dep = await _unitOfWork.Repository<Department>().Update(oldDept);
            await _unitOfWork.Commit();

            var dept = await _unitOfWork.Repository<Department>().GetById(dep.Id);
            var res = new DeptListDto();
            if (dept == null) return res;
            var branch = await _unitOfWork.Repository<Branch>().GetById(dept.BranchId);
            if (branch == null) return res;

            res.Id = dept.Id;
            res.Name = dept.Name;
            res.NameAm = dept.NameAm;
            res.DeptStat = ((DeptStat)Enum.Parse(typeof(DeptStat), dept.DeptStat)).ToDisplayName();
            res.Branch = branch.Name;
            res.BranchAm = branch.NameAm;
            res.IsDeleted = dept.IsDeleted;
            res.DateAdd = dept.DateAdd;
            res.DateMod = dept.DateMod;
            res.RowVersion = Convert.ToBase64String(dept.RowVersion);
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}