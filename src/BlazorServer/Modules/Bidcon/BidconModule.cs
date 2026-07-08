using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Modules.Bidcon.Components;
using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Kernel.Authentication;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb;
using MudBlazor;

namespace BlazorServer.Modules.Bidcon;

public static class BidconModule
{
    public static IServiceCollection AddBidconModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
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

        services.AddMongoCollection<MongoBidconApiCredentials>("BidconApiCredentials");
        services.AddSingleton<IBidconApiCredentialsStore, MongoBidconApiCredentialsStore>();
        services.AddMongoCollection<MongoEstimationSnapshot>("EstimationSnapshots");
        services.AddSingleton<IEstimationSnapshotStore, MongoEstimationSnapshotStore>();

        registrationContext.AccessClaims.Register(new AccessClaimInfo(
            "BrowserAccess",
            "Bidconutforskaren",
            "Använda kalkylutforskaren"));
        registrationContext.AccessClaims.Register(new AccessClaimInfo("BidconApiWritePermission", "Bidcon Api Write permission", "Grants permission to edit Bidcon API credentials."));
        registrationContext.Policies.AddPolicy(
            "ApiPolicy",
            policy => policy.RequireAssertion(context => context.User.HasAccessClaimOrApplicationAdmin("BidconApiWritePermission")));

        registrationContext.UiComposition.AddTopNavigation(new UiNavigationItem(
            "Kalkyler",
            "/BidconBrowser",
            1));

        registrationContext.UiComposition.AddSettingsArea(
            new UiSettingsArea(
                "bidcon",
                "Bidcon API",
                "Configure Bidcon API credentials.",
                "/settings/bidcon",
                20,
                Icons.Material.Sharp.Api,
                "ApiPolicy"),
            typeof(BidconModule).FullName ?? nameof(BidconModule));
        return services;
    }
}
