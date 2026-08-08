using EthiopianCalendar;
using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

 public class LeaveRequestDetailDto : BaseDto
 {
     public Guid LeaveTypeId { get; set; }
     public DateTime StartDate { get; set; }
     public DateTime EndDate { get; set; }
     public DateTime DateRequested { get; set; }
     public DateTime? DateApproved { get; set; }
     public string Comments { get; set; } = default!;
     public string DaysRequestedStr { get; set; } = default!;
     public string IsHalfDayStr { get; set; } = default!;
     public string StatusStr { get; set; } = default!;
     public string StartDateStr => $"{StartDate:MMMM dd, yyyy}";
     public string EndDateStr => $"{EndDate:MMMM dd, yyyy}";
     public string DateApprovedStr => DateApproved.HasValue ? $"{DateApproved:MMMM dd, yyyy}" : "";
     public string DateRequestedStr => $"{DateRequested:MMMM dd, yyyy}";
     public string ApprovedBy { get; set; } = default!;
     public string Employee { get; set; } = default!;
     public string LeaveType { get; set; } = default!;
     public Guid? ApprovalChainId { get; set; }

    public int? CurrentStepOrder { get; set; }
     public string StartDateStrAm
     {
         get
         {
             try
             {
                 return StartDate.ToEthiopianDateString("MMMM dd, yyyy");
             }
             catch
             {
                 return "";
             }
         }
     }

     public string EndDateStrAm
     {
         get
         {
             try
             {
                 return EndDate.ToEthiopianDateString("MMMM dd, yyyy");
             }
             catch
             {
                 return "";
             }
         }
     }

     public string DateApprovedStrAm
     {
         get
         {
             try
             {
                 return DateApproved.HasValue ? DateApproved.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
             }
             catch
             {
                 return "";
             }
         }
     }

     public string DateRequestedStrAm
     {
         get
         {
             try
             {
                 return DateRequested.ToEthiopianDateString("MMMM dd, yyyy");
             }
             catch
             {
                 return "";
             }
         }
     }
 }

public class LeaveRequestAddDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = false;
    public string Comments { get; set; } = default!;
    public Guid? ApprovalChainId { get; set; }
    public Guid? CurrentStepId { get; set; }
}
public class LeaveRequestAddDtoTemp
{
    public Guid LeaveTypeId { get; set; }
    public string StartDate { get; set; } = default!;
    public string EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; }
    public string Comments { get; set; } = default!;
}
public class LeaveRequestModDto
{
    public Guid Id { get; set; }
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = default!;
    public string Comments { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}
public class LeaveRequestListDto : BaseDto
{

    public Guid EmployeeId { get; set; }
    [JsonIgnore]
    public Guid? ApprovedById { get; set; }
    [JsonIgnore]
    public double DaysRequested { get; set; }
    [JsonIgnore]
    public bool IsHalfDay { get; set; }
    [JsonIgnore]
    public string Status { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DateRequested { get; set; }
    public string DaysRequestedStr { get; set; } = default!;
    public string IsHalfDayStr { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string StartDateStr => $"{StartDate:MMMM dd, yyyy}";
    public string EndDateStr => $"{EndDate:MMMM dd, yyyy}";
    public string DateRequestedStr => $"{DateRequested:MMMM dd, yyyy}";
    public string Employee { get; set; } = default!;
    public string LeaveType { get; set; } = default!;
    public Guid? ApprovalChainId { get; set; }
    public int? CurrentStepOrder { get; set; }
   public string Comments { get; set; } = default!;



    public string StartDateStrAm
    {
        get
        {
            try
            {
                return StartDate.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch
            {
                return "";
            }
        }
    }

    public string EndDateStrAm
    {
        get
        {
            try
            {
                return EndDate.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch
            {
                return "";
            }
        }
    }

    public string DateRequestedStrAm
    {
        get
        {
            try
            {
                return DateRequested.ToEthiopianDateString("MMMM dd, yyyy");
            }
            catch
            {
                return "";
            }
        }
    }
}

public class LeaveReqTestDto
{
    public Guid EmpId { get; set; } // LeaveType
    public Guid LeaveTypeId { get; set; } // LeaveType
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public bool IsHalfDay { get; set; } = false;
    public bool IsValid { get; set; } = false;
    public double Days { get; set; } = default!;
    public string Comments { get; set; } = default!;
}
public class ApprovalCommentDto
{
    public string Comments { get; set; } = string.Empty;
    public string RowVersion { get; set; } = string.Empty;
}

public class CancelRequestDto
{
    public string RowVersion { get; set; } = string.Empty;
}