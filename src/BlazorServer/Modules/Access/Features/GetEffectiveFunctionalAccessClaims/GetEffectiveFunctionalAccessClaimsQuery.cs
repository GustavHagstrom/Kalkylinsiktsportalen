using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Access.Features.GetEffectiveFunctionalAccessClaims;

public sealed record GetEffectiveFunctionalAccessClaimsQuery(
    UserId UserId,
    IReadOnlyCollection<string> RoleIds,
    OrganizationId OrganizationId) : IQuery<GetEffectiveFunctionalAccessClaimsResult>;
