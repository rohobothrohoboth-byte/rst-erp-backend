using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpGuarantorFileAddCmd : IRequest<EmpGuarantorFileDto> { public EmpGuarantorFileAddDto AddDto { get; set; } = default!; }

public class EmpGuarantorFileAddCmdHandler : IRequestHandler<EmpGuarantorFileAddCmd, EmpGuarantorFileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpGuarantorFileAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpGuarantorFileDto> Handle(EmpGuarantorFileAddCmd request, CancellationToken cancellationToken)
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

            using var ms = new MemoryStream();
            await request.AddDto.File.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            var pBlob = new EmpGuarantorFileBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _unitOfWork.Repository<EmpGuarantorFileBlob>().Add(pBlob);
            
            var emp = new EmpGuarantorFile
            {
                FileMetaDataId = mData.Id,
                EmpGuarantorId = request.AddDto.EmpGuarantorId
            };
            await _unitOfWork.Repository<EmpGuarantorFile>().Add(emp);
            await _unitOfWork.Commit();

            var res = new EmpGuarantorFileDto();
            var response = await _med.Send(new EmpGuarantorFileByIdQry { Id = emp.Id }, cancellationToken);
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