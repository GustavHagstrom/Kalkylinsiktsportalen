using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;
public sealed class AccessClaim : Claim
{
    public AccessClaim(string value) : base(ApplicationClaimConstants.AccessClaimType, value)
    {

    }
}
