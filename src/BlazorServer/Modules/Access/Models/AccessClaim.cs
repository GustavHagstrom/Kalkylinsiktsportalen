using BlazorServer.Shared.Constants;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Models;
public class AccessClaim : Claim
{
    public AccessClaim(string value) : base(ApplicationClaimTypes.AccessClaim, value)
    {

    }
}
