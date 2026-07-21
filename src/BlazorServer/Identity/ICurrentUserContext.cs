using System.Security.Claims;

namespace BlazorServer.Identity;

public interface ICurrentUserContext
{
    /// <summary>
    /// Gets the current authenticated user, or null if the user is not authenticated.
    /// </summary>
    CurrentUser? CurrentUser { get; }
    //bool IsAuthenticated { get; }
}