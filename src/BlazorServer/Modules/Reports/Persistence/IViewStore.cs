using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Persistence;

public interface IViewStore
{
    Task DeleteAsync(ViewId viewId);
    Task<List<View>> GetAllAsync(OrganizationId orgId);
    Task<View?> GetAsync(ViewId viewId);
    Task UpsertAsync(View view);
}
