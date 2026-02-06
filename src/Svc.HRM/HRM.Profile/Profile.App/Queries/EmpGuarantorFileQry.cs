using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpGuarantorFileByIdQry : IRequest<EmpGuarantorFileDto?> { public Guid Id { get; set; } }

public class EmpGuarantorFileByIdQryHandler : IRequestHandler<EmpGuarantorFileByIdQry, EmpGuarantorFileDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpGuarantorFileByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpGuarantorFileDto?> Handle(EmpGuarantorFileByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<EmpGuarantorFile>().GetFoD(e => e.EmpGuarantorId == request.Id);
        if (nData == null) { return null; }
        var fData = await _unitOfWork.Repository<FileMetaData>().GetById(nData.FileMetaDataId);
        if (fData == null) { return null; }
        var fBlob = await _unitOfWork.Repository<EmpGuarantorFileBlob>().GetFoD(e => e.FileMetaDataId == nData.FileMetaDataId);
        if (fBlob == null) { return null; }
        var gur = await _unitOfWork.Repository<EmpGuarantor>().GetById(nData.EmpGuarantorId);

        var c = new EmpGuarantorFileDto
        {
            Id = fData.Id,
            FileMetaDataId = nData.FileMetaDataId,
            EmpGuarantorFileBlobId = fBlob.Id,
            EmpGuarantorId = nData.Id,
            FileName = fData.FileName,
            ContentType = fData.ContentType,
            FileSize = SizeFormatter.FormatBytes(fData.FileSize),
            EmpGuarantorName = gur != null ? $"{gur.Person.FirstName} {gur.Person.MiddleName} {gur.Person.LastName}" : "NOT AVAILABLE",
            EmpGuarantorNameAm = gur != null ? $"{gur.Person.FirstNameAm} {gur.Person.MiddleNameAm} {gur.Person.LastNameAm}" : "መረጃ ማግኘት አልተቻለም",
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };

        return c;
    }
}

