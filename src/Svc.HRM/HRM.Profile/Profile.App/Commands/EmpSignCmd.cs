using MediatR;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpSignAddCmd : IRequest<EmpSignDto> { public EmpSignAddDto AddDto { get; set; } = default!; }

public class EmpSignAddCmdHandler : IRequestHandler<EmpSignAddCmd, EmpSignDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpSignAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpSignDto> Handle(EmpSignAddCmd request, CancellationToken cancellationToken)
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
            var pBlob = new EmpSignBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _unitOfWork.Repository<EmpSignBlob>().Add(pBlob);

            var emp = new EmpSign
            {
                FileMetaDataId = mData.Id,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpSign>().Add(emp);
            await _unitOfWork.Commit();

            var res = new EmpSignDto();
            var response = await _med.Send(new EmpSignByIdQry { Id = emp.Id }, cancellationToken);
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