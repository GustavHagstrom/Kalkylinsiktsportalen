using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public interface IAuthService
{
    Task LoginAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    User? User { get; }
    string? OrganizationId { get; }
    
}
