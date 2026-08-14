// Commands/ProjectCommands/CreateProjectCommand.cs
using MediatR;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Entities;
namespace Cor.ProjectManagement.Commands.ProjectCommands
{
    public class CreateProjectCommand : IRequest<ProjectDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int Priority { get; set; } = 1;
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Tags { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateProjectCommand : IRequest<ProjectDto>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ProjectStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public decimal? TotalBilled { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }
        public int? Priority { get; set; }
        public double? CompletionPercentage { get; set; }
        public string? Tags { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DeleteProjectCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? DeletedBy { get; set; }
    }

    public class ChangeProjectStatusCommand : IRequest<ProjectDto>
    {
        public Guid Id { get; set; }
        public ProjectStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }
}