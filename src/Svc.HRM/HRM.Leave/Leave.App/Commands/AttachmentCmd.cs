using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class AttachmentAddCmd : IRequest<AttachmentListDto> { public AttachmentAddDto AddDto { get; set; } = default!; }
public class AttachmentModCmd : IRequest<AttachmentListDto> { public AttachmentModDto ModDto { get; set; } = default!; }
public class AttachmentDelCmd : IRequest { public Guid Id { get; set; } }

public class AttachmentAddCmdHandler : IRequestHandler<AttachmentAddCmd, AttachmentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AttachmentAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<AttachmentListDto> Handle(AttachmentAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var attach = new Attachment
            {
                LeaveRequestId = request.AddDto.LeaveRequestId,
                FileName = request.AddDto.File!.FileName,
                ContentType = request.AddDto.File.ContentType,
                FileSize = request.AddDto.File.Length,
                DateUpload = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Attachment>().Add(attach);


            using var ms = new MemoryStream();
            await request.AddDto.File.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            var pBlob = new AttachmentBlob
            {
                AttachmentId = attach.Id,
                Data = ms.ToArray()
            };
            await _unitOfWork.Repository<AttachmentBlob>().Add(pBlob);
            await _unitOfWork.Commit();

            var res = new AttachmentListDto();
            var response = await _med.Send(new AttachmentByIdQry { Id = attach.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class AttachmentModCmdHandler : IRequestHandler<AttachmentModCmd, AttachmentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AttachmentModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<AttachmentListDto> Handle(AttachmentModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<Attachment>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"ATTACHMENT with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.LeaveRequestId = request.ModDto.LeaveRequestId;
            oldData.FileName = request.ModDto.File!.FileName;
            oldData.ContentType = request.ModDto.File.ContentType;
            oldData.FileSize = request.ModDto.File.Length;
            var data = await _unitOfWork.Repository<Attachment>().Update(oldData);

            var blob = await _unitOfWork.Repository<AttachmentBlob>().GetFoD(x => x.AttachmentId == request.ModDto.Id);
            using var ms = new MemoryStream();
            await request.ModDto.File.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            if (blob != null)
            {
                blob.Data = ms.ToArray();
                await _unitOfWork.Repository<AttachmentBlob>().Update(blob);
            }
            else
            {
                var pBlob = new AttachmentBlob
                {
                    AttachmentId = request.ModDto.Id,
                    Data = ms.ToArray()
                };
                await _unitOfWork.Repository<AttachmentBlob>().Add(pBlob);
            }
            await _unitOfWork.Commit();

            var res = new AttachmentListDto();
            var response = await _med.Send(new AttachmentByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class AttachmentDelCmdHandler : IRequestHandler<AttachmentDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public AttachmentDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(AttachmentDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<Attachment>().GetById(request.Id);
            if (data == null) { throw new DomainException($"ATTACHMENT with id [{request.Id}] NOT FOUND."); }

            var blob = await _unitOfWork.Repository<AttachmentBlob>().GetFoD(x => x.AttachmentId == request.Id);
            if (blob != null)
            {
                await _unitOfWork.Repository<AttachmentBlob>().Delete(blob.Id);
            }
            await _unitOfWork.Repository<Attachment>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}