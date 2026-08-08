// Leave.Domain/DTOs/YearEndDtos.cs (or appropriate location)

namespace Leave.Domain.DTOs
{
    public class ProcessResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int EmployeesProcessed { get; set; }
        public int CarryoverRecordsCreated { get; set; }
        public int EncashmentRecordsCreated { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime? ProcessedAt { get; set; }
    }



    public class ApprovalStepDetailDto
    {
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Comments { get; set; }
    }

   public class PendingEncashmentApprovalDto
   {
       public Guid Id { get; set; }
       public Guid EmployeeId { get; set; }
       public string EmployeeName { get; set; } = string.Empty;
       public string Department { get; set; } = string.Empty;
       public string LeaveTypeName { get; set; } = string.Empty;
       public decimal EncashmentDays { get; set; }
       public decimal RatePerDay { get; set; }
       public decimal TotalAmount { get; set; }
       public string Reason { get; set; } = string.Empty;
       public DateTime RequestDate { get; set; }
       public string Status { get; set; } = string.Empty;
       public int CurrentStep { get; set; }
       public int MaxSteps { get; set; }
       public List<ApprovalStepDto> ApprovalChain { get; set; } = new();
   }

   public class EncashmentApprovalDetailsDto
   {
       public Guid Id { get; set; }
       public Guid EmployeeId { get; set; }
       public string EmployeeName { get; set; } = string.Empty;
       public string Department { get; set; } = string.Empty;
       public string LeaveTypeName { get; set; } = string.Empty;
       public decimal EncashmentDays { get; set; }
       public decimal RatePerDay { get; set; }
       public decimal TotalAmount { get; set; }
       public string Reason { get; set; } = string.Empty;
       public DateTime RequestDate { get; set; }
       public string Status { get; set; } = string.Empty;
       public int CurrentStep { get; set; }
       public List<ApprovalStepDetailDto> ApprovalChain { get; set; } = new();
   }

    public class ApprovalStepDto
    {
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
    }

    public class EncashmentApprovalDto
    {
        public string Status { get; set; } = string.Empty; // "Approved" or "Rejected"
        public string Comments { get; set; } = string.Empty;
        public Guid ApproverId { get; set; }
        public string ApproverName { get; set; } = string.Empty;
    }


}