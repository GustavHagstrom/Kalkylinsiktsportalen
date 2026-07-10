using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed record ImportEstimationSnapshotCommand(
    OrganizationId OrganizationId,
    BidconEstimationId BidconEstimationId) : ICommand<ImportEstimationSnapshotResult>;
