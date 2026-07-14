using BlazorServer.Identity;

namespace BlazorServer.AppShell.Authentication;

public interface ICurrentUserProfileLookup
{
    Task<CurrentUser> GetProfileAsync(CurrentUser currentUser);
}
