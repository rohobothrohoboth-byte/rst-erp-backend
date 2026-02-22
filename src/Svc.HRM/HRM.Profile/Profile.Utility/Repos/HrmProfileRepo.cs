using Dapper;
using Microsoft.Extensions.Logging;
using Profile.App.Interfaces;
using Profile.Domain.Entities;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Profile.Utility.Repos;

public class HrmProfileRepo<T> : IHrmProfileRepo<T> where T : BaseEntity
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;
    private readonly string _tableName;
    private readonly ILogger<HrmProfileRepo<T>> _logger;

    public HrmProfileRepo(IDbConnection connection, IDbTransaction? transaction, ILogger<HrmProfileRepo<T>> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction;
        _logger = logger;
        _tableName = typeof(T).Name;
    }

    private async Task<TResult> ExecuteWithTimer<TResult>(string method, Func<Task<TResult>> action)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("{Method} START Entity = {Entity}", method, _tableName);
            var result = await action();
            sw.Stop();
            _logger.LogInformation("{Method} SUCCESS Entity={Entity} Duration={Duration}ms", method, _tableName, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "{Method} FAILED Entity={Entity} Duration={Duration}ms", method, _tableName, sw.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<T?> GetById(Guid id)
    {
        var sql = $""" SELECT * FROM "{_tableName}" WHERE "Id"=@Id AND "IsDeleted"=false """;
        return await ExecuteWithTimer(nameof(GetById), () => _connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id }, _transaction));
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        var sql = $""" SELECT * FROM "{_tableName}" WHERE "IsDeleted"=false ORDER BY "DateAdd" DESC """;
        return await ExecuteWithTimer(nameof(GetAll), () => _connection.QueryAsync<T>(sql, transaction: _transaction));
    }

    public async Task<T?> GetFoD(Expression<Func<T, bool>> predicate)
    {
        var (where, parameters) = ExpressionToSql.Parse(predicate);
        var sql = $""" SELECT * FROM "{_tableName}" WHERE {where} AND "IsDeleted"=false LIMIT 1 """;
        return await ExecuteWithTimer(nameof(GetFoD), () => _connection.QueryFirstOrDefaultAsync<T>(sql, parameters, _transaction));
    }

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        var (where, parameters) = ExpressionToSql.Parse(predicate);
        var sql = $""" SELECT * FROM "{_tableName}" WHERE {where} AND "IsDeleted"=false """;
        return await ExecuteWithTimer(nameof(Find), () => _connection.QueryAsync<T>(sql, parameters, _transaction));
    }

    public async Task Add(T entity)
    {
        entity.Id = Guid.NewGuid();
        entity.DateAdd = DateTime.UtcNow;
        var props = GetProps();
        var columns = string.Join(",", props.Select(p => $"\"{p.Name}\""));
        var values = string.Join(",", props.Select(p => $"@{p.Name}"));
        var sql = $""" INSERT INTO "{_tableName}" ({columns}, "RowVersion") VALUES ({values}, gen_random_bytes(8)) """;
        await ExecuteWithTimer(nameof(Add), () => _connection.ExecuteAsync(sql, entity, _transaction));
        _logger.LogInformation("Entity added successfully: {EntityId}", entity.Id);
    }

    public async Task<T> Update(T entity)
    {
        entity.DateMod = DateTime.UtcNow;
        var newVersion = await _connection.ExecuteScalarAsync<byte[]>("SELECT gen_random_bytes(8)", transaction: _transaction);
        var originalVersion = entity.RowVersion;
        entity.RowVersion = newVersion;
        var props = GetProps();
        var set = string.Join(",", props.Select(p => $"\"{p.Name}\"=@{p.Name}"));

        var sql = $""" UPDATE "{_tableName}" SET {set}, "RowVersion"=@RowVersion WHERE "Id"=@Id AND "RowVersion"=@OriginalVersion RETURNING * """;
        var param = new DynamicParameters(entity);
        param.Add("@OriginalVersion", originalVersion);
        var result = await ExecuteWithTimer(nameof(Update), () => _connection.QuerySingleOrDefaultAsync<T>(sql, param, _transaction));
        if (result == null)
        {
            _logger.LogWarning("Concurrency conflict while updating entity {EntityId}", entity.Id);
            throw new DBConcurrencyException("The record was modified by another user.");
        }

        _logger.LogInformation("Entity updated successfully: {EntityId}", entity.Id);
        return result;
    }

    public async Task Delete(Guid id)
    {
        var sql = $""" UPDATE "{_tableName}" SET "IsDeleted"=true, "DateMod"=CURRENT_TIMESTAMP WHERE "Id"=@Id """;
        var rowsAffected = await ExecuteWithTimer(nameof(Delete), () => _connection.ExecuteAsync(sql, new { Id = id }, _transaction));
        if (rowsAffected == 0)
        {
            _logger.LogWarning("Entity not found or already deleted in {TableName} with Id {Id}", _tableName, id);
            throw new KeyNotFoundException("Entity not found for delete");
        }
        _logger.LogInformation("Entity soft-deleted successfully: {EntityId}", id);
    }

    private static IEnumerable<PropertyInfo> GetProps()
    {
        return typeof(T).GetProperties().Where(p => p.Name != nameof(BaseEntity.RowVersion) && IsSimpleType(p.PropertyType));
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(decimal) || type == typeof(byte[]);
    }
}