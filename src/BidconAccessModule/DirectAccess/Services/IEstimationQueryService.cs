using BidconAccessModule.DirectAccess.Models;
using System.Security.Claims;

namespace BidconAccessModule.DirectAccess.Services;
internal interface IEstimationQueryService
{
    Task<EstimationBatch> GetEstimationBatchAsync(string estimationId, string tenantId);
    Task<IEnumerable<EstimationBatch>> GetEstimationBatchesAsync(IEnumerable<string> estimationIds, string tenantId);
    Task<IEnumerable<B_Estimation>> GetEstimationListAsync(string tenantId);
    Task<EstimationFolderBatch> GetFolderBatchAsync(string tenantId);
}