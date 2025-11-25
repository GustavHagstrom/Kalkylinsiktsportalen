namespace Data.Abstractions;
public interface IDataStore<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task UpsertAsync(T entity, string id);
    Task DeleteAsync(string id);
}
