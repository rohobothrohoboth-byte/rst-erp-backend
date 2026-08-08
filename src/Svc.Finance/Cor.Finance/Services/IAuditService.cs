// Services/Interfaces/IAuditService.cs
using System;
using System.Threading.Tasks;
using  Cor.Finance.Persistence;
using  Cor.Finance.Models.DTOs;
using  Cor.Finance.Models.Enums;
using  Cor.Finance.Models.Entities;
using  Cor.Finance.Services;

 namespace Cor.Finance.Services;
    public interface IAuditService
    {
        Task<AuditLog> LogAsync(AuditLog auditLog);
        Task<(IEnumerable<AuditLog> logs, int total)> GetAuditLogsAsync(
            string entityType,
            string entityId,
            int page = 1,
            int limit = 50);
    }
