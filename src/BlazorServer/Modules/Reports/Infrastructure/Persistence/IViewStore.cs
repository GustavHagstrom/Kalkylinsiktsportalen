using BlazorServer.Modules.Reports.Public.Contracts.Views;

namespace BlazorServer.Modules.Reports.Infrastructure.Persistence;

public interface IViewStore
{
    Task DeleteAsync(string viewId);
    Task<List<ViewDto>> GetAllAsync(string orgId);
    Task<ViewDto?> GetAsync(string viewId);
    Task UpsertAsync(ViewDto view);
}