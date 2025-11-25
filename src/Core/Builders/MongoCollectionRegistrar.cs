using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Core.Builders;
public class MongoCollectionRegistrar(IServiceCollection services)
{
    private readonly Dictionary<Type, string> _collectionNames = [];
    public void Add<T>(string collectionName)
    {
        var type = typeof(T);
        if (_collectionNames.ContainsKey(type))
        {
            throw new InvalidOperationException($"Collection name for type {type.Name} already exists");
        }
        _collectionNames.Add(type, collectionName);
        services.AddSingleton(sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            var collectionNames = db.ListCollectionNames().ToList();
            if (!collectionNames.Contains(collectionName))
            {
                db.CreateCollection(collectionName);
            }
            return db.GetCollection<T>(collectionName);
        });
    }
}
