using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.RevokeRoleViewAccess;

public sealed record RevokeRoleViewAccessCommand(
    ViewId ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right) : ICommand<RevokeRoleViewAccessResult>;
