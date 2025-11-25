using Core.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Builders;
public class MonolithConfigurator(WebApplicationBuilder builder)
{
    private readonly AccessClaimInfoBuilder _accessIdBuilder = new(builder.Services);
    private readonly ComponentBuilder _componentBuilder = new(builder.Services);
    private readonly MongoCollectionRegistrar _mongoCollectionBuilder = new(builder.Services);
    public AccessClaimInfoBuilder AccessClaimInfoBuilder => _accessIdBuilder;
    public ComponentBuilder ComponentBuilder => _componentBuilder;
    public MongoCollectionRegistrar MongoCollectionRegistrar => _mongoCollectionBuilder;

    public void AddModule<T>() where T : IModule, new()
    {
        var module = new T();
        module.GeneralConfig(builder);
        module.RegisterAccessIds(_accessIdBuilder);
        module.RegisterInjectableComponents(_componentBuilder);

        if (builder.Configuration["DbType"] == "SqlServer")
        {
            module.ConfigForEfCore(builder);
        }
        else if (builder.Configuration["DbType"] == "MongoDb")
        {
            module.ConfigForMongoDb(builder, _mongoCollectionBuilder);
        }
        else
        {
            throw new InvalidOperationException("DbType not found");
        }

        builder.Services.AddSingleton<IModule>(module);
    }
   
}
