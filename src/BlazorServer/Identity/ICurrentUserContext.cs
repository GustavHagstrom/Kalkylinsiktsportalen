namespace BlazorServer.Identity;

public interface ICurrentUserContext
{
    ValueTask<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
