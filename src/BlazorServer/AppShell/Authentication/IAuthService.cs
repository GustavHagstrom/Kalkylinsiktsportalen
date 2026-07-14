using BlazorServer.Identity;
using System.Security.Claims;

namespace BlazorServer.AppShell.Authentication;

public interface IAuthService : ICurrentUserContext
{
    Task LoginAsync();
    Task LogoutAsync();
    Task<CurrentUser?> RefreshAsync();
    IReadOnlyList<Claim> Claims { get; }

}
