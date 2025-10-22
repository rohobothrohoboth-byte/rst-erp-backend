using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Cor.Module.Models.Enums;
using MediatR;

namespace Cor.Module.Queries;

public class AllDeptsQry : IRequest<List<DeptListDto>> { }
public class DeptByIdQry : IRequest<DeptListDto?> { public Guid Id { get; set; } }

public class AllDeptsQryHandler : IRequestHandler<AllDeptsQry, List<DeptListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllDeptsQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<DeptListDto>> Handle(AllDeptsQry request, CancellationToken cancellationToken)
    {
        var depts = await _unitOfWork.Repository<Department>().GetAll();
        var deptL = new List<DeptListDto>();
        foreach (var dept in depts)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetById(dept.BranchId);
            if (branch == null) continue;
            var c = new DeptListDto
            {
                Id = dept.Id,
                Name = dept.Name,
                NameAm = dept.NameAm,
                DeptStat = dept.DeptStat,
                DeptStatStr = ((DeptStat)Enum.Parse(typeof(DeptStat), dept.DeptStat)).ToDisplayName(),
                Branch = branch.Name,
                BranchAm = branch.NameAm,
                IsDeleted = dept.IsDeleted,
                DateAdd = dept.DateAdd,
                DateMod = dept.DateMod,
                RowVersion = Convert.ToBase64String(dept.RowVersion)
            };
            deptL.Add(c);
        }

        return deptL;
    }
}

public class DeptByIdQryHandler : IRequestHandler<DeptByIdQry, DeptListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeptByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<DeptListDto?> Handle(DeptByIdQry request, CancellationToken cancellationToken)
    {
        var dept = await _unitOfWork.Repository<Department>().GetById(request.Id);
        if (dept == null)
        {
            return null;
        }

        var branch = await _unitOfWork.Repository<Branch>().GetById(dept.BranchId);
        if (branch == null) return null;
        var c = new DeptListDto
        {
            Id = dept.Id,
            Name = dept.Name,
            NameAm = dept.NameAm,
            DeptStat = dept.DeptStat,
            DeptStatStr = ((DeptStat)Enum.Parse(typeof(DeptStat), dept.DeptStat)).ToDisplayName(),
            Branch = branch.Name,
            BranchAm = branch.NameAm,
            IsDeleted = dept.IsDeleted,
            DateAdd = dept.DateAdd,
            DateMod = dept.DateMod,
            RowVersion = Convert.ToBase64String(dept.RowVersion)
        };
        return c;
    }
}