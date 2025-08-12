using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Cor.Utility.Extensions
{
    public class DapperContext
    {
        private readonly string _connectionString;
        private IDbConnection? _connection;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CoreDbCon") ?? throw new InvalidOperationException("Connection string 'CoreDbCon' is not configured.");
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
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
}