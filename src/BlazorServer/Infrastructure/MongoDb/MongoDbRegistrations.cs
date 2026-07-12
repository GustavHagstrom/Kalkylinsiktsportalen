using MongoDB.Driver;

namespace BlazorServer.Infrastructure.MongoDb;

public static class MongoDbRegistrations
{
    public static void UseMongoDb(this WebApplicationBuilder builder)
    {
        var mongoConnectionString = builder.Configuration.GetValue<string>("MongoDb:ConnectionString");
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDb:DatabaseName");
        var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
        builder.Services.AddSingleton<IMongoClient>(mongoClient);
        builder.Services.AddSingleton(mongoDatabase);
    }
}
