using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace Profile.App.Helpers;

public class DapperCxtHelper : IAsyncDisposable, IDisposable
{
    private readonly string _connectionString;  // internal connection string
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    // Constructor uses IConfiguration directly
    public DapperCxtHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("HRMProDbCon") ?? throw new InvalidOperationException("Connection string 'HRMProDbCon' is not configured.");
    }

    /// <summary>
    /// Get an open connection (lazy) and optionally start a transaction
    /// </summary>
    public async Task<IDbConnection> GetOpenConnectionAsync(bool beginTransaction = false, CancellationToken ct = default)
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
            await ((DbConnection)_connection).OpenAsync(ct);
        }

        if (beginTransaction && _transaction == null)
            _transaction = await ((DbConnection)_connection).BeginTransactionAsync(ct);

        return _connection;
    }

    public IDbTransaction? Transaction => _transaction;

    public async Task CommitAsync()
    {
        if (_transaction == null) return;

        if (_transaction is DbTransaction dbTx) await dbTx.CommitAsync();
        else _transaction.Commit();

        await DisposeTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null) return;

        if (_transaction is DbTransaction dbTx) await dbTx.RollbackAsync();
        else _transaction.Rollback();

        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is IAsyncDisposable asyncTx) await asyncTx.DisposeAsync();
        else _transaction?.Dispose();

        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync();

        if (_connection is IAsyncDisposable asyncConn) await asyncConn.DisposeAsync();
        else _connection?.Dispose();

        _connection = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();

        _transaction = null;
        _connection = null;
    }
}