using Core.Models.FromShared;
using System.Security.Claims;

namespace Data.Abstractions;
public interface IAccessClaimStore
{
    Task<IEnumerable<AccessClaim>> GetAccessClaims(ClaimsPrincipal? userClaims);
    Task<IEnumerable<AccessClaim>> GetAccessClaims(string userId);
}