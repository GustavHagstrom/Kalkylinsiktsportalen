namespace BlazorServer.Shared.Stores;
public interface IGenericDataStore<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string orgId);
    Task<T?> GetByIdAsync(string id, string orgId);
    Task UpsertAsync(T model, string id, string orgId);
    Task DeleteAsync(string id, string orgId);
}
