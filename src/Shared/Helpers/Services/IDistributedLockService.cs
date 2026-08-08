// src/Shared/Helpers/Services/IDistributedLockService.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Helpers.Services;

public interface IDistributedLockService
{
    Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiry, CancellationToken ct = default);
    Task ReleaseLockAsync(string lockKey, CancellationToken ct = default);
}

