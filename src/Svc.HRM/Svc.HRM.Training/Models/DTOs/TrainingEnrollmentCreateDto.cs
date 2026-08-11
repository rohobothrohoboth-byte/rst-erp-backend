namespace Svc.HRM.Training.Models.DTOs;

public class TrainingEnrollmentCreateDto
{
    public Guid CourseId { get; set; }
    public Guid EmployeeId { get; set; }
    public string Status { get; set; } = "Enrolled";
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
}
