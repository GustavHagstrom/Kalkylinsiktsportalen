namespace BlazorServer.AppShell.Authentication;

public class MockApplicationAdminLookup : IApplicationAdminLookup
{
    public Task<bool> IsApplicationAdminAsync(string userId, string organizationId)
    {
        return Task.FromResult(userId == "Admin");
    }
}
