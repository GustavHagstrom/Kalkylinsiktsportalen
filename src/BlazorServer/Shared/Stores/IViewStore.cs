using BlazorServer.Shared.DTOs.Views;

namespace BlazorServer.Shared.Stores;

public interface IViewStore
{
    Task DeleteAsync(string viewId);
    Task<List<ViewDto>> GetAllAsync(string orgId);
    Task<ViewDto?> GetAsync(string viewId);
    Task UpsertAsync(ViewDto view);
}