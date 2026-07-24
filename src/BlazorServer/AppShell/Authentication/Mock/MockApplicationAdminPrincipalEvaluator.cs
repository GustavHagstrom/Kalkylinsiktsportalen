using BlazorServer.Modules.Access.Public.Authorization;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication.Mock;

public sealed class MockApplicationAdminPrincipalEvaluator : IApplicationAdminPrincipalEvaluator
{
    internal const string ApplicationAdminClaimType = "MockApplicationAdmin";
    internal const string ApplicationAdminClaimValue = "true";

    public bool IsApplicationAdmin(ClaimsPrincipal principal)
        => principal.HasClaim(ApplicationAdminClaimType, ApplicationAdminClaimValue);
}
