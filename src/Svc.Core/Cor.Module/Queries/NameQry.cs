using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using MediatR;

namespace Cor.Module.Queries;

public class BranchCompListQry : IRequest<List<NameListDto>> { }
public class DeptAllNameQry : IRequest<List<NameListDto>> { }
public class DeptNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
public class CompAllNameQry : IRequest<List<NameListDto>> { }
public class CompNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
public class FiscalYearAllNameQry : IRequest<List<NameListDto>> { }
public class FiscalYearNameByIdQry : IRequest<NameListDto?> { public Guid Id { get; set; } }
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

public class DeptAllNameQryHandler : IRequestHandler<DeptAllNameQry, List<NameListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeptAllNameQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameListDto>> Handle(DeptAllNameQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Department>().GetAll();
        var nameL = new List<NameListDto>();
        foreach (var data in res)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetById(data.BranchId);
            if (branch == null) continue;
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

public class DeptNameByIdQryHandler : IRequestHandler<DeptNameByIdQry, NameListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeptNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameListDto?> Handle(DeptNameByIdQry request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Repository<Department>().GetById(request.Id);
        if (res == null) { return null; }

        var branch = await _unitOfWork.Repository<Branch>().GetById(res.BranchId);
        if (branch == null) return null;
        var c = new NameListDto
        {
            Id = res.Id,
            Name = res.Name
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