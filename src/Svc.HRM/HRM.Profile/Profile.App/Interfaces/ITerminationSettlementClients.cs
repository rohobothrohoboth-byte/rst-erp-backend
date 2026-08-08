namespace Profile.App.Interfaces;

public interface IPayrollSettlementClient
{
    Task<PayrollSettlementResult> CreateFinalPayRunAsync(
        Guid employeeId,
        DateTime lastWorkingDate,
        string reason,
        CancellationToken ct = default);
}

public interface ILeaveSettlementClient
{
    Task<LeaveSettlementSnapshot> GetUnpaidDaysAsync(
        Guid employeeId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default);
}

public class PayrollSettlementResult
{
    public bool Success { get; set; }
    public Guid? PayrollRunId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class LeaveSettlementSnapshot
{
    public bool Success { get; set; }
    public decimal UnpaidDays { get; set; }
    public string Message { get; set; } = string.Empty;
}
