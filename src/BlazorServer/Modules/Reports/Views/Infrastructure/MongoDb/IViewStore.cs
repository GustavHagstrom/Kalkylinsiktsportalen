using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb;

public interface IViewStore
{
    Task DeleteAsync(ViewId viewId);
    Task<List<ViewDto>> GetAllAsync(OrganizationId orgId);
    Task<ViewDto?> GetAsync(ViewId viewId);
    Task UpsertAsync(ViewDto view);
}
