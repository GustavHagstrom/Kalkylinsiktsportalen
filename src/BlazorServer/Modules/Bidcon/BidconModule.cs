using BlazorServer.AppShell.Composition;
using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition.Settings;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Modules.Bidcon.Components;
using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Authentication;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Public;
using MongoDB.Driver;
using MudBlazor;

namespace BlazorServer.Modules.Bidcon;

public static class BidconModule
{
    public static IServiceCollection AddBidconModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext,
        string? baseUrl = null)
    {
        services.AddTransient<GetBidconCredentialsQueryHandler>();
        services.AddTransient<GetEstimationListQueryHandler>();
        services.AddTransient<UpsertBidconCredentialsCommandHandler>();
        services.AddTransient<RemoveBidconCredentialsCommandHandler>();
        services.AddTransient<IBidconCredentialsVerifier, BidconCredentialsVerifier>();
        services.AddSingleton<IBidconAccessTokenCache, MemoryBidconAccessTokenCache>();
        services.AddTransient<ImportEstimationSnapshotCommandHandler>();
        services.AddTransient<BidconEstimationClient>();
        services.AddScoped<EstimationListState>();
        if (baseUrl is not null)
        {
            services.AddHttpClient(BidconHttpClientNames.ApiClient, client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });
        }

        services.AddBidconApiCredentialsCollection();
        services.AddSingleton<IBidconApiCredentialsStore, MongoBidconApiCredentialsStore>();
        services.AddEstimationSnapshotsCollection();
        services.AddSingleton<IEstimationSnapshotStore, MongoEstimationSnapshotStore>();

        registrationContext.AccessClaims.Register(new AccessClaimInfo(
            "BrowserAccess",
            "Bidconutforskaren",
            "Använda kalkylutforskaren"));
        registrationContext.AccessClaims.Register(new AccessClaimInfo("BidconApiWritePermission", "Bidcon Api Write permission", "Grants permission to edit Bidcon API credentials."));
        registrationContext.Policies.AddPolicy(
            "ApiPolicy",
            policy => policy.RequireAssertion(context => context.User.HasAccessClaimOrApplicationAdmin("BidconApiWritePermission")));

        registrationContext.Ui.AddTopNavigation(new NavigationItem(
            "Kalkyler",
            BidconRoutes.Browser,
            1));

        registrationContext.Ui.AddSettingsArea(
            new SettingsAreaContribution(
                "bidcon",
                "Bidcon API",
                "Configure Bidcon API credentials.",
                BidconRoutes.Settings,
                20,
                Icons.Material.Sharp.Api,
                "ApiPolicy"),
            typeof(BidconModule).FullName ?? nameof(BidconModule));
        return services;
    }

    private static IServiceCollection AddBidconApiCredentialsCollection(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            const string collectionName = "BidconApiCredentials";
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MongoBidconApiCredentials>(collectionName);
        });
        return services;
    }

    private static IServiceCollection AddEstimationSnapshotsCollection(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            const string collectionName = "EstimationSnapshots";
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MongoEstimationSnapshot>(collectionName);
        });
        return services;
    }
}
