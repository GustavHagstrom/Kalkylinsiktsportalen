using BlazorServer.Modules.Bidcon.Contracts;
using BlazorServer.Modules.Bidcon.Domain;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public interface IEstimationSnapshotStore
{
    Task AddAsync(EstimationSnapshot snapshot, CancellationToken cancellationToken);

    Task<EstimationSnapshot?> GetAsync(EstimationSnapshotId snapshotId, CancellationToken cancellationToken);
}
