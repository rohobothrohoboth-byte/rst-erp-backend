// Recruit.App/Services/IRecruitNotificationService.cs

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Recruit.App.Services;

public interface IRecruitNotificationService
{
    Task NotifyStatusChangeAsync(Guid jobApplicationId, string oldStatus, string newStatus, CancellationToken ct);
    Task NotifyInterviewScheduledAsync(Guid interviewId, CancellationToken ct);
    Task NotifyOfferExtendedAsync(Guid offerId, CancellationToken ct);
    Task NotifyApplicationReceivedAsync(Guid jobApplicationId, CancellationToken ct);
    Task NotifyShortlistedAsync(Guid jobApplicationId, CancellationToken ct);
    Task NotifyOfferAcceptedAsync(Guid offerId, CancellationToken ct);
    Task NotifyOfferRejectedAsync(Guid offerId, CancellationToken ct);
}