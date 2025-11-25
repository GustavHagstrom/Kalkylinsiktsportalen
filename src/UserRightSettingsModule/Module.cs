using Core.Constants;
using Core.Abstractions;
using Core.Builders;
using UserRightSettingsModule.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserRightSettingsModule.Data.Abstractions;
using UserRightSettingsModule.Data.EfCore;
using Microsoft.Extensions.Hosting;
using UserRightSettingsModule.Data.Mongo;

namespace UserRightSettingsModule;
public class Module : IModule
{

    public void RegisterAccessIds(AccessClaimInfoBuilder accessIdBuilder)
    {
        accessIdBuilder.AddAccessClaimInfo<ModuleAccessClaimInfo>();
    }
    public void RegisterInjectableComponents(ComponentBuilder componentBuilder)
    {
        componentBuilder.AddSettingsNavigationComponent<SettingsLink>();
    }
    public void GeneralConfig(WebApplicationBuilder builder)
    {
        
        builder.Services.AddAuthorization(configure =>
        {
            configure.AddPolicy(Constants.AuthorizationConstants.Policy, policy =>
            {
                policy.RequireAuthenticatedUser();
                if (builder.Environment.IsProduction())
                {
                    policy.RequireClaim(SharedApplicationClaimTypes.AccessClaim, Constants.AuthorizationConstants.AccessClaimValue);
                }
            });
        });
        
    }
    public void ConfigForEfCore(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAccessStore, SqlAccessStore>();
        builder.Services.AddScoped<IRoleStore, SqlRoleStore>();
    }
    public void OnEfCoreModelCreating(ModelBuilder modelBuilder)
    {

    }
    public void ConfigForMongoDb(WebApplicationBuilder builder, MongoCollectionRegistrar collectionBuilder)
    {
        builder.Services.AddScoped<IAccessStore, MongoAccessStore>();
        builder.Services.AddScoped<IRoleStore, MongoRoleStore>();
    }
    
}
