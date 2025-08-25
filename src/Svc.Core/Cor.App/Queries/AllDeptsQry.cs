using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Queries;
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
                DeptStat = dept.DeptStat.ToString(),
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