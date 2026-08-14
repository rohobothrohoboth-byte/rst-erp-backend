// Queries/TimesheetQueries/GetTimesheetByIdQuery.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;
namespace Cor.ProjectManagement.Queries.TimesheetQueries
{
    public class GetTimesheetByIdQuery : IRequest<TimesheetDto>
    {
        public Guid Id { get; set; }
    }

    public class GetTimesheetsByUserQuery : IRequest<PaginatedResponse<TimesheetDto>>
    {
        public Guid UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TimesheetStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetTimesheetsByProjectQuery : IRequest<PaginatedResponse<TimesheetDto>>
    {
        public Guid ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? UserId { get; set; }
        public TimesheetStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetTimesheetSummaryQuery : IRequest<TimesheetSummaryDto>
    {
        public Guid UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}