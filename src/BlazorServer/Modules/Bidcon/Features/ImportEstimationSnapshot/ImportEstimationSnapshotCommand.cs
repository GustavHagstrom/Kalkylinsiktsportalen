using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed record ImportEstimationSnapshotCommand(
    OrganizationId OrganizationId,
    BidconEstimationId BidconEstimationId) : ICommand<ImportEstimationSnapshotResult>;
