using EthiopianCalendar;

namespace Profile.Domain.DTOs;

public class EmpPensionCardListDto : BaseDto
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string IsReceived { get; set; } = default!; //enum.YesNo
    public string IsSent { get; set; } = default!; //enum.YesNo
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string EmpFullName { get; set; } = default!; //Employee
    public string IsReceivedStr { get; set; } = default!;
    public string IsSentStr { get; set; } = default!;
    public string RegistrationDateStr => $"{RegistrationDate:MMMM dd, yyyy}";
    public string RegistrationDateStrAm => RegistrationDate.ToEthiopianDateString("MMMM dd, yyyy");
    public string SentDateStr => SentDate.HasValue ? $"{SentDate:MMMM dd, yyyy}" : "";
    public string SentDateStrAm => SentDate.HasValue ? SentDate.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
    public string ReceivedDateStr => ReceivedDate.HasValue ? $"{ReceivedDate:MMMM dd, yyyy}" : "";
    public string ReceivedDateStrAm => ReceivedDate.HasValue ? ReceivedDate.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
}

public class EmpPensionCardAddDto
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string IsReceived { get; set; } = default!; //enum.YesNo
    public string IsSent { get; set; } = default!; //enum.YesNo
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpPensionCardModDto
{
    public Guid Id { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string IsReceived { get; set; } = default!; //enum.YesNo
    public string IsSent { get; set; } = default!; //enum.YesNo
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}