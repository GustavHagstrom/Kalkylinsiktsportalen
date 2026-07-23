namespace BlazorServer.Modules.Access.Infrastructure.Caching;

internal interface IApplicationAdminCache
{
    void SetApplicationAdminStatus(string userId, string organizationId, bool isApplicationAdmin);
    bool TryGetApplicationAdminStatus(string userId, string organizationId, out bool isApplicationAdmin);
}
