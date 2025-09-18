using Dapper;
using Svc.Lup.Extensions;
using Svc.Lup.Interfaces;
using System.Data;

namespace Svc.Lup.Repositories;

public class LupRepository<T> : ILupRepository<T> where T : class
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<LupRepository<T>> _logger;
    private readonly string _tableName;

    public LupRepository(DapperContext context, ILogger<LupRepository<T>> logger)
    {
        _dbConnection = context.CreateConnection();
        _logger = logger;
        _tableName = typeof(T).Name;
    }
    
    public async Task<IEnumerable<T>> GetAll()
    {
        _logger.LogInformation("Fetching all entities from {TableName}", _tableName);
        var sql = $@"SELECT * FROM ""{_tableName}"" ORDER BY ""Name"" ASC";
        return await _dbConnection.QueryAsync<T>(sql);
    }

    public async Task<T?> GetById(Guid id)
    {
        _logger.LogInformation("Fetching {Entity} by Id: {Id} from {TableName}", typeof(T).Name, id, _tableName);
        var sql = $@"SELECT * FROM ""{_tableName}"" WHERE ""Id"" = @Id";
        return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
    }
}
