using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Queries;
public class DeptByIdQry : IRequest<DeptListDto?>
{
    public Guid Id { get; set; }
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
            DeptStat = dept.DeptStat.ToString(),
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