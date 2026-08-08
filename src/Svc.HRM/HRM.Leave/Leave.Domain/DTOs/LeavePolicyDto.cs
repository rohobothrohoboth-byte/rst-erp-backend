namespace Leave.Domain.DTOs;

public class LeavePolicyListDto : BaseDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } = default!; // enum.PolicyStatus
    public string LeaveType { get; set; } = default!;
    public string StatusStr { get; set; } = default!; // LeaveType
    public string AllowEncashmentStr { get; set; } = default!;
    public string RequiresAttachmentStr { get; set; } = default!;
}

public class LeavePolicyAddDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public Guid LeaveTypeId { get; set; } // LeaveType
}

public class LeavePolicyModDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } = default!; // enum.PolicyStatus
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string RowVersion { get; set; } = default!;
}

public class EncashmentRequestDto
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public decimal EncashmentDays { get; set; }
    public Guid? ProcessedBy { get; set; }
    public string? Notes { get; set; }
     public string? Reason { get; set; }
}


public class EncashmentResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public double EncashedDays { get; set; }
    public double TotalAmount { get; set; }
    public double TaxAmount { get; set; }
    public double NetAmount { get; set; }
    public DateTime ProcessedAt { get; set; }
    public Guid EncashmentId { get; set; }
}



public class EncashmentHistoryDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public string LeavePolicyName { get; set; } = string.Empty;
    public double DaysEncashed { get; set; }
    public double RatePerDay { get; set; }
    public double TotalAmount { get; set; }
    public double TaxAmount { get; set; }
    public double NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateAdd { get; set; }
    public Guid? ProcessedBy { get; set; }
    public string? ProcessedByName { get; set; }
    public string? Notes { get; set; }
}
public class EncashmentConfigDto
{
    public bool AllowEncashment { get; set; }
    public decimal MaxEncashableDays { get; set; }
    public decimal EncashmentRate { get; set; }
    public bool RequiresApproval { get; set; }
}

// Add to your DTOs file
