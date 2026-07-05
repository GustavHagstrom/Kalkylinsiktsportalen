using BlazorServer.Shared.Constants;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Domain;
public sealed class AccessClaim : Claim
{
    public AccessClaim(string value) : base(ApplicationClaimTypes.AccessClaim, value)
    {

    }
}
