using Helpers;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Services;

public interface IEmpCertService
{
    Task CertBirth(CertSerDto dto, CancellationToken ct);
    Task CertMarriage(CertSerDto dto, CancellationToken ct);
    Task<EmpCertRes> GetCerts(Guid id, CancellationToken ct);
}

public class EmpCertService(IUnitOfWork uow, IDapperHelper dapper) : IEmpCertService
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

            var bCert = await uow.Set<EmpCert>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id && x.CertType == bcType, ct);
            if (bCert != null)
            {
                var bCertB = await uow.Set<EmpCertBirth>().FirstOrDefaultAsync(x => x.EmpCertId == bCert.Id, ct);
                if (bCertB != null)
                {
                    bCertB.Data = ms.ToArray();
                    await uow.Update(bCertB);
                }
                else
                {
                    var cBlob = new EmpCertBirth
                    {
                        EmpCertId = bCert.Id,
                        Data = ms.ToArray()
                    };
                    await uow.Add(cBlob, ct);
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
                await uow.Add(eCert, ct);

                var cBlob = new EmpCertBirth
                {
                    EmpCertId = eCert.Id,
                    Data = ms.ToArray()
                };
                await uow.Add(cBlob, ct);
            }
        }
    }

    public async Task CertMarriage(CertSerDto dto, CancellationToken ct)
    {
        var yes = BoolToStr.EnumToString(YesNo.Yes);
        var mcType = BoolToStr.EnumToString(CertType.Mar);
        if (dto.HasCert == yes && dto.File != null)
        {
            using var ms = new MemoryStream();
            await dto.File.CopyToAsync(ms, ct);
            ms.Position = 0;

            var mCert = await uow.Set<EmpCert>().FirstOrDefaultAsync(x => x.EmployeeId == dto.Id && x.CertType == mcType, ct);
            if (mCert != null)
            {
                var mCertB = await uow.Set<EmpCertMarriage>().FirstOrDefaultAsync(x => x.EmpCertId == mCert.Id, ct);
                if (mCertB != null)
                {
                    mCertB.Data = ms.ToArray();
                    await uow.Update(mCertB);
                }
                else
                {
                    var cBlob = new EmpCertMarriage
                    {
                        EmpCertId = mCert.Id,
                        Data = ms.ToArray()
                    };
                    await uow.Add(cBlob, ct);
                }
            }
            else
            {
                var eCert = new EmpCert
                {
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length,
                    CertType = mcType,
                    EmployeeId = dto.Id
                };
                await uow.Add(eCert, ct);

                var cBlob = new EmpCertMarriage
                {
                    EmpCertId = eCert.Id,
                    Data = ms.ToArray()
                };
                await uow.Add(cBlob, ct);
            }
        }
    }

    public async Task<EmpCertRes> GetCerts(Guid id, CancellationToken ct)
    {
        var res = new EmpCertRes
        {
            HasBiCert = false,
            HasMaCert = false
        };
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EmpCert>(v, x => x.Id, x => x.FileName, x => x.ContentType, x => x.FileSize, x => x.CertType)
            .From<EmpCert>(v)
            .Where<EmpCert>(v, x => x.EmployeeId == id);
        var (sql, parameters) = qb.Build();
        await using var reader = await dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpCertJoin>(ct);
        if (list.Count <= 0) { return res; }

        var bType = BoolToStr.EnumToString(CertType.Birth);
        var mType = BoolToStr.EnumToString(CertType.Mar);
        var bCert = list.FirstOrDefault(x => x.CertType == bType);
        var mCert = list.FirstOrDefault(x => x.CertType == mType);
        if (bCert != null)
        {
            res.BiCertId = bCert.Id;
            res.BiCertName = bCert.FileName;
            res.BiCertType = bCert.ContentType;
            res.BiCertSize = bCert.FileSize;
            res.HasBiCert = true;
        }

        if (mCert != null)
        {
            res.MaCertId = mCert.Id;
            res.MaCertName = mCert.FileName;
            res.MaCertType = mCert.ContentType;
            res.MaCertSize = mCert.FileSize;
            res.HasMaCert = true;
        }

        return res;
    }



}