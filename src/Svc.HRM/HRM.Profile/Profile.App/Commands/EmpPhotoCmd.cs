using MediatR;
using Profile.App.Helpers;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpPhotoAddCmd : IRequest<EmpPhotoDto> { public EmpPhotoAddDto AddDto { get; set; } = default!; }

public class EmpPhotoAddCmdHandler : IRequestHandler<EmpPhotoAddCmd, EmpPhotoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpPhotoAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpPhotoDto> Handle(EmpPhotoAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var mData = new FileMetaData
            {
                FileName = request.AddDto.File.FileName,
                ContentType = request.AddDto.File.ContentType,
                FileSize = request.AddDto.File.Length
            };
            await _unitOfWork.Repository<FileMetaData>().Add(mData);
            await _unitOfWork.Commit();

            using var ms = new MemoryStream();
            await request.AddDto.File.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            var pBlob = new EmpPhotoBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _unitOfWork.Repository<EmpPhotoBlob>().Add(pBlob);
            await _unitOfWork.Commit();

            var thumbData = ThumbnailGenerator.GenerateThumbnail(ms);
            var tData = new FileMetaData
            {
                FileName = $"{request.AddDto.File.FileName}_thumb",
                ContentType = "image/png",
                FileSize = thumbData.Length
            };
            await _unitOfWork.Repository<FileMetaData>().Add(tData);
            await _unitOfWork.Commit();

            var tBlob = new EmpPhotoThumbnail
            {
                FileMetaDataId = tData.Id,
                Data = thumbData.ToArray()
            };
            await _unitOfWork.Repository<EmpPhotoThumbnail>().Add(tBlob);
            await _unitOfWork.Commit();

            var emp = new EmpPhoto
            {
                FileMetaDataId = mData.Id,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpPhoto>().Add(emp);
            await _unitOfWork.Commit();

            var res = new EmpPhotoDto();
            var response = await _med.Send(new EmpPhotoByIdQry { Id = emp.Id }, cancellationToken);
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