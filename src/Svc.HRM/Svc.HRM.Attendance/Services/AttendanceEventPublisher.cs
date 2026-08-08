using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Svc.HRM.Attendance.Services;

public class AttendanceEventPublisher : IAttendanceEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<AttendanceEventPublisher> _logger;

    public AttendanceEventPublisher(IConfiguration configuration, ILogger<AttendanceEventPublisher> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("attendance.events", ExchangeType.Topic, durable: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ");
            throw;
        }
    }

    public async Task PublishAttendanceClockedInAsync(Guid employeeId, Guid attendanceId, DateTime checkIn, CancellationToken ct = default)
    {
        var message = new
        {
            EventType = "ATTENDANCE_CLOCKED_IN",
            EmployeeId = employeeId,
            AttendanceId = attendanceId,
            CheckIn = checkIn,
            Timestamp = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish("attendance.events", "attendance.clocked_in", null, body);
        _logger.LogInformation("Published ATTENDANCE_CLOCKED_IN event for employee {EmployeeId}", employeeId);
        await Task.CompletedTask;
    }

    public async Task PublishAttendanceClockedOutAsync(Guid employeeId, Guid attendanceId, DateTime checkOut, double hoursWorked, CancellationToken ct = default)
    {
        var message = new
        {
            EventType = "ATTENDANCE_CLOCKED_OUT",
            EmployeeId = employeeId,
            AttendanceId = attendanceId,
            CheckOut = checkOut,
            HoursWorked = hoursWorked,
            Timestamp = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish("attendance.events", "attendance.clocked_out", null, body);
        _logger.LogInformation("Published ATTENDANCE_CLOCKED_OUT event for employee {EmployeeId}", employeeId);
        await Task.CompletedTask;
    }

    public async Task PublishLeaveApprovedAsync(Guid employeeId, Guid leaveId, DateTime startDate, DateTime endDate, double days, CancellationToken ct = default)
    {
        var message = new
        {
            EventType = "LEAVE_APPROVED",
            EmployeeId = employeeId,
            LeaveId = leaveId,
            StartDate = startDate,
            EndDate = endDate,
            Days = days,
            Timestamp = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish("attendance.events", "attendance.leave.approved", null, body);
        _logger.LogInformation("Published LEAVE_APPROVED event for employee {EmployeeId}", employeeId);
        await Task.CompletedTask;
    }

    public async Task PublishOvertimeApprovedAsync(Guid employeeId, DateTime date, double hours, CancellationToken ct = default)
    {
        var message = new
        {
            EventType = "OVERTIME_APPROVED",
            EmployeeId = employeeId,
            Date = date,
            Hours = hours,
            Timestamp = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish("attendance.events", "attendance.overtime.approved", null, body);
        _logger.LogInformation("Published OVERTIME_APPROVED event for employee {EmployeeId}", employeeId);
        await Task.CompletedTask;
    }
}