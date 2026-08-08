// src/Svc.Procurement/Cor.Procurement/Models/Enums/PeriodEnums.cs
namespace Cor.Procurement.Models.Enums;

public enum PeriodType
{
    MONTHLY = 1,
    QUARTERLY = 2,
    YEARLY = 3,
    WEEKLY = 4,
    CUSTOM = 5
}

public enum PeriodStatus
{
    OPEN = 1,
    PENDING_CLOSE = 2,
    CLOSED = 3,
    LOCKED = 4
}
 public enum AuditStatus
            {
                Success = 1,
                Failed = 2,
                Pending = 3,
                Error=4,
                Unauthorized=5


            }