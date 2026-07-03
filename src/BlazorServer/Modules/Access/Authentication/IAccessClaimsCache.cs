
namespace BlazorServer.Modules.Access.Authentication;

public interface IAccessClaimsCache
{
    void ClearAccessClaims(string userId, string orgId);
    void SetAccessClaims(string userId, string orgId, List<string> claimValues);
    bool TryGetAccessClaims(string userId, string orgId, out List<string>? claimValues);
}
