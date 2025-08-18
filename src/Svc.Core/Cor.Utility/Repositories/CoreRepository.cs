using Cor.App.Interfaces;
using Cor.Domain.Entities;
using Cor.Utility.Extensions;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Cor.Utility.Repositories
{
    public class CoreRepository<T> : ICoreRepository<T> where T : BaseEntity
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<CoreRepository<T>> _logger;
        private readonly string _tableName;

        public CoreRepository(DapperContext context, ILogger<CoreRepository<T>> logger)
        {
            _dbConnection = context.CreateConnection();
            _logger = logger;
            _tableName = typeof(T).Name + "s"; // assumes plural by adding 's'
        }
        
        public async Task<IEnumerable<T>> GetAll()
        {
            _logger.LogInformation("Fetching all entities from {TableName}", _tableName);
            var sql = $@"SELECT * FROM ""{_tableName}"" ORDER BY ""DateAdd"" DESC";
            return await _dbConnection.QueryAsync<T>(sql);
        }

        public async Task<T?> GetById(Guid id)
        {
            _logger.LogInformation("Fetching {Entity} by Id: {Id}", typeof(T).Name, id);
            var sql = $@"SELECT * FROM ""{_tableName}"" WHERE ""Id"" = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<T?> GetFoD(Expression<Func<T, bool>> predicate)
        {
            var all = await GetAll();
            return all.AsQueryable().FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            var all = await GetAll();
            return all.AsQueryable().Where(predicate);
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
            var sql = $@"UPDATE ""{_tableName}"" SET {setClause} WHERE ""Id"" = @Id AND ""RowVersion"" = @RowVersionOriginal RETURNING *;";

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
                throw new DBConcurrencyException("The record was modified by another user.");
            }

            return updated;
        }
        
        public async Task Delete(Guid id)
        {
            var sql = $@"DELETE FROM ""{_tableName}"" WHERE ""Id"" = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = id });

            if (rowsAffected == 0)
                throw new KeyNotFoundException("Entity not found for delete");

            _logger.LogInformation("Entity deleted from {TableName} with Id {Id}", _tableName, id);
        }
        
        private bool IsSupportedDapperType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t.IsPrimitive || t == typeof(string) || t == typeof(Guid) || t == typeof(DateTime) || t == typeof(byte[]);
        }

        private bool IsIgnoredProperty(PropertyInfo p)
        {
            return p.Name is nameof(BaseEntity.CreatedAt)
                or nameof(BaseEntity.ModifiedAt)
                or nameof(BaseEntity.CreatedAtAm)
                or nameof(BaseEntity.ModifiedAtAm)
                or nameof(BaseEntity.RowVersion) or "StartDate" or "EndDate" or "StartDateAm" or "EndDateAm"
                || !IsSupportedDapperType(p.PropertyType);
        }
    }
}
