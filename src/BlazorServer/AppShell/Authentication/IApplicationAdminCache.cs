namespace BlazorServer.AppShell.Authentication;

public interface IApplicationAdminCache
{
    void SetIsAdminRole(string userId, string organizationId);
    bool TryGetIsAdminRole(string userId, string organizationId, out bool isAdmin);
    void ClearIsAdminRole(string userId, string organizationId);
}
