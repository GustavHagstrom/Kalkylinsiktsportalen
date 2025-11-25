
using Data.Abstractions;
using MongoDB.Driver;

namespace Core.Data;
public class MongoStore<T> : IDataStore<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;

    public MongoStore(IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    public Task DeleteAsync(string id)
    {
        return _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var result = await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        return result;
    }

    public Task UpsertAsync(T entity, string id)
    {
        return _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity, new ReplaceOptions { IsUpsert = true });
    }
}
