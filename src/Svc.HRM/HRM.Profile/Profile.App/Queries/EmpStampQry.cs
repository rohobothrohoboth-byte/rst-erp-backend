using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpStampByIdQry : IRequest<EmpStampDto?> { public Guid Id { get; set; } }

public class EmpStampByIdQryHandler : IRequestHandler<EmpStampByIdQry, EmpStampDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpStampByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpStampDto?> Handle(EmpStampByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<EmpStamp>().GetFoD(e => e.EmployeeId == request.Id);
        if (nData == null) { return null; }
        var fData = await _unitOfWork.Repository<FileMetaData>().GetById(nData.FileMetaDataId);
        if (fData == null) { return null; }
        var fBlob = await _unitOfWork.Repository<EmpStampBlob>().GetFoD(e => e.FileMetaDataId == nData.FileMetaDataId);
        if (fBlob == null) { return null; }

        var c = new EmpStampDto
        {
            Id = fData.Id,
            FileMetaDataId = nData.FileMetaDataId,
            EmpStampBlobId = fBlob.Id,
            EmployeeId = nData.Id,
            FileName = fData.FileName,
            ContentType = fData.ContentType,
            FileSize = SizeFormatter.FormatBytes(fData.FileSize),
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };

        return c;
    }
}

