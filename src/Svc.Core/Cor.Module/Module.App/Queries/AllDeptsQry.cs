using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;
using Module.Domain.Enums;

namespace Module.App.Queries;
public class AllDeptsQry : IRequest<List<DeptListDto>> { }

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
                DeptStat = ((DeptStat)Enum.Parse(typeof(DeptStat), dept.DeptStat)).ToDisplayName(),
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