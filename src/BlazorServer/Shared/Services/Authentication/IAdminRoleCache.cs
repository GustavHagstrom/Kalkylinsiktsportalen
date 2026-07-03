namespace BlazorServer.Shared.Services.Authentication;

public interface IAdminRoleCache
{
    void SetIsAdminRole(string userId, string orgId);
    bool TryGetIsAdminRole(string userId, string orgId, out bool isAdmin);
    void ClearIsAdminRole(string userId, string orgId);
}
