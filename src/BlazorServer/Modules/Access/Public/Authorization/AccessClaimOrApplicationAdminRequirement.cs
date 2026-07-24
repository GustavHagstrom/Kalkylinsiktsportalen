using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Modules.Access.Public.Authorization;

internal sealed class AccessClaimOrApplicationAdminRequirement(string accessClaimValue) : IAuthorizationRequirement
{
    public string AccessClaimValue { get; } = accessClaimValue;
}

internal sealed class AccessClaimOrApplicationAdminHandler(
    IApplicationAdminPrincipalEvaluator applicationAdminEvaluator)
    : AuthorizationHandler<AccessClaimOrApplicationAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccessClaimOrApplicationAdminRequirement requirement)
    {
        if (context.User.HasClaim(AccessAuthorizationConstants.AccessClaimType, requirement.AccessClaimValue)
            || applicationAdminEvaluator.IsApplicationAdmin(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
