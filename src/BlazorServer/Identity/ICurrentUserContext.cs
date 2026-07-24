namespace BlazorServer.Identity;

public interface ICurrentUserContext
{
    Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
