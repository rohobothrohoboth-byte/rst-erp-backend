using System.Data;
using Npgsql;

namespace Svc.Auth.Extensions;

public class DapperContext : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("authMgrCon") ?? throw new InvalidOperationException("Connection string 'authMgrCon' is not configured.");
    }

    public IDbConnection CreateConnection()
    {
        _connection = new NpgsqlConnection(_connectionString);
        return _connection;
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}