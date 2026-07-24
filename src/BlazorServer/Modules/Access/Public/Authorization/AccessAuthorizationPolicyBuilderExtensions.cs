using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Modules.Access.Public.Authorization;

public static class AccessAuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireAccessClaimOrApplicationAdmin(
        this AuthorizationPolicyBuilder builder,
        string accessClaimValue)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessClaimValue);

        builder.Requirements.Add(new AccessClaimOrApplicationAdminRequirement(accessClaimValue));
        return builder;
    }
}
