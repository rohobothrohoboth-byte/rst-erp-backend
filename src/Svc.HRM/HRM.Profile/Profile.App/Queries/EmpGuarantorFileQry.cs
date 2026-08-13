using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

// Download the guarantor's attached file (by employee id). Joins the guarantor,
// its file link, the file metadata and the blob bytes.
public class EmpGuarantorFileQry : IRequest<EmpFileRes?> { public Guid Id { get; set; } }

public class EmpGuarantorFileHandler(IDapperHelper _dapper) : IRequestHandler<EmpGuarantorFileQry, EmpFileRes?>
{
    public async Task<EmpFileRes?> Handle(EmpGuarantorFileQry request, CancellationToken ct)
    {
        const string eg = "eg";
        const string egf = "egf";
        const string fm = "fm";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<FileMetaData>(fm, x => x.FileName, x => x.ContentType)
            .Select<EmpGuarantorFileBlob>(b, x => x.Data)
            .From<EmpGuarantor>(eg)
            .Join<EmpGuarantor, EmpGuarantorFile>(eg, egf, x => x.Id, x => x.EmpGuarantorId)
            .Join<EmpGuarantorFile, FileMetaData>(egf, fm, x => x.FileMetaDataId, x => x.Id)
            .Join<EmpGuarantorFile, EmpGuarantorFileBlob>(egf, b, x => x.FileMetaDataId, x => x.FileMetaDataId)
            .Where<EmpGuarantor>(eg, x => x.EmployeeId == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpFileRes>(sql, parameters, ct);
        return data;
    }
}
