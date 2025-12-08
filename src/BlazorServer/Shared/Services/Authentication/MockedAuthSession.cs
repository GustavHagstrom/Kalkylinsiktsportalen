using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedAuthSession
{
    public User? User { get; private set; } 
    public string? OrganizationId { get; private set; }
    public void SetSession(User user, string orgId)
    {
        User = user;
        OrganizationId = orgId;
    }
    public void ClearSession() { User = null; OrganizationId = null; }
}
