
using BlazorServer.Identity;

namespace BlazorServer.Modules.Access.Authentication;

public interface IAccessClaimsCache
{
    void ClearAccessClaims(UserId userId, OrganizationId orgId);
    void SetAccessClaims(UserId userId, OrganizationId orgId, List<string> claimValues);
    bool TryGetAccessClaims(UserId userId, OrganizationId orgId, out List<string>? claimValues);
}
