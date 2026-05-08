using Helpers;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Services;

public interface IEmpCertService
{
    Task CertBirth(CertSerDto dto, CancellationToken ctx);
    Task CertMarriage(CertSerDto dto, CancellationToken ctx);
}

public class EmpCertService(IUnitOfWork _uow) : IEmpCertService
{
    public async Task CertBirth(CertSerDto dto, CancellationToken ct)
    {
        var yes = BoolToStr.EnumToString(YesNo.Yes);
        var bcType = BoolToStr.EnumToString(CertType.Birth);
        if (dto.HasCert == yes && dto.File != null)
        {
            using var ms = new MemoryStream();
            await dto.File.CopyToAsync(ms, ct);
            ms.Position = 0;

            var bCert = await _uow.Set<EmpCert>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id && x.CertType == bcType, ct);
            if (bCert != null)
            {
                var bCertB = await _uow.Set<EmpCertBirth>().FirstOrDefaultAsync(x => x.EmpCertId == bCert.Id, ct);
                if (bCertB != null)
                {
                    var bcBlop = new EmpCertBirth
                    {
                        Data = ms.ToArray()
                    };
                    await _uow.Update(bcBlop);
                }
                else
                {
                    var cBlob = new EmpCertBirth
                    {
                        EmpCertId = bCert.Id,
                        Data = ms.ToArray()
                    };
                    await _uow.Add(cBlob, ct);
                }
            }
            else
            {
                var eCert = new EmpCert
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length,
                    CertType = bcType,
                    EmployeeId = dto.Id
                };
                await _uow.Add(eCert, ct);

                var cBlob = new EmpCertBirth
                {
                    EmpCertId = eCert.Id,
                    Data = ms.ToArray()
                };
                await _uow.Add(cBlob, ct);
            }
        }
    }
    
    public async Task CertMarriage(CertSerDto dto, CancellationToken ct)
    {
        var yes = BoolToStr.EnumToString(YesNo.Yes);
        var bcType = BoolToStr.EnumToString(CertType.Mar);
        if (dto.HasCert == yes && dto.File != null)
        {
            using var ms = new MemoryStream();
            await dto.File.CopyToAsync(ms, ct);
            ms.Position = 0;

            var mCert = await _uow.Set<EmpCert>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id && x.CertType == bcType, ct);
            if (mCert != null)
            {
                var mCertB = await _uow.Set<EmpCertMarriage>().FirstOrDefaultAsync(x => x.EmpCertId == mCert.Id, ct);
                if (mCertB != null)
                {
                    var bcBlop = new EmpCertMarriage
                    {
                        Data = ms.ToArray()
                    };
                    await _uow.Update(bcBlop);
                }
                else
                {
                    var cBlob = new EmpCertMarriage
                    {
                        EmpCertId = mCert.Id,
                        Data = ms.ToArray()
                    };
                    await _uow.Add(cBlob, ct);
                }
            }
            else
            {
                var eCert = new EmpCert
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length,
                    CertType = bcType,
                    EmployeeId = dto.Id
                };
                await _uow.Add(eCert, ct);

                var cBlob = new EmpCertMarriage
                {
                    EmpCertId = eCert.Id,
                    Data = ms.ToArray()
                };
                await _uow.Add(cBlob, ct);
            }
        }
    }



}