// Helpers/PeriodHelpers.cs

using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Enums;

namespace Cor.Finance.Helpers;

public static class PeriodHelpers
{
    /// <summary>
    /// Gets the status string from a FinancialPeriod entity
    /// </summary>
    public static string GetStatusString(FinancialPeriod period)
    {
        if (period == null)
            return "Unknown";

        if (period.IsClosed)
            return "Closed";

        return period.Status switch
        {
            PeriodStatus.OPEN => "Open",
            PeriodStatus.LOCKED => "Locked",
            PeriodStatus.PENDING => "Pending",
            PeriodStatus.DRAFT => "Draft",
            PeriodStatus.CLOSED => "Closed",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets the status string from a boolean and status enum
    /// </summary>
    public static string GetStatusString(bool isClosed, PeriodStatus status)
    {
        if (isClosed)
            return "Closed";

        return status switch
        {
            PeriodStatus.OPEN => "Open",
            PeriodStatus.LOCKED => "Locked",
            PeriodStatus.PENDING => "Pending",
            PeriodStatus.DRAFT => "Draft",
            PeriodStatus.CLOSED => "Closed",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Maps a FinancialPeriod entity to a FinancialPeriodDto
    /// </summary>
    public static FinancialPeriodDto MapToDto(FinancialPeriod period)
    {
        if (period == null)
            throw new ArgumentNullException(nameof(period));

        return new FinancialPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PeriodType = period.PeriodType.ToString(),
            Status = GetStatusString(period),
            IsClosed = period.IsClosed,
            ClosedDate = period.ClosedDate,
            DateAdd = period.DateAdd,
            DateMod = period.DateMod,
            Notes = period.Notes,
            // ✅ Add these if they exist in your DTO
            // TotalEntries = period.TotalEntries,
            // PostedEntries = period.PostedEntries,
            // UnpostedEntries = period.UnpostedEntries,
            // TotalDebit = period.TotalDebit,
            // TotalCredit = period.TotalCredit,
            // DaysRemaining = period.DaysRemaining,
            // CompletionPercentage = period.CompletionPercentage,
            // CanBeClosed = period.CanBeClosed,
            // ClosingReason = period.ClosingReason,
            // ClosedBy = period.ClosedBy,
            // CreatedBy = period.CreatedBy,
            // CreatedByUserId = period.CreatedByUserId,
            // CreatedByUserName = period.CreatedByUserName,
            // UpdatedByUserId = period.UpdatedByUserId,
            // UpdatedByUserName = period.UpdatedByUserName
        };
    }

    /// <summary>
    /// Maps a list of FinancialPeriod entities to DTOs
    /// </summary>
    public static List<FinancialPeriodDto> MapToDtos(IEnumerable<FinancialPeriod> periods)
    {
        if (periods == null)
            return new List<FinancialPeriodDto>();

        return periods.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Gets the color/style for a period status (useful for UI)
    /// </summary>
    public static string GetStatusColor(string status)
    {
        return status?.ToLower() switch
        {
            "open" => "green",
            "closed" => "gray",
            "locked" => "red",
            "pending" => "yellow",
            "draft" => "blue",
            _ => "gray"
        };
    }

    /// <summary>
    /// Gets the status badge label with icon indicator
    /// </summary>
    public static string GetStatusBadge(string status)
    {
        return status?.ToLower() switch
        {
            "open" => "🟢 Open",
            "closed" => "🔒 Closed",
            "locked" => "🔴 Locked",
            "pending" => "🟡 Pending",
            "draft" => "📝 Draft",
            _ => "⚪ Unknown"
        };
    }

    /// <summary>
    /// Checks if a period can be edited
    /// </summary>
    public static bool CanEdit(FinancialPeriod period)
    {
        if (period == null)
            return false;

        return !period.IsClosed &&
               period.Status != PeriodStatus.LOCKED &&
               period.Status != PeriodStatus.CLOSED;
    }

    /// <summary>
    /// Checks if a period can be deleted
    /// </summary>
    public static bool CanDelete(FinancialPeriod period)
    {
        if (period == null)
            return false;

        // Can delete if not closed and no journal entries
        return !period.IsClosed &&
               period.Status != PeriodStatus.LOCKED &&
               period.Status != PeriodStatus.CLOSED;
    }
}