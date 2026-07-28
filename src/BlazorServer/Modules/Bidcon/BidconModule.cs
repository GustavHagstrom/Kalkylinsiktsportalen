using BlazorServer.Composition;
using BlazorServer.Composition.Navigation;
using BlazorServer.Composition.Settings;
using BlazorServer.Modules.Bidcon.AccessTokens;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Modules.Bidcon.Components;
using BlazorServer.Modules.Bidcon.CredentialAdministration;
using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi;
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
        services.AddTransient<IBidconEstimationListSource, BidconEstimationListSource>();
        services.AddTransient<UpsertBidconCredentialsCommandHandler>();
        services.AddTransient<RemoveBidconCredentialsCommandHandler>();
        services.AddTransient<IBidconCredentialsVerifier, BidconCredentialsVerifier>();
        services.AddSingleton<IBidconAccessTokenCache, MemoryBidconAccessTokenCache>();
        services.AddSingleton<IBidconCredentialsAuditLog, MongoBidconCredentialsAuditLog>();
        services.AddTransient<ImportEstimationSnapshotCommandHandler>();
        services.AddTransient<IBidconEstimationSnapshotSource, BidconEstimationSnapshotSource>();
        services.AddTransient<BidconEstimationClient>();
        if (baseUrl is not null)
        {
            services.AddHttpClient(BidconHttpClientNames.ApiClient, client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });
        }

        services.AddBidconApiCredentialsCollection();
        services.AddBidconCredentialsAuditCollection();
        services.AddSingleton<IBidconApiCredentialsStore, MongoBidconApiCredentialsStore>();
        services.AddEstimationSnapshotsCollection();
        services.AddSingleton<IEstimationSnapshotStore, MongoEstimationSnapshotStore>();

        registrationContext.AccessClaims.Register(new AccessClaimDefinition(
            "BrowserAccess",
            "Bidconutforskaren",
            "Använda kalkylutforskaren"));
        registrationContext.AccessClaims.Register(new AccessClaimDefinition("BidconApiWritePermission", "Bidcon Api Write permission", "Grants permission to edit Bidcon API credentials."));
        registrationContext.Policies.AddPolicy(
            "ApiPolicy",
            policy => policy.RequireAccessClaimOrApplicationAdmin("BidconApiWritePermission"));

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

    private static IServiceCollection AddBidconCredentialsAuditCollection(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            const string collectionName = "BidconCredentialsAudit";
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MongoBidconCredentialsAuditEntry>(collectionName);
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
