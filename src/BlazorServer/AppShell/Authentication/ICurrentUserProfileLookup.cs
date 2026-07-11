namespace BlazorServer.AppShell.Authentication;

public interface ICurrentUserProfileLookup
{
    Task<SessionUser> GetProfileAsync(CurrentUser currentUser);
}
