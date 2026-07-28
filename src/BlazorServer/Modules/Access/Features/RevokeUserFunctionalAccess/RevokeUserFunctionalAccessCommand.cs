using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Access.Features.RevokeUserFunctionalAccess;

public sealed record RevokeUserFunctionalAccessCommand(
    UserId UserId,
    OrganizationId OrganizationId,
    string AccessClaimValue) : ICommand<RevokeUserFunctionalAccessResult>;
