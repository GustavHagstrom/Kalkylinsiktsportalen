using BlazorServer.Identity;

namespace BlazorServer.AppShell.Authentication.Mock;

public class MockProfileLookup : ICurrentUserProfileLookup
{

    Task<CurrentUser> ICurrentUserProfileLookup.GetProfileAsync(CurrentUser currentUser)
    {
        return Task.FromResult(new CurrentUser(
            currentUser.UserId,
            currentUser.OrganizationId,
            currentUser.DisplayName,
            currentUser.Email));
    }
}
