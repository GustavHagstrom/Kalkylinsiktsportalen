using Core.Constants;
using Core.Abstractions;
using Core.Builders;
using ViewSettingsModule.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ViewSettingsModule.Data.Abstractions;
using ViewSettingsModule.Data.EfCore;
using ViewSettingsModule.Data.Mongo;

namespace ViewSettingsModule;
public class Module : IModule
{
    public void RegisterAccessIds(AccessClaimInfoBuilder builder)
    {
        builder.AddAccessClaimInfo<ViewSettingsClaimInfo>();
    }
    public void RegisterInjectableComponents(ComponentBuilder componentBuilder)
    {
        componentBuilder.AddSettingsNavigationComponent<SettingsLink>();
    }

    public void GeneralConfig(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<Helper>();
        builder.Services.AddAuthorization(configure =>
        {
            configure.AddPolicy(Constants.Authorization.Policy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(SharedApplicationClaimTypes.AccessClaim, Constants.Authorization.AccessClaimValue);
            });
        });
    }

    public void ConfigForEfCore(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IViewStore, SqlViewStore>();
        builder.Services.AddScoped<IRoleViewStore, SqlRoleViewStore>();
        builder.Services.AddScoped<IUserViewStore, SqlUserViewStore>();
    }
    public void OnEfCoreModelCreating(ModelBuilder modelBuilder)
    {

    }
    public void ConfigForMongoDb(WebApplicationBuilder builder, MongoCollectionRegistrar collectionBuilder)
    {
        builder.Services.AddScoped<IViewStore, MongoViewStore>();
        builder.Services.AddScoped<IRoleViewStore, MongoRoleViewStore>();
        builder.Services.AddScoped<IUserViewStore, MongoUserViewStore>();

    }
}
