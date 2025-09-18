using Dapper;
using Microsoft.Extensions.Logging;
using Module.App.Interfaces;
using Module.Domain.Entities;
using Module.Utility.Extensions;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Module.Utility.Repositories;

public class DynamicQuery
{
    public string Sql { get; set; } = string.Empty;
    public DynamicParameters Parameters { get; set; } = new DynamicParameters();
}

public class CoreModuleRepo<T> : ICoreModuleRepo<T> where T : BaseEntity
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<CoreModuleRepo<T>> _logger;
    private readonly string _tableName;

    public CoreModuleRepo(DapperContext context, ILogger<CoreModuleRepo<T>> logger)
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
        var query = BuildDynamicQuery(predicate);
        _logger.LogDebug("Executing SQL: {Sql} with parameters: {Params}", query.Sql, query.Parameters);
        return await _dbConnection.QueryFirstOrDefaultAsync<T>(query.Sql, query.Parameters);
    }

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        _logger.LogInformation("Fetching {Entity} matching predicate from {TableName}", typeof(T).Name, _tableName);
        var query = BuildDynamicQuery(predicate);
        _logger.LogDebug("Executing SQL: {Sql} with parameters: {Params}", query.Sql, query.Parameters);
        return await _dbConnection.QueryAsync<T>(query.Sql, query.Parameters);
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
        var newRowVersion = await _dbConnection.ExecuteScalarAsync<byte[]>("SELECT gen_random_bytes(8);") ?? throw new InvalidOperationException("Failed to generate RowVersion.");
        var originalRowVersion = entity.RowVersion;
        entity.RowVersion = newRowVersion;
        entity.DateMod = DateTime.UtcNow;

        var props = typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite && !IsIgnoredProperty(p) && IsSupportedDapperType(p.PropertyType)).ToList();
        props.Add(typeof(T).GetProperty(nameof(BaseEntity.RowVersion))!);

        var setClause = string.Join(", ", props.Select(p => $@"""{p.Name}"" = @{p.Name}"));
        var sql = $@"UPDATE ""{_tableName}"" SET {setClause} WHERE ""Id"" = @Id AND ""RowVersion"" = @RowVersionOriginal AND ""IsDeleted"" = false RETURNING *;";

        var parameters = new DynamicParameters();
        foreach (var prop in props)
        {
            parameters.Add("@" + prop.Name, prop.GetValue(entity));
        }

        parameters.Add("@Id", entity.Id);
        parameters.Add("@RowVersionOriginal", originalRowVersion, DbType.Binary);
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
        //return p.Name is nameof(BaseEntity.RowVersion) or "StartDate" or "EndDate" or "StartDateAm" or "EndDateAm" || !IsSupportedDapperType(p.PropertyType);
        return p.Name is nameof(BaseEntity.RowVersion) || !IsSupportedDapperType(p.PropertyType);
    }

    private DynamicQuery BuildDynamicQuery(Expression<Func<T, bool>> predicate)
    {
        var parameters = new DynamicParameters();
        var sb = new StringBuilder();
        int index = 0;

        void Parse(Expression expr)
        {
            if (expr is BinaryExpression be)
            {
                if (be.NodeType == ExpressionType.Equal)
                {
                    if (be.Left is MemberExpression member)
                    {
                        var value = Expression.Lambda(be.Right).Compile().DynamicInvoke();
                        var paramName = $"@p{index++}";
                        sb.Append($@"""{member.Member.Name}"" = {paramName}");
                        parameters.Add(paramName, value);
                    }
                    else
                        throw new NotSupportedException("Left side must be a property.");
                }
                else if (be.NodeType == ExpressionType.AndAlso || be.NodeType == ExpressionType.OrElse)
                {
                    sb.Append("(");
                    Parse(be.Left);
                    sb.Append(be.NodeType == ExpressionType.AndAlso ? " AND " : " OR ");
                    Parse(be.Right);
                    sb.Append(")");
                }
                else
                    throw new NotSupportedException($"Operator {be.NodeType} not supported.");
            }
            else
                throw new NotSupportedException($"Expression type {expr.GetType().Name} not supported.");
        }

        Parse(predicate.Body);

        return new DynamicQuery
        {
            Sql = $@"SELECT * FROM ""{_tableName}"" WHERE ""IsDeleted"" = false AND {sb}",
            Parameters = parameters
        };
    }

    private void ParseExpression(Expression expression, StringBuilder whereClause, DynamicParameters parameters, ref int paramIndex)
    {
        if (expression is BinaryExpression binary)
        {
            if (binary.NodeType == ExpressionType.Equal)
            {
                if (binary.Left is MemberExpression member && binary.Right is ConstantExpression constant)
                {
                    var columnName = member.Member.Name;
                    var paramName = $"@p{paramIndex++}";
                    whereClause.Append($@"""{columnName}"" = {paramName}");
                    parameters.Add(paramName, constant.Value);
                }
                else
                {
                    throw new NotSupportedException($"Expression type {binary.Right.GetType().Name} not supported.");
                }
            }
            else if (binary.NodeType == ExpressionType.AndAlso || binary.NodeType == ExpressionType.OrElse)
            {
                whereClause.Append("(");
                ParseExpression(binary.Left, whereClause, parameters, ref paramIndex);
                whereClause.Append(binary.NodeType == ExpressionType.AndAlso ? " AND " : " OR ");
                ParseExpression(binary.Right, whereClause, parameters, ref paramIndex);
                whereClause.Append(")");
            }
            else
            {
                throw new NotSupportedException($"Binary operator {binary.NodeType} not supported.");
            }
        }
        else
        {
            throw new NotSupportedException($"Expression type {expression.GetType().Name} not supported.");
        }
    }
}
