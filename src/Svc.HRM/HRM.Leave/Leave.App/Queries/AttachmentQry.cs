using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class AttachmentAllQry : IRequest<List<AttachmentListDto>> { }
public class AttachmentByIdQry : IRequest<AttachmentListDto?> { public Guid Id { get; set; } }

public class AttachmentAllQryHandler : IRequestHandler<AttachmentAllQry, List<AttachmentListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AttachmentAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<AttachmentListDto>> Handle(AttachmentAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<Attachment>().GetAll();
        var dataL = new List<AttachmentListDto>();

        foreach (var data in dbData)
        {
            var c = new AttachmentListDto
            {
                Id = data.Id,
                LeaveRequestId = data.LeaveRequestId,
                FileName = data.FileName,
                ContentType = data.ContentType,
                FileSize = SizeFormatter.FormatBytes(data.FileSize),
                DateUpload = data.DateUpload,
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

public class AttachmentByIdQryHandler : IRequestHandler<AttachmentByIdQry, AttachmentListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public AttachmentByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<AttachmentListDto?> Handle(AttachmentByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Attachment>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new AttachmentListDto
        {
            Id = data.Id,
            LeaveRequestId = data.LeaveRequestId,
            FileName = data.FileName,
            ContentType = data.ContentType,
            FileSize = SizeFormatter.FormatBytes(data.FileSize),
            DateUpload = data.DateUpload,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}