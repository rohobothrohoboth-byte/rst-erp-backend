using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class LeavePolicyAllQry : IRequest<List<LeavePolicyListDto>> { }
public class LeavePolicyByIdQry : IRequest<LeavePolicyListDto?> { public Guid Id { get; set; } }
public class ActiveLeavePolicyQry : IRequest<List<LeavePolicyListDto>> { }

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
                Code = data.Code,
                Name = data.Name,
                AllowEncashment = data.AllowEncashment,
                RequiresAttachment = data.RequiresAttachment,
                Status = data.Status,
                LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
                StatusStr = ((PolicyStatus)Enum.Parse(typeof(PolicyStatus), data.Status)).ToDisplayName(),
                AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment),
                RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment),
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
            Code = data.Code,
            Name = data.Name,
            AllowEncashment = data.AllowEncashment,
            RequiresAttachment = data.RequiresAttachment,
            Status = data.Status,
            LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
            StatusStr = ((PolicyStatus)Enum.Parse(typeof(PolicyStatus), data.Status)).ToDisplayName(),
            AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment),
            RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class ActiveLeavePolicyHandler : IRequestHandler<ActiveLeavePolicyQry, List<LeavePolicyListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public ActiveLeavePolicyHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<LeavePolicyListDto>> Handle(ActiveLeavePolicyQry request, CancellationToken cancellationToken)
    {
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        var dbData = await _unitOfWork.Repository<LeavePolicy>().Find(p => p.Status == stat);
        var dataL = new List<LeavePolicyListDto>();
        var lvtL = await _unitOfWork.Repository<LeaveType>().GetAll();

        foreach (var data in dbData)
        {
            var lvt = lvtL.FirstOrDefault(t => t.Id == data.LeaveTypeId);
            var c = new LeavePolicyListDto
            {
                Id = data.Id,
                Code = data.Code,
                Name = data.Name,
                AllowEncashment = data.AllowEncashment,
                RequiresAttachment = data.RequiresAttachment,
                Status = data.Status,
                LeaveType = lvt != null ? lvt.Name : "NOT AVAILABLE",
                StatusStr = ((PolicyStatus)Enum.Parse(typeof(PolicyStatus), data.Status)).ToDisplayName(),
                AllowEncashmentStr = BoolToStr.FormatBool(data.AllowEncashment),
                RequiresAttachmentStr = BoolToStr.FormatBool(data.RequiresAttachment),
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