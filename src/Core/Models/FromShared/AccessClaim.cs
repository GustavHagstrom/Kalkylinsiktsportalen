using Core.Constants;
using System.Security.Claims;

namespace Core.Models.FromShared;
public class AccessClaim : Claim
{
    public AccessClaim(string value) : base(SharedApplicationClaimTypes.AccessClaim, value)
    {

    }
}
