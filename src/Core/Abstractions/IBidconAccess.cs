using Core.Models.EstimationEnteties;
using Core.Models.FromShared;

namespace Core.Abstractions;
public interface IBidconAccess
{
    Task<Folder> GetFolderRootAsync(string tenantId);
    Task<Estimation> GetEstimationAsync(string estimationId, string tenantId);

}
