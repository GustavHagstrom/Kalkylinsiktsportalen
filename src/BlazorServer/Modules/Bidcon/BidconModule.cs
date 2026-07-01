using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Stores.MongoDb;

namespace BlazorServer.Modules.Bidcon;

public static class BidconModule
{
    public static IServiceCollection AddBidconModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        services.AddTransient<GetBidconCredentialsQueryHandler>();
        services.AddTransient<UpsertBidconCredentialsCommandHandler>();
        services.AddTransient<RemoveBidconCredentialsCommandHandler>();
        services.AddTransient<IBidconCredentialsVerifier, BidconCredentialsVerifier>();
        services.AddSingleton<IBidconAccessTokenCache, MemoryBidconAccessTokenCache>();
        services.AddTransient<ImportEstimationSnapshotCommandHandler>();
        services.AddTransient<BidconEstimationDetailsClient>();

        services.AddMongoCollection<MongoBidconApiCredentials>("BidconApiCredentials");
        services.AddSingleton<IBidconApiCredentialsStore, MongoBidconApiCredentialsStore>();
        services.AddMongoCollection<MongoEstimationSnapshot>("EstimationSnapshots");
        services.AddSingleton<IEstimationSnapshotStore, MongoEstimationSnapshotStore>();

        return services;
    }
}
