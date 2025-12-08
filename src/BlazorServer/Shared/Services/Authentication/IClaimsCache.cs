
namespace BlazorServer.Shared.Services.Authentication;

public interface IClaimsCache
{
    void ClearAccessClaims(string userId, string orgId);
    void SetAccessClaims(string userId, string orgId, List<string> claimValues);
    bool TryGetAccessClaims(string userId, string orgId, out List<string>? claimValues);

    void SetIsAdminRole(string userId, string orgId);
    bool TryGetIsAdminRole(string userId, string orgId, out bool isAdmin);
    void ClearIsAdminRole(string userId, string orgId);
}