using BlazorServer.Modules.Bidcon.Contracts;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed record ImportEstimationSnapshotCommand(
    OrganizationId OrganizationId,
    BidconEstimationId BidconEstimationId) : ICommand<ImportEstimationSnapshotResult>;
