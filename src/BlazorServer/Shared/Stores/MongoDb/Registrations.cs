using BlazorServer.Shared.Stores.MongoDb.Enteties;
using BlazorServer.Shared.Stores.MongoDb.Stores;
using MongoDB.Driver;

namespace BlazorServer.Shared.Stores.MongoDb;

public static class Registrations
{
    
    static IServiceCollection RegisterSharedMongoCollections(this IServiceCollection services)
    {
        services.AddMongoCollection<MongoUserSettings>("UserSettings");
        services.AddMongoCollection<MongoView>("Views");
        return services;
    }
    static IServiceCollection RegisterSharedMongoStores(this IServiceCollection services)
    {
        services.AddSingleton<IUserSettingsStore, MongoUserSettingsStore>();
        services.AddSingleton<IDarkmodeStore, MongoUserSettingsStore>();
        services.AddSingleton<IViewStore, MongoViewStore>();
        return services;
    }










    public static void UseMongoDb(this WebApplicationBuilder builder)
    {
        var mongoConnectionString = builder.Configuration.GetValue<string>("MongoDb:ConnectionString");
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDb:DatabaseName");
        var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
        builder.Services.AddSingleton<IMongoClient>(mongoClient);
        builder.Services.AddSingleton(mongoDatabase);

        builder.Services.RegisterSharedMongoCollections();
        builder.Services.RegisterSharedMongoStores();
    }
    public static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, string collectionName)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }
            var collection = database.GetCollection<T>(collectionName);
            return collection;
        });
        return services;
    }
}
