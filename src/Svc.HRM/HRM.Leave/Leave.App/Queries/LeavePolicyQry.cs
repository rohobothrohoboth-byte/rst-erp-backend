using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeavePolicyAllQry : IRequest<List<LeavePolicyListDto>> { }
public class LeavePolicyByIdQry : IRequest<LeavePolicyListDto?> { public Guid Id { get; set; } }

public class LeavePolicyAllQryHandler : IRequestHandler<LeavePolicyAllQry, List<LeavePolicyListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public LeavePolicyAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<LeavePolicyListDto>> Handle(LeavePolicyAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<LeavePolicy>().GetAll();
        var dataL = new List<LeavePolicyListDto>();
        var lvtL = await _unitOfWork.Repository<LeaveType>().GetAll();

        foreach (var data in dbData)
        {
            var lvt = lvtL.FirstOrDefault(t => t.Id == data.LeaveTypeId);
            var c = new LeavePolicyListDto
            {
                Id = data.Id,
                Name = data.Name,
                LeaveTypeId = data.LeaveTypeId,
                RequiresAttachment = data.RequiresAttachment,
                //MinDurPerReq = data.MinDurPerReq,
                //MaxDurPerReq = data.MaxDurPerReq,
                //HolidaysAsLeave = data.HolidaysAsLeave,
                LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
                RequiresAttachmentStr = data.RequiresAttachment.ToString(),
                //MinDurPerReqStr = $"{data.MinDurPerReq:#,##0.##} days",
                //MaxDurPerReqStr = $"{data.MaxDurPerReq:#,##0.##} days",
                //HolidaysAsLeaveStr = data.HolidaysAsLeave.ToString(),
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class LeavePolicyByIdQryHandler : IRequestHandler<LeavePolicyByIdQry, LeavePolicyListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public LeavePolicyByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<LeavePolicyListDto?> Handle(LeavePolicyByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<LeavePolicy>().GetById(request.Id);
        if (data == null) { return null; }
        var lvt = await _unitOfWork.Repository<LeaveType>().GetById(data.LeaveTypeId);

        var c = new LeavePolicyListDto
        {
            Id = data.Id,
            Name = data.Name,
            LeaveTypeId = data.LeaveTypeId,
            RequiresAttachment = data.RequiresAttachment,
            //MinDurPerReq = data.MinDurPerReq,
            //MaxDurPerReq = data.MaxDurPerReq,
            //HolidaysAsLeave = data.HolidaysAsLeave,
            LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
            RequiresAttachmentStr = data.RequiresAttachment.ToString(),
            //MinDurPerReqStr = $"{data.MinDurPerReq:#,##0.##} days",
            //MaxDurPerReqStr = $"{data.MaxDurPerReq:#,##0.##} days",
            //HolidaysAsLeaveStr = data.HolidaysAsLeave.ToString(),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}