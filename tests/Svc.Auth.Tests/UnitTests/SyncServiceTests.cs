using Moq;
using Xunit;
using Svc.Auth.Services;
using Svc.Auth.Models.Dtos;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Microsoft.Extensions.Configuration;
using Svc.Auth.HealthChecks;

namespace Svc.Auth.Tests.UnitTests;

public class SyncServiceTests
{
    private readonly Mock<ICacheService> _mockCache;
    private readonly Mock<ILogger<SyncService>> _mockLogger;
    private readonly Mock<IAlertService> _mockAlert;
    private readonly Mock<IConfiguration> _mockConfig;

    public SyncServiceTests()
    {
        _mockCache = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<SyncService>>();
        _mockAlert = new Mock<IAlertService>();
        _mockConfig = new Mock<IConfiguration>();

        // ✅ FIX: Properly mock GetConnectionString
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns("Host=localhost;Database=test;");

        _mockConfig.Setup(x => x.GetSection("ConnectionStrings")).Returns(mockSection.Object);
        _mockConfig.Setup(x => x.GetConnectionString("authMgrCon")).Returns("Host=localhost;Database=test;");
    }

    [Fact]
    public async Task SyncBranchAsync_ShouldNotThrowException()
    {
        // Arrange
        var syncService = new SyncService(_mockConfig.Object, _mockLogger.Object, _mockCache.Object, _mockAlert.Object);

        var branch = new BranchDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Branch",
            Code = "TB001"
        };

        // Act
        var exception = await Record.ExceptionAsync(() => syncService.SyncBranchAsync(branch));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task SyncEmployeeAsync_ShouldNotThrowException()
    {
        // Arrange
        var syncService = new SyncService(_mockConfig.Object, _mockLogger.Object, _mockCache.Object, _mockAlert.Object);

        var employee = new EmployeeDto
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            PositionId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid()
        };

        // Act
        var exception = await Record.ExceptionAsync(() => syncService.SyncEmployeeAsync(employee));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void SyncHealthCheck_ShouldRecordSuccess()
    {
        // Arrange
        var initialStatus = SyncHealthCheck.GetStatus();
        var initialSuccessCount = initialStatus.SuccessfulSyncs;

        // Act
        SyncHealthCheck.RecordSyncSuccess();

        // Assert
        var status = SyncHealthCheck.GetStatus();
        Assert.True(status.IsHealthy);
        Assert.Equal(initialSuccessCount + 1, status.SuccessfulSyncs);
    }

    [Fact]
    public void SyncHealthCheck_ShouldRecordFailure()
    {
        // Arrange
        var initialStatus = SyncHealthCheck.GetStatus();
        var initialFailureCount = initialStatus.FailedSyncs;

        // Act
        SyncHealthCheck.RecordSyncFailure(new Exception("Test exception"));

        // Assert
        var status = SyncHealthCheck.GetStatus();
        Assert.False(status.IsHealthy);
        Assert.Equal(initialFailureCount + 1, status.FailedSyncs);
    }
}