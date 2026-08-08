using MediatR;
using Cor.Procurement.Models.DTOs;

namespace Cor.Procurement.Queries;

public class GetReportByIdQuery : IRequest<ReportDto>
{
    public Guid Id { get; set; }
}

public class DownloadReportQuery : IRequest<(byte[] FileData, string FileName, string ContentType)>
{
    public Guid Id { get; set; }
}