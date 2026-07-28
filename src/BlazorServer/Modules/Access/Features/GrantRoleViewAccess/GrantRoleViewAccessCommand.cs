using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.GrantRoleViewAccess;

public sealed record GrantRoleViewAccessCommand(
    ViewId ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right) : ICommand<GrantRoleViewAccessResult>;
