using BlazorServer.Shared.Constants;
using System.Security.Claims;

namespace BlazorServer.Shared.Models;
public class AccessClaim : Claim
{
    public AccessClaim(string value) : base(ApplicationClaimTypes.AccessClaim, value)
    {

    }
}
