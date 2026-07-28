using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.GrantUserViewAccess;

public sealed record GrantUserViewAccessCommand(
    ViewId ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right) : ICommand<GrantUserViewAccessResult>;
