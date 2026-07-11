using BlazorServer.Shared.Models;

namespace BlazorServer.AppShell.Authentication;

public interface IAuthService
{
    Task LoginAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    User? User { get; }
    string? OrganizationId { get; }
    string? UserEmail { get; }
    Task<User?> UpdateUserAsync();
}
