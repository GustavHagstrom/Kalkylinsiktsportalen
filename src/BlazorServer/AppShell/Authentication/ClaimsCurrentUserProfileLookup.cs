namespace BlazorServer.AppShell.Authentication;

public class ClaimsCurrentUserProfileLookup : ICurrentUserProfileLookup
{
    public Task<SessionUser> GetProfileAsync(CurrentUser currentUser)
    {
        return Task.FromResult(new SessionUser(
            currentUser.UserId,
            currentUser.DisplayName,
            currentUser.Email,
            null));
    }
}
