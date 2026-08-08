// Services/AuditService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Cor.Finance.Persistence;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Enums;
using Cor.Finance.Models.Entities;
using Cor.Finance.Services;

namespace Cor.Finance.Services;

public class AuditService : IAuditService
{
    private readonly FinanceDbContext _context;

    public AuditService(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> LogAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<(IEnumerable<AuditLog> logs, int total)> GetAuditLogsAsync(
        string entityType,
        string entityId,
        int page = 1,
        int limit = 50)
    {
        var query = _context.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId);

        var total = await query.CountAsync();

        var logs = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (logs, total);
    }

    // ✅ FIX: Return nullable string
    public static string? SerializeObject(object? obj)
    {
        if (obj == null) return null;
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        });
    }

    // ✅ FIX: Return nullable T with proper handling
    public static T? DeserializeObject<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // ✅ ADD: For value types (int, decimal, etc.)
    public static T? DeserializeValue<T>(string? json) where T : struct
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}