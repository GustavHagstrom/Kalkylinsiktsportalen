namespace BlazorServer.AppShell.Authentication;

public interface IApplicationAdminLookup
{
    Task<bool> IsApplicationAdminAsync(string userId, string organizationId);
}
