using Dapper;
using Svc.Auth.Extensions;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Entities;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Svc.Auth.Repos;

public class AuthMngrRepo<T> : IAuthMngrRepo<T> where T : BaseEntity
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<AuthMngrRepo<T>> _logger;
    private readonly string _tableName;

    public AuthMngrRepo(DapperContext context, ILogger<AuthMngrRepo<T>> logger)
    {
        _dbConnection = context.CreateConnection();
        _logger = logger;
        _tableName = typeof(T).Name;
    }
    
    public async Task<IEnumerable<T>> GetAll()
    {
        _logger.LogInformation("Fetching all entities from {TableName} where IsDeleted = false", _tableName);
        var sql = $@"SELECT * FROM ""{_tableName}"" WHERE ""IsDeleted"" = false ORDER BY ""DateAdd"" DESC";
        return await _dbConnection.QueryAsync<T>(sql);
    }

    public async Task<T?> GetById(Guid id)
    {
        _logger.LogInformation("Fetching {Entity} by Id: {Id} from {TableName}", typeof(T).Name, id, _tableName);
        var sql = $@"SELECT * FROM ""{_tableName}"" WHERE ""Id"" = @Id AND ""IsDeleted"" = false";
        return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
    }

    public async Task<T?> GetFoD(Expression<Func<T, bool>> predicate)
    {
        _logger.LogInformation("Fetching first {Entity} matching predicate from {TableName}", typeof(T).Name, _tableName);

        var (sqlWhere, parameters) = ExpressionToSql.Parse(predicate);
        var sql = $@"SELECT * FROM ""{_tableName}"" WHERE {sqlWhere} AND ""IsDeleted"" = false LIMIT 1";
        _logger.LogDebug("Executing SQL: {Sql} with parameters: {@Params}", sql, parameters);

        return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters);
    }
    
    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        _logger.LogInformation("Fetching {Entity} matching predicate from {TableName}", typeof(T).Name, _tableName);
        var (sqlWhere, parameters) = ExpressionToSql.Parse(predicate);
        var sql = $@"SELECT * FROM ""{_tableName}"" WHERE {sqlWhere} AND ""IsDeleted"" = false";
        _logger.LogDebug("Executing SQL: {Sql} with parameters: {Params}", sql, parameters);
        return await _dbConnection.QueryAsync<T>(sql, parameters);
    }

    public async Task Add(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        entity.DateAdd = DateTime.UtcNow;

        var properties = typeof(T).GetProperties().Where(p => IsSupportedDapperType(p.PropertyType) && !IsIgnoredProperty(p)).ToList();

        var columns = string.Join(", ", properties.Select(p => $@"""{p.Name}"""));
        var paramList = string.Join(", ", properties.Select(p => "@" + p.Name));

        var sql = $@"INSERT INTO ""{_tableName}"" ({columns}, ""RowVersion"") VALUES ({paramList}, gen_random_bytes(8));";

        var paramObj = new DynamicParameters();
        foreach (var prop in properties)
        {
            paramObj.Add("@" + prop.Name, prop.GetValue(entity));
        }

        _logger.LogDebug("Inserting entity into {TableName} with Id {Id}", _tableName, entity.Id);
        await _dbConnection.ExecuteAsync(sql, paramObj);
    }

    public async Task<T> Update(T entity)
    {
        entity.DateMod = DateTime.UtcNow;

        var props = typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite && !IsIgnoredProperty(p) && IsSupportedDapperType(p.PropertyType)).ToList();
        
        var setClause = string.Join(", ", props.Select(p => $@"""{p.Name}"" = @{p.Name}"));
        var sql = $@"UPDATE ""{_tableName}"" SET {setClause} WHERE ""Id"" = @Id AND ""RowVersion"" = @RowVersionOriginal AND ""IsDeleted"" = false RETURNING *;";

        var parameters = new DynamicParameters();
        foreach (var prop in props)
        {
            parameters.Add("@" + prop.Name, prop.GetValue(entity));
        }

        parameters.Add("@Id", entity.Id);
        _logger.LogDebug("Updating entity in {TableName} with Id {Id}", _tableName, entity.Id);

        var updated = await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        if (updated == null)
        {
            _logger.LogWarning("Concurrency conflict or entity not found for update in {TableName} with Id {Id}", _tableName, entity.Id);
            throw new DBConcurrencyException("The record was modified by another user or deleted.");
        }

        return updated;
    }

    public async Task Delete(Guid id)
    {
        var sql = $@"UPDATE ""{_tableName}"" SET ""IsDeleted"" = true, ""DateMod"" = CURRENT_TIMESTAMP WHERE ""Id"" = @Id AND ""IsDeleted"" = false";
        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = id });

        if (rowsAffected == 0)
        {
            _logger.LogWarning("Entity not found for soft delete in {TableName} with Id {Id}", _tableName, id);
            throw new KeyNotFoundException("Entity not found for delete");
        }

        _logger.LogInformation("Entity soft-deleted from {TableName} with Id {Id}", _tableName, id);
    }

    private bool IsSupportedDapperType(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        return t.IsPrimitive || t == typeof(string) || t == typeof(Guid) || t == typeof(Enum) || t == typeof(int) || t == typeof(DateTime) || t == typeof(byte[]);
    }

    private bool IsIgnoredProperty(PropertyInfo p)
    {
        return !IsSupportedDapperType(p.PropertyType);
    }
}