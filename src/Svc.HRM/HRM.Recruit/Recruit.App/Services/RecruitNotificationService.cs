// Recruit.App/Services/RecruitNotificationService.cs

using Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Recruit.App.Interfaces;
using Recruit.Domain.Entities;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Recruit.App.Services;

public class RecruitNotificationService : IRecruitNotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<RecruitNotificationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _notificationServiceUrl;

    public RecruitNotificationService(
        IUnitOfWork uow,
        ILogger<RecruitNotificationService> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _uow = uow;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _notificationServiceUrl = configuration["NotificationServiceUrl"] ?? "https://localhost:1217";
    }

    public async Task NotifyStatusChangeAsync(Guid jobApplicationId, string oldStatus, string newStatus, CancellationToken ct)
    {
        try
        {
            var jobApp = await _uow.Set<JobApplication>()
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x => x.Id == jobApplicationId && !x.IsDeleted, ct);

            if (jobApp == null) return;

            var userId = jobApp.EmployeeId?.ToString() ?? jobApp.ApplicantId?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            var userName = await GetUserName(jobApp, ct);
            var position = jobApp.JobPosting?.PostNumber ?? "Position";

            var notification = new
            {
                Title = $"Application Status Updated - {position}",
                Message = GetStatusMessage(newStatus, userName, position),
                Type = GetNotificationType(newStatus),
                UserId = userId,
                ModuleName = "Recruitment",
                Metadata = new
                {
                    jobApplicationId = jobApp.Id,
                    jobPostingId = jobApp.JobPostingId,
                    status = newStatus,
                    oldStatus = oldStatus,
                    applicantName = userName,
                    position = position
                },
                Priority = newStatus == "Rejected" || newStatus == "OfferRejected" ? "high" : "normal"
            };

            await SendNotificationAsync(notification, ct);

            // Also notify HR/Admin if important status change
            if (newStatus == "OfferExtended" || newStatus == "OfferAccepted" || newStatus == "Rejected")
            {
                await NotifyHrTeamAsync(notification, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send status change notification for application {ApplicationId}", jobApplicationId);
        }
    }

    public async Task NotifyInterviewScheduledAsync(Guid interviewId, CancellationToken ct)
    {
        try
        {
            var interview = await _uow.Set<Interview>()
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x => x.Id == interviewId && !x.IsDeleted, ct);

            if (interview == null) return;

            var jobApp = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.Id == interview.ApplicantId && !x.IsDeleted, ct);

            if (jobApp == null) return;

            var userId = jobApp.EmployeeId?.ToString() ?? jobApp.ApplicantId?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            var userName = await GetUserName(jobApp, ct);
            var position = interview.JobPosting?.PostNumber ?? "Position";

            var notification = new
            {
                Title = $"Interview Scheduled - {position}",
                Message = $"An interview has been scheduled for {userName} on {interview.ScheduledDate:MMM dd, yyyy} at {interview.ScheduledDate:HH:mm}",
                Type = "info",
                UserId = userId,
                ModuleName = "Recruitment",
                Metadata = new
                {
                    interviewId = interview.Id,
                    jobPostingId = interview.JobPostingId,
                    scheduledDate = interview.ScheduledDate,
                    location = interview.Location,
                    meetingLink = interview.MeetingLink,
                    applicantName = userName,
                    position = position
                },
                Priority = "high"
            };

            await SendNotificationAsync(notification, ct);

            // Notify interviewer if assigned
            if (interview.InterviewerId.HasValue)
            {
                var interviewerNotification = new
                {
                    Title = $"Interview Assignment - {position}",
                    Message = $"You have been assigned to interview {userName} on {interview.ScheduledDate:MMM dd, yyyy} at {interview.ScheduledDate:HH:mm}",
                    Type = "info",
                    UserId = interview.InterviewerId.Value.ToString(),
                    ModuleName = "Recruitment",
                    Metadata = new
                    {
                        interviewId = interview.Id,
                        applicantName = userName,
                        scheduledDate = interview.ScheduledDate,
                        location = interview.Location,
                        meetingLink = interview.MeetingLink,
                        position = position
                    },
                    Priority = "high"
                };

                await SendNotificationAsync(interviewerNotification, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send interview scheduled notification for interview {InterviewId}", interviewId);
        }
    }

    public async Task NotifyOfferExtendedAsync(Guid offerId, CancellationToken ct)
    {
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .Include(x => x.JobApplication)
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x => x.Id == offerId && !x.IsDeleted, ct);

            if (offer == null) return;

            var jobApp = offer.JobApplication;
            if (jobApp == null) return;

            var userId = jobApp.EmployeeId?.ToString() ?? jobApp.ApplicantId?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            var userName = await GetUserName(jobApp, ct);
            var position = offer.JobPosting?.PostNumber ?? "Position";

            var notification = new
            {
                Title = $"Job Offer Extended - {position}",
                Message = $"An offer has been extended to {userName} for the position. Please review and respond by {offer.ExpirationDate:MMM dd, yyyy}.",
                Type = "success",
                UserId = userId,
                ModuleName = "Recruitment",
                Metadata = new
                {
                    offerId = offer.Id,
                    jobApplicationId = jobApp.Id,
                    jobPostingId = offer.JobPostingId,
                    offerDate = offer.OfferDate,
                    expiryDate = offer.ExpirationDate,
                    applicantName = userName,
                    position = position
                },
                Priority = "urgent"
            };

            await SendNotificationAsync(notification, ct);

            // Notify HR team
            await NotifyHrTeamAsync(notification, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send offer extended notification for offer {OfferId}", offerId);
        }
    }

    public async Task NotifyApplicationReceivedAsync(Guid jobApplicationId, CancellationToken ct)
    {
        try
        {
            var jobApp = await _uow.Set<JobApplication>()
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x => x.Id == jobApplicationId && !x.IsDeleted, ct);

            if (jobApp == null) return;

            var userId = jobApp.EmployeeId?.ToString() ?? jobApp.ApplicantId?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            var userName = await GetUserName(jobApp, ct);
            var position = jobApp.JobPosting?.PostNumber ?? "Position";

            var notification = new
            {
                Title = $"Application Received - {position}",
                Message = $"Your application for {position} has been received successfully. We will review it and get back to you soon.",
                Type = "success",
                UserId = userId,
                ModuleName = "Recruitment",
                Metadata = new
                {
                    jobApplicationId = jobApp.Id,
                    jobPostingId = jobApp.JobPostingId,
                    appliedDate = jobApp.AppliedDate,
                    applicantName = userName,
                    position = position
                },
                Priority = "normal"
            };

            await SendNotificationAsync(notification, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send application received notification for application {ApplicationId}", jobApplicationId);
        }
    }

    public async Task NotifyShortlistedAsync(Guid jobApplicationId, CancellationToken ct)
    {
        await NotifyStatusChangeAsync(jobApplicationId, "", "Shortlisted", ct);
    }

    public async Task NotifyOfferAcceptedAsync(Guid offerId, CancellationToken ct)
    {
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .Include(x => x.JobApplication)
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x => x.Id == offerId && !x.IsDeleted, ct);

            if (offer == null) return;

            var jobApp = offer.JobApplication;
            if (jobApp == null) return;

            var userId = jobApp.EmployeeId?.ToString() ?? jobApp.ApplicantId?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            var userName = await GetUserName(jobApp, ct);
            var position = offer.JobPosting?.PostNumber ?? "Position";

            var notification = new
            {
                Title = $"Offer Accepted - {position}",
                Message = $"{userName} has accepted the offer for {position}. Welcome to the team!",
                Type = "success",
                UserId = userId,
                ModuleName = "Recruitment",
                Metadata = new
                {
                    offerId = offer.Id,
                    jobApplicationId = jobApp.Id,
                    jobPostingId = offer.JobPostingId,
                    applicantName = userName,
                    position = position
                },
                Priority = "urgent"
            };

            await SendNotificationAsync(notification, ct);
            await NotifyHrTeamAsync(notification, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send offer accepted notification for offer {OfferId}", offerId);
        }
    }

    public async Task NotifyOfferRejectedAsync(Guid offerId, CancellationToken ct)
    {
        try
        {
            var offer = await _uow.Set<JobOffer>()
                .Include(x => x.JobApplication)
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x => x.Id == offerId && !x.IsDeleted, ct);

            if (offer == null) return;

            var jobApp = offer.JobApplication;
            if (jobApp == null) return;

            var userId = jobApp.EmployeeId?.ToString() ?? jobApp.ApplicantId?.ToString();
            if (string.IsNullOrEmpty(userId)) return;

            var userName = await GetUserName(jobApp, ct);
            var position = offer.JobPosting?.PostNumber ?? "Position";

            var notification = new
            {
                Title = $"Offer Rejected - {position}",
                Message = $"{userName} has rejected the offer for {position}.",
                Type = "error",
                UserId = userId,
                ModuleName = "Recruitment",
                Metadata = new
                {
                    offerId = offer.Id,
                    jobApplicationId = jobApp.Id,
                    jobPostingId = offer.JobPostingId,
                    applicantName = userName,
                    position = position
                },
                Priority = "high"
            };

            await SendNotificationAsync(notification, ct);
            await NotifyHrTeamAsync(notification, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send offer rejected notification for offer {OfferId}", offerId);
        }
    }

    private async Task SendNotificationAsync(object notification, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(notification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_notificationServiceUrl}/api/auth/v1/Notification", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to send notification: {StatusCode} - {Reason}",
                    response.StatusCode, response.ReasonPhrase);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
        }
    }

    private async Task NotifyHrTeamAsync(object notification, CancellationToken ct)
    {
        try
        {
            // In a real implementation, you would get HR team user IDs
            // For now, we'll just log it
            _logger.LogInformation("HR notification would be sent: {Title}",
                notification.GetType().GetProperty("Title")?.GetValue(notification));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify HR team");
        }
    }

    private string GetStatusMessage(string status, string userName, string position)
    {
        return status switch
        {
            "Applied" => $"Your application for {position} has been received successfully.",
            "UnderReview" => $"Your application for {position} is now under review by the hiring team.",
            "Shortlisted" => $"Congratulations! You have been shortlisted for the {position} position.",
            "Interviewed" => $"Your interview for {position} has been completed. We will update you on the next steps.",
            "PassEval" => $"You have passed the evaluation for {position}. The hiring team will contact you shortly.",
            "OfferExtended" => $"An offer has been extended for the {position} position. Please review the offer letter.",
            "OfferAccepted" => $"Congratulations on accepting the offer for {position}! Welcome aboard!",
            "OfferRejected" => $"The offer for {position} has been rejected.",
            "Rejected" => $"Thank you for your interest in {position}. After careful review, we will not be moving forward with your application.",
            "Withdrawn" => $"Your application for {position} has been withdrawn.",
            "OnHold" => $"Your application for {position} is currently on hold.",
            _ => $"Your application status for {position} has been updated to {status}."
        };
    }

    private string GetNotificationType(string status)
    {
        return status switch
        {
            "Applied" => "info",
            "UnderReview" => "info",
            "Shortlisted" => "success",
            "Interviewed" => "info",
            "PassEval" => "success",
            "OfferExtended" => "success",
            "OfferAccepted" => "success",
            "OfferRejected" => "error",
            "Rejected" => "error",
            "Withdrawn" => "info",
            "OnHold" => "warning",
            _ => "info"
        };
    }

    private async Task<string> GetUserName(JobApplication jobApp, CancellationToken ct)
    {
        try
        {
            if (jobApp.EmployeeId.HasValue)
            {
                // In a real implementation, you would call gRPC to get employee name
                return "Employee";
            }
            else if (jobApp.ApplicantId.HasValue)
            {
                var applicant = await _uow.Set<Applicant>()
                    .Include(x => x.Person)
                    .FirstOrDefaultAsync(x => x.Id == jobApp.ApplicantId.Value && !x.IsDeleted, ct);

                if (applicant?.Person != null)
                {
                    return $"{applicant.Person.FirstName} {applicant.Person.MiddleName} {applicant.Person.LastName}".Trim();
                }
                return "Applicant";
            }
            return "User";
        }
        catch
        {
            return "User";
        }
    }
}