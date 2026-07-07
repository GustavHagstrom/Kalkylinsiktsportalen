using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public interface IEstimationSnapshotStore
{
    Task AddAsync(EstimationSnapshot snapshot, CancellationToken cancellationToken);

    Task<EstimationSnapshot?> GetAsync(EstimationSnapshotId snapshotId, CancellationToken cancellationToken);
}
