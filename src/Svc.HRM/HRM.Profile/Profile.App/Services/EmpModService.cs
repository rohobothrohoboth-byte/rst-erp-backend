using Common;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Services;

public interface IEmpModService
{
    Task Salary(ModSalaryDto dto, CancellationToken ct);
    Task Photo(ModFileDto dto, CancellationToken ct);
    Task GraFile(ModFileDto dto, CancellationToken ct);
    Task StampFile(ModFileDto dto, CancellationToken ct);
    Task SignFile(ModFileDto dto, CancellationToken ct);


}

public class EmpModService(IUnitOfWork _uow, ICorHrmmClient _hrmmClient) : IEmpModService
{
    public async Task Salary(ModSalaryDto dto, CancellationToken ct)
    {
        var slyTask = await _hrmmClient.GetSalaryJgs((dto.JgStepId).ToString(), ct);
        var sal = 0.0; var cur = ""; var fre = ""; 
        if (slyTask.Salary != null)
        {
            sal = double.Parse(slyTask.Salary);
            cur = slyTask.Currency;
            fre = slyTask.SalaryPayFreq;
        }

        var empSly = await _uow.Set<EmpSalary>().FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId, ct);
        if (empSly == null)
        {
            var salary = new EmpSalary
            {
                BaseSalary = sal,
                Currency = cur,
                SalaryPayFreq = fre,
                EffectiveFrom = dto.EmploymentDate,
                JgStepId = dto.JgStepId,
                EmployeeId = dto.EmployeeId
            };
            await _uow.Add(salary, ct);
        }
        else
        {
            empSly.BaseSalary = sal;
            empSly.Currency = cur;
            empSly.SalaryPayFreq = fre;
            empSly.EffectiveFrom = dto.EmploymentDate;
            empSly.JgStepId = dto.JgStepId;
            await _uow.Update(empSly);
        }
    }

    public async Task Photo(ModFileDto dto, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await dto.File.CopyToAsync(ms, ct);
        ms.Position = 0;

        var ePhoto = await _uow.Set<EmpPhoto>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id, ct);
        if (ePhoto != null)
        {
            var fId = new Guid();
            var mData = await _uow.Set<FileMetaData>().FirstOrDefaultAsync(x => x.Id == ePhoto.FileMetaDataId, ct);
            if (mData != null)
            {
                mData.FileName = dto.File.FileName;
                mData.ContentType = dto.File.ContentType;
                mData.FileSize = dto.File.Length;
                await _uow.Update(mData);
                fId = mData.Id;
            }
            else
            {
                var mData2 = new FileMetaData
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length
                };
                await _uow.Add(mData2, ct);
                fId = mData2.Id;
            }

            var pBlob = await _uow.Set<EmpPhotoBlob>().FirstOrDefaultAsync(x => x.FileMetaDataId == ePhoto.FileMetaDataId, ct);
            if (pBlob != null)
            {
                pBlob.Data = ms.ToArray();
                await _uow.Update(pBlob);
            }
            else
            {
                var pBlob2 = new EmpPhotoBlob
                {
                    FileMetaDataId = fId,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob2, ct);
            }

            var tId = new Guid();
            var thumbData = ThumbnailGenerator.GenerateThumbnail(ms);
            var tData = await _uow.Set<FileMetaData>().FirstOrDefaultAsync(x => x.Id == ePhoto.ThumbnailId, ct);
            if (tData != null)
            {
                tData.FileName = $"{dto.File.FileName}_thumbnail";
                tData.ContentType = "image/png";
                tData.FileSize = thumbData.Length;
                await _uow.Update(tData);
                tId = tData.Id;
            }
            else
            {
                var tData2 = new FileMetaData
                {
                    FileName = $"{dto.File.FileName}_thumbnail",
                    ContentType = "image/png",
                    FileSize = thumbData.Length
                };
                await _uow.Add(tData2, ct);
                tId = tData2.Id;
            }

            var tBlob = await _uow.Set<EmpPhotoThumbnail>().FirstOrDefaultAsync(x => x.FileMetaDataId == ePhoto.ThumbnailId, ct);
            if (tBlob != null)
            {
                tBlob.Data = ms.ToArray();
                await _uow.Update(tBlob);
            }
            else
            {
                var tBlob2 = new EmpPhotoThumbnail
                {
                    FileMetaDataId = tId,
                    Data = ms.ToArray()
                };
                await _uow.Add(tBlob2, ct);
            }
        }
        else
        {
            var mData = new FileMetaData
            {
                FileName = dto.File.FileName,
                ContentType = dto.File.ContentType,
                FileSize = dto.File.Length
            };
            await _uow.Add(mData, ct);

            var pBlob = new EmpPhotoBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _uow.Add(pBlob, ct);

            var thumbData = ThumbnailGenerator.GenerateThumbnail(ms);
            var tData = new FileMetaData
            {
                FileName = $"{dto.File.FileName}_thumbnail",
                ContentType = "image/png",
                FileSize = thumbData.Length
            };
            await _uow.Add(tData, ct);

            var tBlob = new EmpPhotoThumbnail
            {
                FileMetaDataId = tData.Id,
                Data = thumbData.ToArray()
            };
            await _uow.Add(tBlob, ct);

            var emp = new EmpPhoto
            {
                ThumbnailId = tData.Id,
                FileMetaDataId = mData.Id,
                EmployeeId = dto.Id
            };
            await _uow.Add(emp, ct);
        }
    }

    public async Task GraFile(ModFileDto dto, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await dto.File.CopyToAsync(ms, ct);
        ms.Position = 0;

        var eGuar = await _uow.Set<EmpGuarantorFile>().FirstOrDefaultAsync(x => x.EmpGuarantorId == dto.Id, ct);
        if (eGuar != null)
        {
            var fId = new Guid();
            var mData = await _uow.Set<FileMetaData>().FirstOrDefaultAsync(x => x.Id == eGuar.FileMetaDataId, ct);
            if (mData != null)
            {
                mData.FileName = dto.File.FileName;
                mData.ContentType = dto.File.ContentType;
                mData.FileSize = dto.File.Length;
                await _uow.Update(mData);
                fId = mData.Id;
            }
            else
            {
                var mData2 = new FileMetaData
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length
                };
                await _uow.Add(mData2, ct);
                fId = mData2.Id;
            }

            var pBlob = await _uow.Set<EmpGuarantorFileBlob>().FirstOrDefaultAsync(x => x.FileMetaDataId == eGuar.FileMetaDataId, ct);
            if (pBlob != null)
            {
                pBlob.Data = ms.ToArray();
                await _uow.Update(pBlob);
            }
            else
            {
                var pBlob2 = new EmpGuarantorFileBlob
                {
                    FileMetaDataId = fId,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob2, ct);
            }
        }
        else
        {
            var mData = new FileMetaData
            {
                FileName = dto.File.FileName,
                ContentType = dto.File.ContentType,
                FileSize = dto.File.Length
            };
            await _uow.Add(mData, ct);

            var pBlob = new EmpGuarantorFileBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _uow.Add(pBlob, ct);

            var emp = new EmpGuarantorFile
            {
                FileMetaDataId = mData.Id,
                EmpGuarantorId = dto.Id
            };
            await _uow.Add(emp, ct);
        }
    }

    public async Task StampFile(ModFileDto dto, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await dto.File.CopyToAsync(ms, ct);
        ms.Position = 0;

        var eStamp = await _uow.Set<EmpSign>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id, ct);
        if (eStamp != null)
        {
            var fId = new Guid();
            var mData = await _uow.Set<FileMetaData>().FirstOrDefaultAsync(x => x.Id == eStamp.FileMetaDataId, ct);
            if (mData != null)
            {
                mData.FileName = dto.File.FileName;
                mData.ContentType = dto.File.ContentType;
                mData.FileSize = dto.File.Length;
                await _uow.Update(mData);
                fId = mData.Id;
            }
            else
            {
                var mData2 = new FileMetaData
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length
                };
                await _uow.Add(mData2, ct);
                fId = mData2.Id;
            }

            var pBlob = await _uow.Set<EmpSignBlob>().FirstOrDefaultAsync(x => x.FileMetaDataId == eStamp.FileMetaDataId, ct);
            if (pBlob != null)
            {
                pBlob.Data = ms.ToArray();
                await _uow.Update(pBlob);
            }
            else
            {
                var pBlob2 = new EmpSignBlob
                {
                    FileMetaDataId = fId,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob2, ct);
            }
        }
        else
        {
            var mData = new FileMetaData
            {
                FileName = dto.File.FileName,
                ContentType = dto.File.ContentType,
                FileSize = dto.File.Length
            };
            await _uow.Add(mData, ct);

            var pBlob = new EmpSignBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _uow.Add(pBlob, ct);

            var emp = new EmpSign
            {
                FileMetaDataId = mData.Id,
                EmployeeId = dto.Id
            };
            await _uow.Add(emp, ct);
        }
    }

    public async Task SignFile(ModFileDto dto, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        await dto.File.CopyToAsync(ms, ct);
        ms.Position = 0;

        var eStamp = await _uow.Set<EmpStamp>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id, ct);
        if (eStamp != null)
        {
            var fId = new Guid();
            var mData = await _uow.Set<FileMetaData>().FirstOrDefaultAsync(x => x.Id == eStamp.FileMetaDataId, ct);
            if (mData != null)
            {
                mData.FileName = dto.File.FileName;
                mData.ContentType = dto.File.ContentType;
                mData.FileSize = dto.File.Length;
                await _uow.Update(mData);
                fId = mData.Id;
            }
            else
            {
                var mData2 = new FileMetaData
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length
                };
                await _uow.Add(mData2, ct);
                fId = mData2.Id;
            }

            var pBlob = await _uow.Set<EmpStampBlob>().FirstOrDefaultAsync(x => x.FileMetaDataId == eStamp.FileMetaDataId, ct);
            if (pBlob != null)
            {
                pBlob.Data = ms.ToArray();
                await _uow.Update(pBlob);
            }
            else
            {
                var pBlob2 = new EmpStampBlob
                {
                    FileMetaDataId = fId,
                    Data = ms.ToArray()
                };
                await _uow.Add(pBlob2, ct);
            }
        }
        else
        {
            var mData = new FileMetaData
            {
                FileName = dto.File.FileName,
                ContentType = dto.File.ContentType,
                FileSize = dto.File.Length
            };
            await _uow.Add(mData, ct);

            var pBlob = new EmpStampBlob
            {
                FileMetaDataId = mData.Id,
                Data = ms.ToArray()
            };
            await _uow.Add(pBlob, ct);

            var emp = new EmpStamp
            {
                FileMetaDataId = mData.Id,
                EmployeeId = dto.Id
            };
            await _uow.Add(emp, ct);
        }
    }





}