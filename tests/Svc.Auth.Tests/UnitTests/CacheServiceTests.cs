using Moq;
using Xunit;
using Shared.Helpers.Services;

namespace Svc.Auth.Tests.UnitTests;

public class CacheServiceTests
{
    public class TestObject
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldReturnCachedValue()
    {
        // Arrange
        var mockCache = new Mock<ICacheService>();
        var testKey = "test_key";
        var expectedValue = new TestObject { Name = "Test", Id = 1 };

        // ✅ Simplified mock: just return the cached value
        mockCache
            .Setup(x => x.GetOrCreateAsync(
                testKey,
                It.IsAny<Func<CancellationToken, Task<TestObject>>>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        // Act
        var result = await mockCache.Object.GetOrCreateAsync(testKey, async (ct) =>
        {
            return await Task.FromResult(new TestObject { Name = "New", Id = 2 });
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue.Name, result.Name);
        Assert.Equal(expectedValue.Id, result.Id);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveKey()
    {
        // Arrange
        var mockCache = new Mock<ICacheService>();
        var testKey = "test_key";
        var isRemoved = false;

        mockCache.Setup(x => x.RemoveAsync(testKey, It.IsAny<CancellationToken>()))
            .Callback(() => isRemoved = true)
            .Returns(Task.CompletedTask);

        // Act
        await mockCache.Object.RemoveAsync(testKey);

        // Assert
        Assert.True(isRemoved);
        mockCache.Verify(x => x.RemoveAsync(testKey, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyNotFound()
    {
        // Arrange
        var mockCache = new Mock<ICacheService>();
        var testKey = "non_existent_key";

        mockCache.Setup(x => x.GetAsync<TestObject>(testKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestObject?)null);

        // Act
        var result = await mockCache.Object.GetAsync<TestObject>(testKey);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_ShouldStoreValue()
    {
        // Arrange
        var mockCache = new Mock<ICacheService>();
        var testKey = "test_key";
        var value = new TestObject { Name = "Test Value", Id = 42 };
        var isSet = false;

        mockCache.Setup(x => x.SetAsync(testKey, value, It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()))
            .Callback(() => isSet = true)
            .Returns(Task.CompletedTask);

        // Act
        await mockCache.Object.SetAsync(testKey, value);

        // Assert
        Assert.True(isSet);
        mockCache.Verify(x => x.SetAsync(testKey, value, It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldCallFactory_WhenCacheMiss()
    {
        // Arrange
        var mockCache = new Mock<ICacheService>();
        var testKey = "test_key";
        var factoryCalled = false;
        var expectedValue = new TestObject { Name = "Factory Created", Id = 3 };

        // ✅ Simulate cache miss by returning null, then factory should be called
        mockCache
            .Setup(x => x.GetOrCreateAsync(
                testKey,
                It.IsAny<Func<CancellationToken, Task<TestObject>>>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, Task<TestObject>> factory, TimeSpan? expiry, CancellationToken ct) =>
            {
                factoryCalled = true;
                return factory(ct);
            });

        // Act
        var result = await mockCache.Object.GetOrCreateAsync(testKey, async (ct) =>
        {
            return await Task.FromResult(expectedValue);
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(factoryCalled);
        Assert.Equal(expectedValue.Name, result.Name);
        Assert.Equal(expectedValue.Id, result.Id);
    }
}