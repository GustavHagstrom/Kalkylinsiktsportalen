using System.Security.Claims;
using BlazorServer.Modules.Access.Public.Authorization;

namespace BlazorServer.Modules.Access.Authentication;

public sealed class AccessClaim : Claim
{
    public AccessClaim(string value)
        : base(AccessAuthorizationConstants.AccessClaimType, value)
    {
    }
}
