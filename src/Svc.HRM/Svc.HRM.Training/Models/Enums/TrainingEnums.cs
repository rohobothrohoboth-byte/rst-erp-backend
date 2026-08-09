namespace Svc.HRM.Training.Models.Enums;

public static class ProgramStatus
{
    public const string Draft = "Draft";
    public const string Published = "Published";
    public const string Active = "Active";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}

public static class SessionStatus
{
    public const string Scheduled = "Scheduled";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}

public static class EnrollmentStatus
{
    public const string Enrolled = "Enrolled";
    public const string Waitlisted = "Waitlisted";
    public const string Attended = "Attended";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
    public const string NoShow = "NoShow";
}

public static class CertificateStatus
{
    public const string Issued = "Issued";
    public const string Revoked = "Revoked";
    public const string Expired = "Expired";
}
