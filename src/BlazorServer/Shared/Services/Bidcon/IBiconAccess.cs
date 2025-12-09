using BlazorServer.Shared.Models.Bidcon;

namespace BlazorServer.Shared.Services.Bidcon;

public interface IBidconAccess
{
    Task<Estimation?> GetEstimationAsync(string estimationId, string orgId);
    Task<List<Estimation>> GetAllEstimationsFlat(string userEmail, string orgId);
}