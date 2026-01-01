using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leave.App.Queries;

public class LeaveTypeNameAllQry : IRequest<List<NameList>> { }
public class LeaveTypeNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class LeaveTypeNameAllQryHandler : IRequestHandler<LeaveTypeNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<NameList>> Handle(LeaveTypeNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeaveType>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Name }).ToList();
    }
}

public class LeaveTypeNameByIdQryHandler : IRequestHandler<LeaveTypeNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveTypeNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<NameList?> Handle(LeaveTypeNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<LeaveType>().GetById(request.Id);
        if (nData == null) { return null; }

        var c = new NameList { Id = nData.Id, Name = nData.Name };
        return c;
    }
}

