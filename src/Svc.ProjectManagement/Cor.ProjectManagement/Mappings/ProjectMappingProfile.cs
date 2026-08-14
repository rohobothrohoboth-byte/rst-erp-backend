// Mappings/ProjectMappingProfile.cs
using AutoMapper;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using TaskStatus = Cor.ProjectManagement.Models.Entities.TaskStatus;
namespace Cor.ProjectManagement.Mappings
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Project mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.TaskCount,
                    opt => opt.MapFrom(src => src.Tasks != null ? src.Tasks.Count(t => !t.IsDeleted) : 0))
                .ForMember(dest => dest.MilestoneCount,
                    opt => opt.MapFrom(src => src.Milestones != null ? src.Milestones.Count(m => !m.IsDeleted) : 0))
                .ForMember(dest => dest.ResourceCount,
                    opt => opt.MapFrom(src => src.Resources != null ? src.Resources.Count(r => !r.IsDeleted) : 0));

            CreateMap<ProjectCreateDto, Project>();
            CreateMap<ProjectUpdateDto, Project>();

            // Task mappings
            CreateMap<ProjectTask, ProjectTaskDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.ParentTaskTitle, opt => opt.MapFrom(src => src.ParentTask != null ? src.ParentTask.Title : string.Empty))
                .ForMember(dest => dest.PhaseName, opt => opt.MapFrom(src => src.Phase != null ? src.Phase.Name : string.Empty))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate.HasValue && src.DueDate.Value < DateTime.UtcNow && src.Status != TaskStatus.Completed && src.Status != TaskStatus.Cancelled));

            CreateMap<ProjectTaskCreateDto, ProjectTask>();
            CreateMap<ProjectTaskUpdateDto, ProjectTask>();

            // Phase mappings
            CreateMap<ProjectPhase, ProjectPhaseDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks != null ? src.Tasks.Count(t => !t.IsDeleted) : 0))
                .ForMember(dest => dest.MilestoneCount, opt => opt.MapFrom(src => src.Milestones != null ? src.Milestones.Count(m => !m.IsDeleted) : 0));

            CreateMap<ProjectPhaseCreateDto, ProjectPhase>();
            CreateMap<ProjectPhaseUpdateDto, ProjectPhase>();

            // Timesheet mappings
            CreateMap<Timesheet, TimesheetDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task != null ? src.Task.Title : src.TaskName));

            CreateMap<TimesheetCreateDto, Timesheet>();
            CreateMap<TimesheetUpdateDto, Timesheet>();

            // Resource mappings
            CreateMap<ProjectResource, ProjectResourceDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == ResourceAllocationStatus.Allocated || src.Status == ResourceAllocationStatus.InUse));

            CreateMap<ProjectResourceCreateDto, ProjectResource>();
            CreateMap<ProjectResourceUpdateDto, ProjectResource>();

            // Milestone mappings
            CreateMap<ProjectMilestone, ProjectMilestoneDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.PhaseName, opt => opt.MapFrom(src => src.Phase != null ? src.Phase.Name : string.Empty))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => !src.IsCompleted && src.DueDate < DateTime.UtcNow));

            CreateMap<ProjectMilestoneCreateDto, ProjectMilestone>();
            CreateMap<ProjectMilestoneUpdateDto, ProjectMilestone>();

            // Budget mappings
            CreateMap<ProjectBudget, ProjectBudgetDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.UtilizationPercentage,
                    opt => opt.MapFrom(src => src.PlannedAmount > 0 ? (src.ActualAmount / src.PlannedAmount) * 100 : 0));

            CreateMap<ProjectBudgetCreateDto, ProjectBudget>();
            CreateMap<ProjectBudgetUpdateDto, ProjectBudget>();

            // Risk mappings
            CreateMap<ProjectRisk, ProjectRiskDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.RiskLevel,
                    opt => opt.MapFrom(src => $"{src.Impact} - {src.Probability}"));

            CreateMap<ProjectRiskCreateDto, ProjectRisk>();
            CreateMap<ProjectRiskUpdateDto, ProjectRisk>();

            // Issue mappings
            CreateMap<ProjectIssue, ProjectIssueDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.DueDate.HasValue && src.DueDate.Value < DateTime.UtcNow && src.Status != IssueStatus.Resolved && src.Status != IssueStatus.Closed))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments != null ? src.Comments.Count(c => !c.IsDeleted) : 0));

            CreateMap<ProjectIssueCreateDto, ProjectIssue>();
            CreateMap<ProjectIssueUpdateDto, ProjectIssue>();

            // Change mappings
            CreateMap<ProjectChange, ProjectChangeDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));

            CreateMap<ProjectChangeCreateDto, ProjectChange>();
            CreateMap<ProjectChangeUpdateDto, ProjectChange>();

            // Document mappings
            CreateMap<ProjectDocument, ProjectDocumentDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.PhaseName, opt => opt.MapFrom(src => src.Phase != null ? src.Phase.Name : string.Empty))
                .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task != null ? src.Task.Title : string.Empty))
                .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => $"/api/documents/download/{src.Id}"));

            CreateMap<ProjectDocumentCreateDto, ProjectDocument>();
            CreateMap<ProjectDocumentUpdateDto, ProjectDocument>();

            // Comment mappings
            CreateMap<ProjectComment, ProjectCommentDto>()
                .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task != null ? src.Task.Title : string.Empty))
                .ForMember(dest => dest.MilestoneName, opt => opt.MapFrom(src => src.Milestone != null ? src.Milestone.Title : string.Empty))
                .ForMember(dest => dest.IssueName, opt => opt.MapFrom(src => src.Issue != null ? src.Issue.Title : string.Empty))
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies != null ? src.Replies.Count(r => !r.IsDeleted) : 0));

            CreateMap<ProjectCommentCreateDto, ProjectComment>();
            CreateMap<ProjectCommentUpdateDto, ProjectComment>();

            // Notification mappings
            CreateMap<ProjectNotification, ProjectNotificationDto>();

            // Audit Log mappings
            CreateMap<ProjectAuditLog, ProjectAuditLogDto>();
        }
    }
}