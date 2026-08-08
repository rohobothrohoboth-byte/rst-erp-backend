// Services/Interfaces/IAuditService.cs
using System;
using System.Threading.Tasks;
using  Cor.Procurement.Persistence;
using  Cor.Procurement.Models.DTOs;
using  Cor.Procurement.Models.Enums;
using  Cor.Procurement.Models.Entities;
using  Cor.Procurement.Services;

 namespace Cor.Procurement.Services;
    public interface IAuditService
    {
        Task<AuditLog> LogAsync(AuditLog auditLog);
        Task<(IEnumerable<AuditLog> logs, int total)> GetAuditLogsAsync(
            string entityType,
            string entityId,
            int page = 1,
            int limit = 50);
    }
