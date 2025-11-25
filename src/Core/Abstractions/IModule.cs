using Core.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Core.Abstractions;
public interface IModule
{
    void GeneralConfig(WebApplicationBuilder builder);
    void ConfigForEfCore(WebApplicationBuilder builder);
    void OnEfCoreModelCreating(ModelBuilder modelBuilder);
    void ConfigForMongoDb(WebApplicationBuilder builder, MongoCollectionRegistrar collectionBuilder);
    void RegisterAccessIds(AccessClaimInfoBuilder accessIdBuilder);
    void RegisterInjectableComponents(ComponentBuilder componentBuilder);
}
