using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Access.Authentication;

namespace BlazorServer.Modules.Access.Features.GetEffectiveFunctionalAccessClaims;

public abstract record GetEffectiveFunctionalAccessClaimsResult;

public sealed record EffectiveFunctionalAccessClaimsLoaded(
    UserId UserId,
    OrganizationId OrganizationId,
    IReadOnlyList<AccessClaim> AccessClaims) : GetEffectiveFunctionalAccessClaimsResult;

public sealed record InvalidGetEffectiveFunctionalAccessClaimsRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GetEffectiveFunctionalAccessClaimsResult;
