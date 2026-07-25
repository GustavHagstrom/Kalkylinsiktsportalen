using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Public.Contracts.Views;

namespace BlazorServer.Modules.Reports.Views;

public interface IViewStore
{
    Task DeleteAsync(ViewId viewId);
    Task<List<ViewDto>> GetAllAsync(OrganizationId orgId);
    Task<ViewDto?> GetAsync(ViewId viewId);
    Task UpsertAsync(ViewDto view);
}
