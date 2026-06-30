using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Registration;
using BlazorServer.Shared.Stores.MongoDb;

namespace BlazorServer.Modules.Bidcon;

public sealed class BidconModule : IModule
{
    public void Register(ModuleRegistration module)
    {
        module.Services.AddTransient<GetBidconCredentialsQueryHandler>();
        module.Services.AddTransient<UpsertBidconCredentialsCommandHandler>();
        module.Services.AddTransient<ImportEstimationSnapshotCommandHandler>();
        module.Services.AddTransient<BidconEstimationDetailsClient>();

        module.Services.AddMongoCollection<MongoBidconApiCredentials>("BidconApiCredentials");
        module.Services.AddSingleton<IBidconApiCredentialsStore, MongoBidconApiCredentialsStore>();
        module.Services.AddMongoCollection<MongoEstimationSnapshot>("EstimationSnapshots");
        module.Services.AddSingleton<IEstimationSnapshotStore, MongoEstimationSnapshotStore>();
    }
}
