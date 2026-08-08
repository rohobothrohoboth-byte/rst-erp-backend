using System.Data.Common;

namespace Helpers;

public static class DbReaderExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this DbDataReader reader, CancellationToken ct = default) where T : new()
    {
        var mapper = DbReaderMapper<T>.GetMapper(reader);
        var list = new List<T>();
        while (await reader.ReadAsync(ct))
        {
            list.Add(mapper(reader));
        }

        return list;
    }

    public static async Task<T?> FirstOrDefaultAsync<T>(this DbDataReader reader, CancellationToken ct = default) where T : new()
    {
        var mapper = DbReaderMapper<T>.GetMapper(reader);
        if (await reader.ReadAsync(ct))
            return mapper(reader);

        return default;
    }
}
