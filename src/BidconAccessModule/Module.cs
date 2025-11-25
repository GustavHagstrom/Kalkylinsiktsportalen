using BidconAccessModule.SdkAccess.Services;
using Core.Constants;
using Core.Abstractions;
using Core.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BidconAccessModule.DirectAccess.Data.EfCore;
using BidconAccessModule.DirectAccess.Data.Abstractions;
using BidconAccessModule.DirectAccess.Data.Mongo;

namespace BidconAccessModule;
public class Module : IModule
{
    public void RegisterAccessIds(AccessClaimInfoBuilder accessIdBuilder)
    {
        accessIdBuilder.AddAccessClaimInfo<ModuleClaimInfo>();
    }

    public void RegisterInjectableComponents(ComponentBuilder componentBuilder)
    {

    }

    public void GeneralConfig(WebApplicationBuilder builder)
    {

        builder.Services.AddScoped<IBidconAccess, BidconSdkReflectionAccess>();
        builder.Services.AddScoped<IBidconReflectionService, BidconReflectionService>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Constants.Authorization.EditBidconConnectionPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(SharedApplicationClaimTypes.AccessClaim, Constants.Authorization.AccessClaimValue);
            });
    }
    public void ConfigForEfCore(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDirectCredentialsStore, SqlDirectCredentialsStore>();
    }

    public void ConfigForMongoDb(WebApplicationBuilder builder, MongoCollectionRegistrar collectionBuilder)
    {
        builder.Services.AddTransient<IDirectCredentialsStore, MongoDirectCredentialsStore>();
    }


    public void OnEfCoreModelCreating(ModelBuilder modelBuilder)
    {

    }
}
