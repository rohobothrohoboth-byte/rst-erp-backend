using Dapper;
using Svc.Auth.Extensions;
using Svc.Auth.Interfaces;

namespace Svc.Auth.Repos;

public class GenericDapperRepository<T> : IGenericRepository<T> where T : class
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _tableName;

    public GenericDapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _tableName = typeof(T).Name.ToLower() + "s"; // Simple pluralization
    }

    public async Task AddAsync(T entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        var insertQuery = $"INSERT INTO {_tableName} VALUES (@*);"; // placeholder, override in specific repo
        await conn.ExecuteAsync(insertQuery, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = $"DELETE FROM {_tableName} WHERE id = @Id";
        await conn.ExecuteAsync(query, new { Id = id });
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = $"SELECT * FROM {_tableName}";
        return await conn.QueryAsync<T>(query);
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        var query = $"SELECT * FROM {_tableName} WHERE id = @Id";
        return await conn.QuerySingleOrDefaultAsync<T>(query, new { Id = id });
    }

    public async Task UpdateAsync(T entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        var updateQuery = $"UPDATE {_tableName} SET /* fields */ WHERE id = @Id"; // placeholder, override in specific repo
        await conn.ExecuteAsync(updateQuery, entity);
    }
}
