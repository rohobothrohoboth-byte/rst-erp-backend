// Leave.App/Interfaces/IHrmProDapperHelper.cs
using System.Data.Common;
namespace Leave.App.Interfaces;

public interface IHrmProDapperHelper
{
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default);
}