using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpPhotoByIdQry : IRequest<EmpPhotoDto?> { public Guid Id { get; set; } }

public class EmpPhotoByIdQryHandler : IRequestHandler<EmpPhotoByIdQry, EmpPhotoDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpPhotoByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<EmpPhotoDto?> Handle(EmpPhotoByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<EmpPhoto>().GetFoD(e => e.EmployeeId == request.Id);
        if (nData == null) { return null; }
        var fData = await _unitOfWork.Repository<FileMetaData>().GetById(nData.FileMetaDataId);
        if (fData == null) { return null; }
        var fBlob = await _unitOfWork.Repository<EmpPhotoBlob>().GetFoD(e => e.FileMetaDataId == nData.FileMetaDataId);
        if (fBlob == null) { return null; }
        var tBlob = await _unitOfWork.Repository<EmpPhotoThumbnail>().GetFoD(e => e.FileMetaDataId == nData.FileMetaDataId);
        if (tBlob == null) { return null; }
        var emp = await _unitOfWork.Repository<Employee>().GetById(nData.EmployeeId);

        var c = new EmpPhotoDto
        {
            Id = fData.Id,
            FileMetaDataId = nData.FileMetaDataId,
            PhotoBlobId = fBlob.Id,
            PhotoThumbnailId = tBlob.Id,
            EmployeeId = nData.Id,
            FileName = fData.FileName,
            ContentType = fData.ContentType,
            FileSize = SizeFormatter.FormatBytes(fData.FileSize),
            EmpFullName = emp != null ? emp!.Person.FullName : "EMPLOYEE NOT AVAILABLE",
            EmpFullNameAm = emp != null ? emp!.Person.FullNameAm : "የሰራተኛው መረጃ ማግኘት አልተቻለም",
            IsDeleted = nData.IsDeleted,
            DateAdd = nData.DateAdd,
            DateMod = nData.DateMod,
            RowVersion = Convert.ToBase64String(nData.RowVersion)
        };
        
        return c;
    }
}

