using Core.Abstractions;
using Core.Builders;
using Core.Constants;
using Core.Services;
using Data.Abstractions;
using Data.EfCore.Stores;
using SharedModule.Components.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Core.Extensions;
using Microsoft.Graph;
using Core.Models.FromShared;
using SharedModule.Services;
using SharedModule.Services.MicrosoftGraph;
using SharedModule.Services.Views;
using Data.MongoDb.Stores;
using MongoEnteties = Data.MongoDb.Enteties;
namespace SharedModule;
public class Module : IModule
{
    public void ConfigForEfCore(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDarkModeStore, SqlDarkModeStore>();
        builder.Services.AddTransient<IAccessClaimStore, SqlAccessClaimStore>();
        builder.Services.AddScoped<IViewAccessStore, SqlViewAccessStore>();
    }

    public void ConfigForMongoDb(WebApplicationBuilder builder, MongoCollectionRegistrar collectionBuilder)
    {
        collectionBuilder.Add<MongoEnteties.UserSettings>("UserSettings");
        collectionBuilder.Add<MongoEnteties.Authorization.Role>("Roles");
        collectionBuilder.Add<MongoEnteties.Authorization.UserAccess>("UserAccess");
        collectionBuilder.Add<MongoEnteties.Authorization.UserRole>("UserRoles");
        collectionBuilder.Add<MongoEnteties.EstimationView.RoleView>("RoleViews");
        collectionBuilder.Add<MongoEnteties.EstimationView.UserView>("UserViews");
        collectionBuilder.Add<MongoEnteties.EstimationView.View>("Views");
        collectionBuilder.Add<MongoEnteties.BidconAccessCredentials>("BidconAccessCredentials");


        builder.Services.AddScoped<IDarkModeStore, MongoDarkModeStore>();
        builder.Services.AddTransient<IAccessClaimStore, MongoAccessClaimStore>();
        builder.Services.AddScoped<IViewAccessStore, MongoViewAccessStore>();

    }

    public void GeneralConfig(WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization();

        builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
        builder.Services.AddSingleton<IApplicationRenderComponentsService, ApplicationRenderComponentsService>();
        builder.Services.AddApplicationInjectableComponent<AppbarSettingsLink>(SharedInjectableComponentKeys.AppBarNavLinkComponent);
        builder.Services.AddSingleton<IAccessClaimInfoContainer, AccessClaimInfoContainer>();
        builder.Services.AddAccessClaimInfo<BidconConnectionClaimInfo>();
        builder.Services.AddTransient<IAuthenticationProvider, GraphClientAuthProvider>();
        builder.Services.AddScoped(sp => new GraphServiceClient(sp.GetRequiredService<IAuthenticationProvider>()));
        builder.Services.AddScoped<IMicrosoftGraphUserAccess, GraphClientUserAccess>();
        builder.Services.AddTransient<IViewStyleService, ViewStyleService>();
        builder.Services.AddScoped<ISheetDataService, SheetDataService>();
        builder.Services.AddAuthorization(configure =>
        {
            configure.AddPolicy(SharedAuthorizationPolicies.BidconConnection, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(SharedApplicationClaimTypes.AccessClaim, SharedAccessClaimValues.BidconConnection);
            });
        });
    }

    public void OnEfCoreModelCreating(ModelBuilder modelBuilder)
    {

    }

    public void RegisterAccessIds(AccessClaimInfoBuilder accessIdBuilder)
    {

    }

    public void RegisterInjectableComponents(ComponentBuilder componentBuilder)
    {

    }
}
