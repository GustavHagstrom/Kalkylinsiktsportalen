using Core.Models.EstimationView;
using System.Security.Claims;

namespace ViewSettingsModule.Data.Abstractions;
internal interface IViewStore
{
    Task<View?> GetAsync(string tenantId, string viewId);
    Task DeleteAsync(string tenantId, View view);
    Task UpsertAsync(string tenantId, View view);
    Task<List<View>> GetViewListAsync(string tenantId);
}
