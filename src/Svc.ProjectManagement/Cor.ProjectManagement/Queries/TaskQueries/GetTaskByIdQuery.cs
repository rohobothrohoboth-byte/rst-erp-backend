// Queries/TaskQueries/GetTaskByIdQuery.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;
namespace Cor.ProjectManagement.Queries.TaskQueries
{
    public class GetTaskByIdQuery : IRequest<ProjectTaskDto>
    {
        public Guid Id { get; set; }
    }

    public class GetTasksByProjectQuery : IRequest<PaginatedResponse<ProjectTaskDto>>
    {
        public Guid ProjectId { get; set; }
        public Cor.ProjectManagement.Models.Entities.TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? AssigneeId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }

    public class GetTasksByAssigneeQuery : IRequest<PaginatedResponse<ProjectTaskDto>>
    {
        public Guid AssigneeId { get; set; }
         public Cor.ProjectManagement.Models.Entities.TaskStatus? Status { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetTaskTreeQuery : IRequest<List<ProjectTaskDto>>
    {
        public Guid ProjectId { get; set; }
    }
}