// Queries/DocumentQueries.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Queries.DocumentQueries
{
    public class GetDocumentByIdQuery : IRequest<ProjectDocumentDto>
    {
        public Guid Id { get; set; }
    }

    public class GetDocumentsByProjectQuery : IRequest<List<ProjectDocumentDto>>
    {
        public Guid ProjectId { get; set; }
        public DocumentType? Type { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsArchived { get; set; }
        public Guid? PhaseId { get; set; }
        public Guid? TaskId { get; set; }
    }

    public class GetDocumentVersionsQuery : IRequest<List<ProjectDocumentDto>>
    {
        public Guid DocumentId { get; set; }
    }
}