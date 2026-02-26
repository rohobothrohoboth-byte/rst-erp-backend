using System.Data;

namespace Profile.App.Interfaces;

public interface IUnitOfWorkNew : IAsyncDisposable, IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    Task BeginAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}