using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpSignByIdQry : IRequest<EmpSignDto?> { public Guid Id { get; set; } }

public class EmpSignByIdQryHandler : IRequestHandler<EmpSignByIdQry, EmpSignDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpSignByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpSignDto?> Handle(EmpSignByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<EmpSign>().GetFoD(e => e.EmployeeId == request.Id);
        if (nData == null) { return null; }
        var fData = await _unitOfWork.Repository<FileMetaData>().GetById(nData.FileMetaDataId);
        if (fData == null) { return null; }
        var fBlob = await _unitOfWork.Repository<EmpSignBlob>().GetFoD(e => e.FileMetaDataId == nData.FileMetaDataId);
        if (fBlob == null) { return null; }

        var c = new EmpSignDto
        {
            Id = fData.Id,
            FileMetaDataId = nData.FileMetaDataId,
            EmpSignBlobId = fBlob.Id,
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

