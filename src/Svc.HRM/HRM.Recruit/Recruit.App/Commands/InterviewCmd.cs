// Recruit.App/Commands/InterviewCmd.cs
using Common;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Queries;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;
using Recruit.App.Services;

namespace Recruit.App.Commands;

public class InterviewAddCmd : IRequest<InterviewListDto>
{
    public InterviewAddDto AddDto { get; set; } = default!;
}

public class InterviewModCmd : IRequest<InterviewListDto>
{
    public InterviewModDto ModDto { get; set; } = default!;
}

public class InterviewDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class InterviewStatusUpdateCmd : IRequest<InterviewListDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
}
// Recruit.App/Commands/InterviewCmd.cs - Updated InterviewAddHandler

// Recruit.App/Commands/InterviewCmd.cs - Updated InterviewAddHandler
public class InterviewAddHandler : IRequestHandler<InterviewAddCmd, InterviewListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IJobAppService _jobAppService;
    private readonly IHrmProfileClient _hrmProfileClient;
    private readonly IRecruitNotificationService _notificationService;

    public InterviewAddHandler(
        IUnitOfWork uow,
        IMediator med,
        IJobAppService jobAppService,
        IHrmProfileClient hrmProfileClient,
        IRecruitNotificationService notificationService)
    {
        _uow = uow;
        _med = med;
        _jobAppService = jobAppService;
        _hrmProfileClient = hrmProfileClient;
        _notificationService = notificationService;
    }

    public async Task<InterviewListDto> Handle(InterviewAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // ✅ STEP 1: Find or create JobApplication
            Guid jobApplicationId;
            Guid actualPersonId = request.AddDto.ApplicantId;
            bool isEmployee = false;
            JobApplication? application = null;

            // Check if the provided ID is a JobApplication ID
            var jobAppCheck = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.ApplicantId && !x.IsDeleted, ct);

            if (jobAppCheck != null)
            {
                // It's a JobApplication ID - use it directly
                jobApplicationId = jobAppCheck.Id;
                application = jobAppCheck;

                // Get the actual person ID for reference
                if (jobAppCheck.EmployeeId.HasValue && jobAppCheck.EmployeeId.Value != Guid.Empty)
                {
                    actualPersonId = jobAppCheck.EmployeeId.Value;
                    isEmployee = true;
                }
                else if (jobAppCheck.ApplicantId.HasValue && jobAppCheck.ApplicantId.Value != Guid.Empty)
                {
                    actualPersonId = jobAppCheck.ApplicantId.Value;
                }
            }
            else
            {
                // Check if the ID is an Employee (internal)
                try
                {
                    var emp = await _hrmProfileClient.GetEmp(request.AddDto.ApplicantId.ToString(), ct);
                    if (emp?.Res != null)
                    {
                        isEmployee = true;
                        actualPersonId = request.AddDto.ApplicantId;
                    }
                }
                catch { /* Not an employee */ }

                // If not an employee, check if it's an external Applicant
                if (!isEmployee)
                {
                    var external = await _uow.Set<Applicant>()
                        .FirstOrDefaultAsync(x => x.Id == request.AddDto.ApplicantId && !x.IsDeleted, ct);

                    if (external == null)
                    {
                        throw new DomainException(
                            $"Person with Id {request.AddDto.ApplicantId} not found. " +
                            "Please provide a valid Employee ID (internal) or Applicant ID (external)."
                        );
                    }
                }

                // Find or create JobApplication
                application = await _uow.Set<JobApplication>()
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == actualPersonId &&
                        x.JobPostingId == request.AddDto.JobPostingId &&
                        !x.IsDeleted, ct);

                if (application == null && !isEmployee)
                {
                    application = await _uow.Set<JobApplication>()
                        .FirstOrDefaultAsync(x =>
                            x.ApplicantId == actualPersonId &&
                            x.JobPostingId == request.AddDto.JobPostingId &&
                            !x.IsDeleted, ct);
                }

                if (application == null)
                {
                    // Create JobApplication
                    var jobPosting = await _uow.Set<JobPosting>()
                        .FirstOrDefaultAsync(x => x.Id == request.AddDto.JobPostingId && !x.IsDeleted, ct);

                    if (jobPosting == null)
                        throw new DomainException($"Job posting with Id {request.AddDto.JobPostingId} not found.");

                    application = new JobApplication
                    {
                        Id = Guid.CreateVersion7(),
                        Status = BoolToStr.EnumToString(ApplicationStatus.Applied),
                        PostType = jobPosting.PostType,
                        AppliedDate = DateTime.UtcNow,
                        ApplicantId = isEmployee ? null : actualPersonId,
                        EmployeeId = isEmployee ? actualPersonId : null,
                        JobPostingId = request.AddDto.JobPostingId,
                        DateAdd = DateTime.UtcNow,
                        DateMod = null,
                        IsDeleted = false
                    };
                    await _uow.Add(application, ct);
                }

                jobApplicationId = application.Id;
            }

            // ✅ STEP 2: Update JobApplication status to "Interviewed"
            if (application != null && application.Status != BoolToStr.EnumToString(ApplicationStatus.Interviewed))
            {
                application.Status = BoolToStr.EnumToString(ApplicationStatus.Interviewed);
                application.DateMod = DateTime.UtcNow;
                await _uow.Update(application);
            }

            // ✅ STEP 3: Create Interview
            var data = new Interview
            {
                Id = Guid.CreateVersion7(),
                ApplicantId = jobApplicationId,
                JobPostingId = request.AddDto.JobPostingId,
                InterviewType = request.AddDto.InterviewType,
                ScheduledDate = request.AddDto.ScheduledDate.ToUniversalTime(),
                Location = string.IsNullOrEmpty(request.AddDto.Location) ? null : request.AddDto.Location,
                MeetingLink = string.IsNullOrEmpty(request.AddDto.MeetingLink) ? null : request.AddDto.MeetingLink,
                Notes = string.IsNullOrEmpty(request.AddDto.Notes) ? null : request.AddDto.Notes,
                InterviewerId = request.AddDto.InterviewerId.HasValue && request.AddDto.InterviewerId.Value != Guid.Empty
                    ? request.AddDto.InterviewerId.Value
                    : null,
                Status = BoolToStr.EnumToString(InterviewStatus.Scheduled),
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };

            await _uow.Add(data, ct);
            await _uow.Commit(ct);
  await _notificationService.NotifyInterviewScheduledAsync(data.Id, ct);
            // ✅ STEP 4: Get the created interview
            var response = await _med.Send(new InterviewByIdQry { Id = data.Id }, ct);
            if (response == null)
                return new InterviewListDto();

            return new InterviewListDto
            {
                Id = response.Id,
                ApplicantId = response.ApplicantId,
                JobPostingId = response.JobPostingId,
                InterviewType = response.InterviewType,
                InterviewTypeStr = response.InterviewTypeStr,
                ScheduledDate = response.ScheduledDate,
                Location = response.Location,
                MeetingLink = response.MeetingLink,
                Notes = response.Notes,
                InterviewerId = response.InterviewerId,
                InterviewerName = response.InterviewerName,
                Status = response.Status,
                StatusStr = response.StatusStr,
                ApplicantName = response.ApplicantName,
                Position = response.Position,
                Department = response.Department,
                DateAdd = response.DateAdd,
                DateMod = response.DateMod,
                IsDeleted = response.IsDeleted,
                RowVersion = response.RowVersion
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}
public class InterviewModHandler : IRequestHandler<InterviewModCmd, InterviewListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public InterviewModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<InterviewListDto> Handle(InterviewModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Interview>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct);

            if (data == null)
                throw new DomainException($"INTERVIEW with Id {request.ModDto.Id} NOT FOUND.");

            data.InterviewType = request.ModDto.InterviewType;
            data.ScheduledDate = request.ModDto.ScheduledDate;
            data.Location = request.ModDto.Location;
            data.MeetingLink = request.ModDto.MeetingLink;
            data.Notes = request.ModDto.Notes;
            data.InterviewerId = request.ModDto.InterviewerId;
            data.Status = request.ModDto.Status;
            data.DateMod = DateTime.UtcNow;
            data.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

            await _uow.Update(data);
            await _uow.Commit(ct);

            var response = await _med.Send(new InterviewByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) return new InterviewListDto();

            return new InterviewListDto
            {
                Id = response.Id,
                ApplicantId = response.ApplicantId,
                JobPostingId = response.JobPostingId,
                InterviewType = response.InterviewType,
                InterviewTypeStr = response.InterviewTypeStr,
                ScheduledDate = response.ScheduledDate,
                Location = response.Location,
                MeetingLink = response.MeetingLink,
                Notes = response.Notes,
                InterviewerId = response.InterviewerId,
                InterviewerName = response.InterviewerName,
                Status = response.Status,
                StatusStr = response.StatusStr,
                ApplicantName = response.ApplicantName,
                Position = response.Position,
                Department = response.Department,
                DateAdd = response.DateAdd,
                DateMod = response.DateMod,
                IsDeleted = response.IsDeleted,
                RowVersion = response.RowVersion
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class InterviewDelHandler : IRequestHandler<InterviewDelCmd>
{
    private readonly IUnitOfWork _uow;

    public InterviewDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(InterviewDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Interview>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (data == null)
                throw new DomainException($"INTERVIEW with id [{request.Id}] NOT FOUND.");

            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// Recruit.App/Commands/InterviewCmd.cs - Fixed InterviewStatusUpdateHandler



public class InterviewStatusUpdateHandler : IRequestHandler<InterviewStatusUpdateCmd, InterviewListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public InterviewStatusUpdateHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<InterviewListDto> Handle(InterviewStatusUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var interview = await _uow.Set<Interview>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (interview == null)
                throw new DomainException($"INTERVIEW with id [{request.Id}] NOT FOUND.");

            // ✅ Get the JobApplication associated with this interview
            var jobApp = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == interview.ApplicantId && !x.IsDeleted, ct);

            // ✅ Update interview status
            var newStatus = request.Status;
            interview.Status = newStatus;
            interview.DateMod = DateTime.UtcNow;

            // ✅ Update JobApplication status based on interview status
            if (jobApp != null)
            {
                var jobAppStatus = jobApp.Status;
                var interviewStatusStr = MyEnumHelper.FormatEnum<InterviewStatus>(newStatus);

                // Map InterviewStatus to ApplicationStatus
                if (interviewStatusStr == "Completed")
                {
                    // If interview is completed, move to PassEval (Evaluation Passed)
                    if (jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.PassEval) &&
                        jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.OfferExtended) &&
                        jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.OfferAccepted) &&
                        jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.OfferRejected))
                    {
                        jobApp.Status = BoolToStr.EnumToString(ApplicationStatus.PassEval);
                        jobApp.DateMod = DateTime.UtcNow;
                        await _uow.Update(jobApp);
                    }
                }
                else if (interviewStatusStr == "Cancelled" || interviewStatusStr == "NoShow")
                {
                    // If interview is cancelled or no-show, move to Rejected
                    if (jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.Rejected))
                    {
                        jobApp.Status = BoolToStr.EnumToString(ApplicationStatus.Rejected);
                        jobApp.DateMod = DateTime.UtcNow;
                        await _uow.Update(jobApp);
                    }
                }
                else if (interviewStatusStr == "Rescheduled")
                {
                    // If rescheduled, keep as Interviewed
                    if (jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.Interviewed))
                    {
                        jobApp.Status = BoolToStr.EnumToString(ApplicationStatus.Interviewed);
                        jobApp.DateMod = DateTime.UtcNow;
                        await _uow.Update(jobApp);
                    }
                }
                else if (interviewStatusStr == "InProgress" || interviewStatusStr == "Scheduled")
                {
                    // If in progress or scheduled, keep as Interviewed
                    if (jobAppStatus != BoolToStr.EnumToString(ApplicationStatus.Interviewed))
                    {
                        jobApp.Status = BoolToStr.EnumToString(ApplicationStatus.Interviewed);
                        jobApp.DateMod = DateTime.UtcNow;
                        await _uow.Update(jobApp);
                    }
                }
            }

            await _uow.Update(interview);
            await _uow.Commit(ct);

            // Get updated interview
            var response = await _med.Send(new InterviewByIdQry { Id = request.Id }, ct);
            if (response == null)
                return new InterviewListDto();

            return new InterviewListDto
            {
                Id = response.Id,
                ApplicantId = response.ApplicantId,
                JobPostingId = response.JobPostingId,
                InterviewType = response.InterviewType,
                InterviewTypeStr = response.InterviewTypeStr,
                ScheduledDate = response.ScheduledDate,
                Location = response.Location,
                MeetingLink = response.MeetingLink,
                Notes = response.Notes,
                InterviewerId = response.InterviewerId,
                InterviewerName = response.InterviewerName,
                Status = response.Status,
                StatusStr = response.StatusStr,
                ApplicantName = response.ApplicantName,
                Position = response.Position,
                Department = response.Department,
                DateAdd = response.DateAdd,
                DateMod = response.DateMod,
                IsDeleted = response.IsDeleted,
                RowVersion = response.RowVersion
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}