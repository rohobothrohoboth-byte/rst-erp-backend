using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using MediatR;

namespace Cor.Module.Queries;

public class BranchCompListQry : IRequest<List<NameListDto>> { }
public class BranchCompByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
public class BranchAllNameQry : IRequest<List<NameListDto>> { }
public class BranchNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
public class DeptByBraQry : IRequest<List<BranchDeptList>> { public Guid Id { get; set; } }
public class DeptAllNameQry : IRequest<List<NameAmListDto>> { }
public class DeptNameByIdQry : IRequest<NameAmListDto?> { public Guid Id { get; set; } }
public class CompAllNameQry : IRequest<List<NameListDto>> { }
public class CompNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
public class FiscalYearAllNameQry : IRequest<List<NameListDto>> { }
public class FiscalYearNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
public class FiscalYearActiveQry : IRequest<List<NameListDto>> { }
public class PeriodAllNameQry : IRequest<List<NameListDto>> { }
public class PeriodNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }

public class BranchCompListQryHandler : IRequestHandler<BranchCompListQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public BranchCompListQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(BranchCompListQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Branch>().GetAll();
        var nameL = new List<NameListDto>();
        foreach (var data in res)
        {
            var comp = await _unitOfWork.Repository<Company>().GetById(data.CompId);
            if (comp == null) continue;
            var c = new NameListDto
            {
                Id = data.Id,
                Name = $"{data.Name} => {comp.Name}"
            };
            nameL.Add(c);
        }

        return nameL;
    }
}

public class BranchCompByIdQryHandler : IRequestHandler<BranchCompByIdQry, NameListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public BranchCompByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameListDto?> Handle(BranchCompByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Branch>().GetById(request.Id);
        if (res == null) { return null; }
        var comp = await _unitOfWork.Repository<Company>().GetById(res.CompId);
        if (comp == null) { return null; }

        var c = new NameListDto
        {
            Id = res.Id,
            Name = $"{res.Name} => {comp.Name}"
        };
        return c;
    }
}

public class BranchAllNameQryHandler : IRequestHandler<BranchAllNameQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public BranchAllNameQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(BranchAllNameQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Branch>().GetAll();

        return res.Select(data => new NameListDto { Id = data.Id, Name = data.Name }).ToList();
    }
}

public class BranchNameByIdQryHandler : IRequestHandler<BranchNameByIdQry, NameListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public BranchNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameListDto?> Handle(BranchNameByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Branch>().GetById(request.Id);
        if (res == null) { return null; }

        var c = new NameListDto
        {
            Id = res.Id,
            Name = res.Name
        };
        return c;
    }
}

public class DeptByBraQryHandler : IRequestHandler<DeptByBraQry, List<BranchDeptList>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeptByBraQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<BranchDeptList>> Handle(DeptByBraQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Department>().Find(c => c.BranchId == request.Id);
        var resL = new List<BranchDeptList>();
        var nData = res.ToList();
        if (nData.Count <= 0) return resL;
        foreach (var data in nData)
        {
            var bra = await _unitOfWork.Repository<Branch>().GetById(data.BranchId);
            if (bra == null) continue;
            var c = new BranchDeptList
            {
                Id = data.Id,
                BranchId = data.BranchId,
                Dept = data.Name,
                Branch = bra.Name
            };
            resL.Add(c);
        }

        return resL;
    }
}

public class DeptAllNameQryHandler : IRequestHandler<DeptAllNameQry, List<NameAmListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeptAllNameQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameAmListDto>> Handle(DeptAllNameQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Department>().GetAll();
        var nameL = new List<NameAmListDto>();

        foreach (var data in res)
        {
            var bra = await _unitOfWork.Repository<Branch>().GetById(data.BranchId);
            if (bra == null) continue;
            var c = new NameAmListDto
            {
                Id = data.Id,
                Name = data.Name,
                NameAm = bra.Name
            };
            nameL.Add(c);
        }

        return nameL;
    }
}

public class DeptNameByIdQryHandler : IRequestHandler<DeptNameByIdQry, NameAmListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeptNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameAmListDto?> Handle(DeptNameByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Department>().GetById(request.Id);
        if (res == null) { return null; }
        var bra = await _unitOfWork.Repository<Branch>().GetById(res.BranchId);
        if (bra == null) { return null; }

        var c = new NameAmListDto
        {
            Id = res.Id,
            Name = res.Name,
            NameAm = bra.Name
        };
        return c;
    }
}

public class CompAllNameQryHandler : IRequestHandler<CompAllNameQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CompAllNameQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(CompAllNameQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Company>().GetAll();
        var nameL = new List<NameListDto>();
        foreach (var data in res)
        {
            var c = new NameListDto
            {
                Id = data.Id,
                Name = data.Name
            };
            nameL.Add(c);
        }

        return nameL;
    }
}

public class CompNameByIdQryHandler : IRequestHandler<CompNameByIdQry, NameListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public CompNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameListDto?> Handle(CompNameByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Company>().GetById(request.Id);
        if (res == null) { return null; }

        var c = new NameListDto
        {
            Id = res.Id,
            Name = res.Name
        };
        return c;
    }
}

public class FiscalYearAllNameQryHandler : IRequestHandler<FiscalYearAllNameQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public FiscalYearAllNameQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(FiscalYearAllNameQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<FiscalYear>().GetAll();
        var nameL = new List<NameListDto>();
        foreach (var data in res)
        {
            var c = new NameListDto
            {
                Id = data.Id,
                Name = data.Name
            };
            nameL.Add(c);
        }

        return nameL;
    }
}

public class FiscalYearNameByIdQryHandler : IRequestHandler<FiscalYearNameByIdQry, NameListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public FiscalYearNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameListDto?> Handle(FiscalYearNameByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<FiscalYear>().GetById(request.Id);
        if (res == null) { return null; }

        var c = new NameListDto
        {
            Id = res.Id,
            Name = res.Name
        };
        return c;
    }
}

public class FiscalYearActiveQryHandler : IRequestHandler<FiscalYearActiveQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public FiscalYearActiveQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(FiscalYearActiveQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<FiscalYear>().Find(f => f.IsActive == "0" && f.DateEnd >= DateTime.UtcNow);
        var nameL = new List<NameListDto>();
        foreach (var data in res)
        {
            var c = new NameListDto
            {
                Id = data.Id,
                Name = data.Name
            };
            nameL.Add(c);
        }

        return nameL;
    }
}

public class PeriodAllNameQryHandler : IRequestHandler<PeriodAllNameQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PeriodAllNameQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(PeriodAllNameQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Period>().GetAll();
        var nameL = new List<NameListDto>();
        foreach (var data in res)
        {
            var c = new NameListDto
            {
                Id = data.Id,
                Name = data.Name
            };
            nameL.Add(c);
        }

        return nameL;
    }
}

public class PeriodNameByIdQryHandler : IRequestHandler<PeriodNameByIdQry, NameListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PeriodNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameListDto?> Handle(PeriodNameByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Period>().GetById(request.Id);
        if (res == null) { return null; }

        var c = new NameListDto
        {
            Id = res.Id,
            Name = res.Name
        };
        return c;
    }
}