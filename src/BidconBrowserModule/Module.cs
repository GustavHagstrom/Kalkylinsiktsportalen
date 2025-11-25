using BidconBrowserModule.Components;
using Core.Constants;
using Core.Abstractions;
using Core.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BidconBrowserModule.Services;

namespace BidconBrowserModule;
public class Module : IModule
{
    public void RegisterAccessIds(AccessClaimInfoBuilder accessIdBuilder)
    {
        accessIdBuilder.AddAccessClaimInfo<ModuleClaimInfo>();
    }
    public void RegisterInjectableComponents(ComponentBuilder componentBuilder)
    {
        componentBuilder.AddAppBarNavLinkComponent<BidconBrowserAppLink>();
    }
    public void GeneralConfig(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IBidconBrowserAccesService, BidconBrowserAccesService>();
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Constants.Authorization.Policy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(SharedApplicationClaimTypes.AccessClaim, Constants.Authorization.AccessClaimValue);
            });
    }

    public void ConfigForEfCore(WebApplicationBuilder builder)
    {
        
    }
    public void OnEfCoreModelCreating(ModelBuilder modelBuilder)
    {

    }
    public void ConfigForMongoDb(WebApplicationBuilder builder, MongoCollectionRegistrar collectionBuilder)
    {
        
    }
}
