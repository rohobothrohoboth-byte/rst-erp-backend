using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cor.CRM.Services
{
    public interface ILeadScoringService
    {
        Task<int> CalculateLeadScoreAsync(Guid leadId, CancellationToken ct = default);
        Task UpdateLeadScoreAsync(Guid leadId, CancellationToken ct = default);
    }
}