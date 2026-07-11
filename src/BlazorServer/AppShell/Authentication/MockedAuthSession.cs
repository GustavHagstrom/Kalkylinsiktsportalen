namespace BlazorServer.AppShell.Authentication;

public class MockedAuthSession
{
    public SessionUser? User { get; private set; }
    public string? OrganizationId { get; private set; }

    public void SetSession(SessionUser user, string orgId)
    {
        User = user;
        OrganizationId = orgId;
    }

    public void ClearSession()
    {
        User = null;
        OrganizationId = null;
    }
}
