// Models/DTOs/DashboardDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.ProjectManagement.Models.Entities;

namespace Cor.ProjectManagement.Models.DTOs
{
    public class ProjectDashboardDto
    {
        public int TotalProjects { get; set; }
        public Dictionary<ProjectStatus, int> ProjectsByStatus { get; set; } = new Dictionary<ProjectStatus, int>();
        public List<ProjectDto> RecentProjects { get; set; } = new List<ProjectDto>();
        public List<MilestoneDto> UpcomingMilestones { get; set; } = new List<MilestoneDto>();
        public List<TaskSummaryDto> OverdueTasks { get; set; } = new List<TaskSummaryDto>();
        public ResourceSummaryDto ResourceSummary { get; set; } = new ResourceSummaryDto();
        public BudgetSummaryDto BudgetSummary { get; set; } = new BudgetSummaryDto();
    }

    public class MilestoneDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Status => IsCompleted ? "Completed" : "Pending";
    }

    public class TaskSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string AssigneeName { get; set; } = string.Empty;
        public Cor.ProjectManagement.Models.Entities.TaskStatus Status { get; set; }
    }

    public class ResourceSummaryDto
    {
        public int TotalResources { get; set; }
        public int AvailableResources { get; set; }
        public int AllocatedResources { get; set; }
        public int OverAllocatedResources { get; set; }
        public Dictionary<ResourceType, int> ResourcesByType { get; set; } = new Dictionary<ResourceType, int>();
    }

    public class ProjectStatisticsDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TotalResources { get; set; }
        public int AllocatedResources { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal BudgetUtilization { get; set; }
        public Dictionary<Cor.ProjectManagement.Models.Entities.TaskStatus, int> TasksByStatus { get; set; } = new Dictionary<Cor.ProjectManagement.Models.Entities.TaskStatus, int>();
        public Dictionary<string, int> RisksByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> IssuesByStatus { get; set; } = new Dictionary<string, int>();
    }

    public class ProjectGanttDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<GanttTaskDto> Tasks { get; set; } = new List<GanttTaskDto>();
    }

    public class GanttTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Completion { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Assignee { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
        public string? Color { get; set; }
    }

    public class ProjectTimelineDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public List<TimelineMilestoneDto> Milestones { get; set; } = new List<TimelineMilestoneDto>();
        public List<TimelineEventDto> Events { get; set; } = new List<TimelineEventDto>();
    }

    public class TimelineMilestoneDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string Type => "Milestone";
    }

    public class TimelineEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}