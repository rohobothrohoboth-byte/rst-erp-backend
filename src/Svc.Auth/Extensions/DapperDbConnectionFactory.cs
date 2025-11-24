using System.Data;
using Npgsql;

namespace Svc.Auth.Extensions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DapperDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DapperDbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres") ?? throw new ArgumentNullException("Postgres connection string not found");
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
