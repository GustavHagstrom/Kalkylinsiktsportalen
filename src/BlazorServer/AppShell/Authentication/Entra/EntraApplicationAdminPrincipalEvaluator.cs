using BlazorServer.Modules.Access.Public.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication.Entra;

public sealed class EntraApplicationAdminPrincipalEvaluator : IApplicationAdminPrincipalEvaluator
{
    private const string ExternalDirectoryRoleClaimType = "wids";
    private const string ApplicationAdminRoleClaimValue = "62e90394-69f5-4237-9190-012177145e10";

    public bool IsApplicationAdmin(ClaimsPrincipal principal)
        => principal.HasClaim(ExternalDirectoryRoleClaimType, ApplicationAdminRoleClaimValue);
}
