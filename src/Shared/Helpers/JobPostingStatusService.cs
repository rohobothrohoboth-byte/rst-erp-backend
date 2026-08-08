// src/Shared/Helpers/JobPostingStatusService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Helpers;

public static class JobPostingStatusService
{
    // Status Constants
    public const string Draft = "Draft";
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Published = "Published";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";
    public const string Expired = "Expired";
    public const string OnHold = "OnHold";

    // Status Lists
    public static readonly List<string> AllStatuses = new()
    {
        Draft, Pending, Approved, Published, Closed, Cancelled, Expired, OnHold
    };

    public static readonly List<string> ModifiableStatuses = new()
    {
        Draft, Pending, Approved, OnHold
    };

    public static readonly List<string> PublishableStatuses = new()
    {
        Draft, Pending, Approved
    };

    public static readonly List<string> ClosableStatuses = new()
    {
        Published, OnHold
    };

    public static readonly List<string> TerminalStatuses = new()
    {
        Closed, Cancelled, Expired
    };

    /// <summary>
    /// Parses status string to enum
    /// </summary>
    public static PostingStatus ParseStatus(string statusStr)
    {
        if (string.IsNullOrEmpty(statusStr))
            return PostingStatus.Draft;

        if (Enum.TryParse<PostingStatus>(statusStr, true, out var status))
            return status;

        return statusStr.ToLower() switch
        {
            "pending approval" => PostingStatus.Pending,
            "on hold" => PostingStatus.OnHold,
            "draft" => PostingStatus.Draft,
            "approved" => PostingStatus.Approved,
            "published" => PostingStatus.Published,
            "closed" => PostingStatus.Closed,
            "cancelled" => PostingStatus.Cancelled,
            "expired" => PostingStatus.Expired,
            _ => PostingStatus.Draft
        };
    }

    /// <summary>
    /// Gets display name for status
    /// </summary>
    public static string GetDisplayName(PostingStatus status)
    {
        return status switch
        {
            PostingStatus.Draft => "Draft",
            PostingStatus.Pending => "Pending Approval",
            PostingStatus.Approved => "Approved",
            PostingStatus.Published => "Published",
            PostingStatus.Closed => "Closed",
            PostingStatus.Cancelled => "Cancelled",
            PostingStatus.Expired => "Expired",
            PostingStatus.OnHold => "On Hold",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Gets display name from string
    /// </summary>
    public static string GetDisplayNameFromString(string statusStr)
    {
        var status = ParseStatus(statusStr);
        return GetDisplayName(status);
    }

    /// <summary>
    /// Validates if a job posting can be published
    /// </summary>
    public static void ValidatePublish(string currentStatus)
    {
        var status = ParseStatus(currentStatus);

        if (status == PostingStatus.Published)
            return;

        if (!PublishableStatuses.Contains(status.ToString()))
        {
            var allowed = string.Join(", ", PublishableStatuses);
            throw new BusinessExc(
                $"Cannot publish job posting with status '{GetDisplayName(status)}'. " +
                $"Only {allowed} can be published."
            );
        }
    }

    /// <summary>
    /// Validates if a job posting can be closed
    /// </summary>
    public static void ValidateClose(string currentStatus)
    {
        var status = ParseStatus(currentStatus);

        if (status == PostingStatus.Closed)
            return;

        if (!ClosableStatuses.Contains(status.ToString()))
        {
            var allowed = string.Join(", ", ClosableStatuses);
            throw new BusinessExc(
                $"Cannot close job posting with status '{GetDisplayName(status)}'. " +
                $"Only {allowed} can be closed."
            );
        }
    }

    /// <summary>
    /// Validates if a job posting can be modified
    /// </summary>
    public static void ValidateModify(string currentStatus)
    {
        var status = ParseStatus(currentStatus);

        if (!ModifiableStatuses.Contains(status.ToString()))
        {
            var allowed = string.Join(", ", ModifiableStatuses);
            throw new BusinessExc(
                $"Cannot modify job posting with status '{GetDisplayName(status)}'. " +
                $"Only {allowed} can be modified."
            );
        }
    }

    /// <summary>
    /// Validates status transition
    /// </summary>
    public static void ValidateTransition(string currentStatusStr, string newStatusStr)
    {
        var currentStatus = ParseStatus(currentStatusStr);
        var newStatus = ParseStatus(newStatusStr);

        if (currentStatus == newStatus) return;

        var allowed = GetAllowedTransitions(currentStatus);
        if (!allowed.Contains(newStatus))
        {
            var allowedNames = string.Join(", ", allowed.Select(s => GetDisplayName(s)));
            throw new BusinessExc(
                $"Cannot transition from '{GetDisplayName(currentStatus)}' to '{GetDisplayName(newStatus)}'. " +
                $"Allowed transitions: {allowedNames}"
            );
        }
    }

    /// <summary>
    /// Validates if job posting can be deleted
    /// </summary>
    public static void ValidateDelete(string currentStatus)
    {
        var status = ParseStatus(currentStatus);

        if (TerminalStatuses.Contains(status.ToString()) || status == PostingStatus.Published)
        {
            throw new BusinessExc(
                $"Cannot delete job posting with status '{GetDisplayName(status)}'. " +
                "Only Draft, Pending, or On Hold postings can be deleted."
            );
        }
    }

    private static List<PostingStatus> GetAllowedTransitions(PostingStatus fromStatus)
    {
        return fromStatus switch
        {
            PostingStatus.Draft => new List<PostingStatus>
            {
                PostingStatus.Pending,
                PostingStatus.Cancelled,
                PostingStatus.Published
            },
            PostingStatus.Pending => new List<PostingStatus>
            {
                PostingStatus.Approved,
                PostingStatus.OnHold,
                PostingStatus.Cancelled,
                PostingStatus.Published
            },
            PostingStatus.Approved => new List<PostingStatus>
            {
                PostingStatus.Published,
                PostingStatus.OnHold,
                PostingStatus.Cancelled
            },
            PostingStatus.Published => new List<PostingStatus>
            {
                PostingStatus.Closed,
                PostingStatus.OnHold
            },
            PostingStatus.OnHold => new List<PostingStatus>
            {
                PostingStatus.Pending,
                PostingStatus.Approved,
                PostingStatus.Published,
                PostingStatus.Cancelled
            },
            PostingStatus.Closed => new List<PostingStatus>(),
            PostingStatus.Cancelled => new List<PostingStatus>(),
            PostingStatus.Expired => new List<PostingStatus>(),
            _ => new List<PostingStatus>()
        };
    }
}